using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Windows;
using System.Windows.Input;
using VideoTranslationTool.Commands;

namespace VideoTranslationTool.SpeechToVideoModule
{
    /// <summary>
    /// Public class <c>SpeechToVideoViewModel</c> is view model for SpeechToVideo View
    /// </summary>
    public class SpeechToVideoViewModel: ModuleViewModel
    {
        #region Members
        private ICommand _generateCommand;
        private string _inputAudioFilePath;
        private string _inputVideoFilePath = "";
        private SpeechToVideo _module;
        private ICommand _openAudioFileCommand;
        private ICommand _openVideoFileCommand;
        private string _outputVideoPath = "";
        [ImportMany(typeof(SpeechToVideo))] private List<SpeechToVideo> _supportedModules = null;
        #endregion Members

        #region Properties
        /// <summary>
        /// Public command <c>GenerateCommand</c> calls Generate method if executable
        /// </summary>
        public ICommand GenerateCommand
        {
            get
            {
                if (_generateCommand is null) _generateCommand = new RelayCommand(param => this.Generate(), param => this.CanGenerate());
                return _generateCommand;
            }
        }

        /// <summary>
        /// Public property <c>InputAudioPath</c> to get / set the input audio path as string
        /// </summary>
        public string InputAudioPath
        {
            get { return _inputAudioFilePath; }
            set
            {
                _inputAudioFilePath = value;
                OnPropertyChanged(nameof(InputAudioPath));
            }
        }

        /// <summary>
        /// Public property <c>InputVideoPath</c> to get / set the input video path as string
        /// </summary>
        public string InputVideoPath
        {
            get { return _inputVideoFilePath; }
            set
            {
                _inputVideoFilePath = value;
                OnPropertyChanged(nameof(InputVideoPath));
            }
        }

        /// <summary>
        /// Public property <c>Module</c> to get / set the module used for video generation
        /// </summary>
        public SpeechToVideo Module
        {
            get { return _module; }
            set
            {
                _module = value;
                OnPropertyChanged(nameof(Module));
            }
        }

        /// <summary>
        /// Public command <c>OpenAudioFileCommand</c> calls OpenAudioFile method if executable
        /// </summary>
        public ICommand OpenAudioFileCommand
        {
            get
            {
                if (_openAudioFileCommand is null) _openAudioFileCommand = new RelayCommand(param => this.OpenAudioFile(), param => CanOpenAudioFile());
                return _openAudioFileCommand;
            }
        }

        /// <summary>
        /// Public command <c>OpenVideoFileCommand</c> calls OpenVideoFile method if executable
        /// </summary>
        public ICommand OpenVideoFileCommand
        {
            get
            {
                if (_openVideoFileCommand is null) _openVideoFileCommand = new RelayCommand(param => this.OpenVideoFile(), param => CanOpenVideoFile());
                return _openVideoFileCommand;
            }
        }

        /// <summary>
        /// Public property <c>OutputVideoPath</c> to get / private set the output video path as string
        /// </summary>
        public string OutputVideoPath
        {
            get { return _outputVideoPath; }
            private set
            {
                _outputVideoPath = value;
                OnPropertyChanged(nameof(OutputVideoPath));
            }
        }

