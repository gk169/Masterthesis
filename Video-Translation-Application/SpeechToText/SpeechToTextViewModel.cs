using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Windows;
using System.Windows.Input;
using VideoTranslationTool.Commands;

namespace VideoTranslationTool.SpeechToTextModule
{
    /// <summary>
    /// Public class <c>SpeechToTextViewModel</c> is view model for SpeechToTextView
    /// </summary>
    public class SpeechToTextViewModel : ModuleViewModel
    {
        #region Members
        private string _audioLanguage;
        private string _audioPath;
        private SpeechToText _module;
        private ICommand _openAudioFileCommand;
        [ImportMany(typeof(SpeechToText))] private List<SpeechToText> _supportedModules = null;
        private string _text;
        private ICommand _transcribeCommand;
        #endregion Members

        #region Properties
        /// <summary>
        /// Public property <c>AudioLanguage</c> to get / set the language of the audio file as string
        /// </summary>
        public string AudioLanguage
        {
            get { return _audioLanguage; }
            set
            {
                _audioLanguage = value;
                OnPropertyChanged(nameof(AudioLanguage));
            }
        }

        /// <summary>
        /// Public property <c>AudioPath</c> to get / set the path of the audio file to be transcribed as string
        /// </summary>
        public string AudioPath
        {
            get { return _audioPath; }
            set
            {
                _audioPath = value;
                OnPropertyChanged(nameof(AudioPath));
            }
        }

        /// <summary>
        /// Public property <c>Module</c> to get / set the module used for transcription
        /// </summary>
        public SpeechToText Module
        {
            get { return _module; }
            set
            {
                _module = value;
                OnPropertyChanged(nameof(Module));
                OnPropertyChanged(nameof(AudioLanguage));
            }
        }

        /// <summary>
        /// Public commmand <c>OpenAudioFileCommand</c> calls OpenFile method if executable
        /// </summary>
        public ICommand OpenAudioFileCommand
        {
            get
            {
                if (_openAudioFileCommand is null) _openAudioFileCommand = new RelayCommand(param => this.OpenFile(), param => CanOpenFile());
                return _openAudioFileCommand;
            }
        }

        /// <summary>
        /// Public property <c>SupportedModules</c> to get the possible modules for transcription
        /// </summary>
        public List<SpeechToText> SupportedModules => _supportedModules;
        
        /// <summary>
        /// Public property <c>Text</c> to get / set the transcribed text
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        /// <summary>
        /// Public command <c>TranscribeCommand</c> calls Transcribe method if executable
        /// </summary>
        public ICommand TranscribeCommand
        {
            get
            {
                if (_transcribeCommand is null) _transcribeCommand = new RelayCommand(param => this.Transcribe(), param => this.CanTranscribe());
                return _transcribeCommand;
            }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Constructor of class <c>SpeechToTextViewModel</c>
        /// </summary>
        public SpeechToTextViewModel()
        {
            // Load SpeechToText module plugins
            var catalog = new DirectoryCatalog(".");
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            // Set default module
            Module = _supportedModules[0];

            // Set default language
            if (Module.SupportedAudioLanguages.Contains("German")) AudioLanguage = "German";
            else if (Module.SupportedAudioLanguages.Contains("English")) AudioLanguage = "English";
            else AudioLanguage = Module.SupportedAudioLanguages[0];
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Private method <c>CanExport</c> indicates if Export method is executable
        /// </summary>
        /// <returns>
        /// True if executable, otherwise false
        /// </returns>
        protected override bool CanExport() => Text is not null and not "";

        /// <summary>
        /// See <see cref="ModuleViewModel.CanNext"/>
        /// </summary>
        /// <returns>
        /// See <see cref="ModuleViewModel.CanNext"/>
        /// </returns>
        public override bool CanNext() => Text is not null and not "";

        /// <summary>
        /// Private method <c>CanOpenFile</c> indicateds if OpenFile method is executable
        /// </summary>
        /// <returns>
        /// True if executable, otherwise false
        /// </returns>
        private static bool CanOpenFile() => true; // OpenFile is always allowed

        /// <summary>
        /// Private method <c>CanTranscribe</c> indicateds if Transcribe method is executable
        /// </summary>
        /// <returns>
        /// True if executable, otherwise false
        /// </returns>
        private bool CanTranscribe() => Module is not null
                                        && AudioLanguage is not null and not ""
                                        && AudioPath is not null and not "";
        /// <summary>
        /// Private method <c>Export</c> exports the transcribed text to a text-file
        /// </summary>
        protected override void Export()
        {
            SaveFileDialog saveFileDialog = new() { Filter = "Text file (*.txt)|*.txt", };
            if (saveFileDialog.ShowDialog() == true) File.WriteAllText(saveFileDialog.FileName, Text);
        }

        /// <summary>
        /// See <see cref="ModuleViewModel.GetResultforNextStep"/>
        /// </summary>
        /// <returns>
        /// See <see cref="ModuleViewModel.GetResultforNextStep"/>
        /// </returns>
        public override string GetResultforNextStep() => Text;

        /// <summary>
        /// Private method <c>OpenFile</c> to show OpenFileDialog and ask the user to select a audio file
        /// </summary>
        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new() { Filter = "Audio file (*.mp3;*.wav)|*.mp3;*.wav", };

            if (AudioPath is null or "") openFileDialog.InitialDirectory = @"C:\KeinVerzeichnis";
            else openFileDialog.InitialDirectory = Path.GetDirectoryName(AudioPath); // open at latest selection

            if (openFileDialog.ShowDialog() == true) AudioPath = openFileDialog.FileName;
        }

        /// <summary>
        /// See <see cref="ModuleViewModel.SetResultOfPreviousStep(string)"/>
        /// </summary>
        /// <param name="resultsOfPrevious">
        /// See <see cref="ModuleViewModel.SetResultOfPreviousStep(string)"/>
        /// </param>
        public override void SetResultOfPreviousStep(string resultsOfPrevious)
        {
            // no previous step
            throw new NotImplementedException();
        }

        /// <summary>
        /// Private method <c>Transcribe</c> calls the Transcribe method of the module
        /// </summary>
        private void Transcribe()
        {
            string text = null;

            try { text = _module.Transcribe(AudioPath, AudioLanguage); }    // try to transcribe
            catch (Exception e) { MessageBox.Show(e.ToString()); }          // show occurring errors

            if (text is not null) Text = text;
        }
        #endregion Methods
    }
}
