using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using VideoTranslationTool.Commands;

namespace VideoTranslationTool.TextToSpeechModule
{
    /// <summary>
    /// Public abstract class <c>TextToSpeechViewModel</c> as view model for TextToSpeech view
    /// </summary>
    public class TextToSpeechViewModel : ModuleViewModel
    {
        #region Members
        private string _audioFilePath;
        private string _language;
        private TextToSpeech _module;
        [ImportMany(typeof(TextToSpeech))] private List<TextToSpeech> _supportedModules = null;
        private ICommand _synthesizeCommand;
        private string _text;
        private string _voice;
        #endregion Members

        #region Properties
        /// <summary>
        /// Public property <c>AudioPath</c> to get / private set the audio file path as string
        /// </summary>
        public string AudioPath
        {
            get { return _audioFilePath; }
            private set
            {
                _audioFilePath = value;
                OnPropertyChanged(nameof(AudioPath));
            }
        }

        /// <summary>
        /// Public property <c>Language</c> to get / set the language of the text
        /// </summary>
        public string Language
        {
            get { return _language; }
            set
            {
                _language = value;

                OnPropertyChanged(nameof(Language));

                OnPropertyChanged(nameof(SupportedVoices));
                OnPropertyChanged(nameof(Voice));
            }
        }

        /// <summary>
        /// Public property <c>Module</c> to get / set the module currently used for TextToSpeech synthetization
        /// </summary>
        public TextToSpeech Module
        {
            get { return _module; }
            set
            {
                _module = value;
                OnPropertyChanged(nameof(Module));

                OnPropertyChanged(nameof(SupportedLanguages));
                OnPropertyChanged(nameof(Language));

                OnPropertyChanged(nameof(SupportedVoices));
                OnPropertyChanged(nameof(Voice));
            }
        }

        /// <summary>
        /// Public property <c>SupportedLanguages</c> to get a list of supported languages
        /// </summary>
        public List<string> SupportedLanguages => _module.SupportedVoices.Keys.ToList();

        /// <summary>
        /// Public property <c>SupportedModules</c> to get the supported TextToSpeech modules
        /// </summary>
        public List<TextToSpeech> SupportedModules => _supportedModules;

        /// <summary>
        /// Public property <c>SupportedVoices</c> to get a list of supported voices
        /// </summary>
        public List<string> SupportedVoices
        {
            get
            {
                if (Language is null) return null;
                else return _module.SupportedVoices[Language];
            }
        }

        /// <summary>
        /// Public command <c>SynthesizeCommand</c> calls synthesize method if it is executable
        /// </summary>
        public ICommand SynthesizeCommand
        {
            get
            {
                if (_synthesizeCommand is null) _synthesizeCommand = new RelayCommand(param => this.Synthesize(), param => this.CanSynthesize());
                return _synthesizeCommand;
            }
        }

        /// <summary>
        /// Public property <c>Text</c> to get / set the text which shall be synthesized as string
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
        /// Public property <c>Voice</c> to get / set the voice of the audio
        /// </summary>
        public string Voice
        {
            get { return _voice; }
            set
            {
                _voice = value;
                OnPropertyChanged(nameof(Voice));
            }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Constructor for class <c>TextToSpeechViewModel</c>
        /// </summary>
        public TextToSpeechViewModel()
        {
            // Load TextToSpeech module plugins
            var catalog = new DirectoryCatalog(".");
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            // Set default module
            Module = _supportedModules[0];

            // Set default language
            if (SupportedLanguages.Contains("German")) Language = "German";
            else if (SupportedLanguages.Contains("German (Germany)")) Language = "German (Germany)";
            else if (SupportedLanguages.Contains("English")) Language = "English";
            else if (SupportedLanguages.Contains("English (United States)")) Language = "English (United States)";
            else Language = SupportedLanguages[0];

            // Set default voice
            Voice = SupportedVoices[0];
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Private method <c>CanExport</c> indicates if export method is executable
        /// </summary>
        /// <returns>
        /// True if executable, otherwise false
        /// </returns>
        protected override bool CanExport() => AudioPath is not null and not "";

        /// <summary>
        /// See <see cref="ModuleViewModel.CanNext"/>
        /// </summary>
        /// <returns>
        /// See <see cref="ModuleViewModel.CanNext"/>
        /// </returns>
        public override bool CanNext() => AudioPath is not null and not "";

        /// <summary>
        /// Private method <c>CanSynthesize</c> indicates if synthesize method is executable
        /// </summary>
        /// <returns>
        /// True if executable, otherwise false
        /// </returns>
        private bool CanSynthesize() => Module is not null
                                        && Language is not null and not ""
                                        && Voice is not null
                                        && Text is not null and not "";

        /// <summary>
        /// Private method <c>Export</c> to export the synthesized audio file
        /// </summary>
        protected override void Export()
        {
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "Audio file (*.mp3)|*.mp3";
            if (saveFileDialog.ShowDialog() == true) File.Copy(AudioPath, saveFileDialog.FileName);
        }

        /// <summary>
        /// See <see cref="ModuleViewModel.GetResultforNextStep"/>
        /// </summary>
        /// <returns>
        /// See <see cref="ModuleViewModel.GetResultforNextStep"/>
        /// </returns>
        public override string GetResultforNextStep() => AudioPath;

        /// <summary>
        /// See <see cref="ModuleViewModel.SetResultOfPreviousStep(string)"/>
        /// </summary>
        /// <param name="resultsOfPrevious">
        /// See <see cref="ModuleViewModel.SetResultOfPreviousStep(string)"/>
        /// </param>
        public override void SetResultOfPreviousStep(string resultsOfPrevious) => Text = resultsOfPrevious;

        /// <summary>
        /// Private method <c>Synthesize</c> to synthesize the given text to a audio file
        /// </summary>
        private void Synthesize()
        {
            Cursor previousCursor = Application.Current.MainWindow.Cursor;
            Application.Current.MainWindow.Cursor = System.Windows.Input.Cursors.Wait;

            string newAudioPath = null;

            try { newAudioPath = _module.Synthesize(Text, Language, Voice); }
            catch (Exception e) { MessageBox.Show(e.ToString()); }

            if (newAudioPath is not null) AudioPath = newAudioPath;

            Application.Current.MainWindow.Cursor = previousCursor;
        }
        #endregion Methods
    }
}