        /// <summary>
        /// Public property <c>SupportedModules</c> to get the list of supported modules for video generation
        /// </summary>
        public List<SpeechToVideo> SupportedModules => _supportedModules;
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Constructor of <c>SpeechToVideoViewModel</c> class
        /// </summary>
        public SpeechToVideoViewModel()
        {
            // Load SpeechToText module plugins
            var catalog = new DirectoryCatalog(".");
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            // Set default module
            Module = _supportedModules[0];
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// See <see cref="ModuleViewModel.CanExport"/>
        /// </summary>
        /// <returns>
        /// See <see cref="ModuleViewModel.CanExport"/>
        /// </returns>
        protected override bool CanExport() => OutputVideoPath is not null and not "";

        /// <summary>
        /// Private method <c>CanGenerate</c> indicates if Generate method is executable
        /// </summary>
        /// <returns>
        /// True if executable, otherwise false
        /// </returns>
        private bool CanGenerate() => _module is not null
                                      && InputAudioPath is not null and not ""
                                      && InputVideoPath is not null and not "";

        /// <summary>
        /// See <see cref="ModuleViewModel.CanNext"/>
        /// </summary>
        /// <returns>
        /// See <see cref="ModuleViewModel.CanNext"/>
        /// </returns>
        public override bool CanNext() => false;        // no next step available -> always false

        /// <summary>
        /// Private method <c>CanOpenAudioFile</c> indicates if OpenAudioFile method is executable
        /// </summary>
        /// <returns>
        /// True if executable, otherwise false
        /// </returns>
        public static bool CanOpenAudioFile() => true;  // always true

        /// <summary>
        /// Private method <c>CanOpenVideoFile</c> indicates if OpenVideoFile method is executable
        /// </summary>
        /// <returns>
        /// True if executable, otherwise false
        /// </returns>
        public static bool CanOpenVideoFile() => true;  // always true

        /// <summary>
        /// Private method <c>Export</c> exports the generated video to a video file
        /// </summary>
        protected override void Export()
        {
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "Video File (*.mp4)|*.mp4";
            if (saveFileDialog.ShowDialog() == true) File.Copy(OutputVideoPath, saveFileDialog.FileName);
        }

        /// <summary>
        /// Private method <c>Generate</c> calls the Generate method of the module
        /// </summary>
        private void Generate()
        {
            Cursor previousCursor = Application.Current.MainWindow.Cursor;

            Application.Current.MainWindow.Cursor = Cursors.Wait;
            
            OutputVideoPath = null; // Set video to off, unload file

            string newVideoPath = null;

            try { newVideoPath = _module.Generate(InputAudioPath, InputVideoPath); }
            catch (Exception e) { MessageBox.Show(e.ToString()); }

            OutputVideoPath = newVideoPath;

            Application.Current.MainWindow.Cursor = previousCursor;
        }

        /// <summary>
        /// See <see cref="ModuleViewModel.GetResultforNextStep"/>
        /// </summary>
        /// <returns>
        /// See <see cref="ModuleViewModel.GetResultforNextStep"/>
        /// </returns>
        public override string GetResultforNextStep()
        {
            // no next step
            throw new NotImplementedException();
        }

        /// <summary>
        /// Private method <c>OpenAudioFile</c> to show OpenFileDialog and ask the user to select a audio file
        /// </summary>
        private void OpenAudioFile()
        {
            OpenFileDialog openFileDialog = new() { Filter = "Audio file (*.mp3;*.wav)|*.mp3;*.wav", };

            if (InputAudioPath is null or "") openFileDialog.InitialDirectory = @"C:\KeinVerzeichnis";
            else openFileDialog.InitialDirectory = Path.GetDirectoryName(InputAudioPath); // open at latest selection

            if (openFileDialog.ShowDialog() == true) InputAudioPath = openFileDialog.FileName;
        }

        /// <summary>
        /// Private method <c>OpenVideoFile</c> to show OpenFileDialog and ask the user to select a video file
        /// </summary>
        private void OpenVideoFile()
        {
            OpenFileDialog openFileDialog = new() { Filter = "Video file (*.mp4)|*.mp4", };

            if (InputVideoPath is null or "") openFileDialog.InitialDirectory = @"C:\KeinVerzeichnis";
            else openFileDialog.InitialDirectory = Path.GetDirectoryName(InputVideoPath); // open at latest selection

            if (openFileDialog.ShowDialog() == true) InputVideoPath = openFileDialog.FileName;
        }

        /// <summary>
        /// See <see cref="ModuleViewModel.SetResultOfPreviousStep(string)"/>
        /// </summary>
        /// <param name="resultsOfPrevious">
        /// See <see cref="ModuleViewModel.SetResultOfPreviousStep(string)"/>
        /// </param>
        public override void SetResultOfPreviousStep(string resultsOfPrevious)
        {
            InputAudioPath = resultsOfPrevious;
        }
        #endregion Methods
    }
}
