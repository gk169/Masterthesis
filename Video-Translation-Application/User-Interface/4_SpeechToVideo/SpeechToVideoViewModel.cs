using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using VideoTranslationTool.Commands;

namespace VideoTranslationTool.SpeechToVideoModule
{
    public class SpeechToVideoViewModel: ViewModel
    {
        private readonly List<SpeechToVideo> _supportedModules;
        private SpeechToVideo _module;
        private ICommand _exportCommand;
        private ICommand _generateCommand;
        private string _inputVideoFilePath;
        private string _inputAudioFilePath;
        private string _outputVideoPath;

        public string OutputVideoPath
        {
            get { return _outputVideoPath; }
            private set 
            { 
                _outputVideoPath = value;
                OnPropertyChanged(nameof(OutputVideoPath));
            }
        }

        public string InputAudioPath
        {
            get { return _inputAudioFilePath; }
            set
            {
                _inputAudioFilePath = value;
                OnPropertyChanged(nameof(InputAudioPath));
            }
        }

        public string InputVideoPath
        {
            get { return _inputVideoFilePath; }
            set
            {
                _inputVideoFilePath = value;
                OnPropertyChanged(nameof(InputVideoPath));
            }
        }

        public ICommand ExportCommand
        {
            get
            {
                if (_exportCommand == null) _exportCommand = new RelayCommand(param => this.Export(), param => this.CanExport());
                return _exportCommand;
            }
        }

        public ICommand GenerateCommand
        {
            get
            {
                if (_generateCommand == null) _generateCommand = new RelayCommand(param => this.Generate(), param => this.CanGenerate());
                return _generateCommand;
            }
        }

        public SpeechToVideoViewModel()
        {
            _supportedModules = new List<SpeechToVideo>()
            {
                new Wav2Lip(),
            };

            _module = _supportedModules[0];
        }

        public List<SpeechToVideo> SupportedModules
        {
            get { return _supportedModules; }
        }

        public SpeechToVideo Module
        {
            get { return _module; }
            set
            {
                _module = value;
                OnPropertyChanged(nameof(Module));
            }
        }

        private void Export()
        {
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "Video File (*.mp4)|*.mp4";
            if (saveFileDialog.ShowDialog() == true) File.Copy(OutputVideoPath, saveFileDialog.FileName);
        }

        private void Generate()
        {
            string newVideoPath = null;

            try
            {
                newVideoPath = _module.Generate(InputAudioPath, InputVideoPath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            if (newVideoPath != null) OutputVideoPath = newVideoPath;
        }

        private bool CanGenerate()
        {
            if (_module == null || InputAudioPath == null || InputAudioPath == "" || InputVideoPath == null || InputVideoPath == "") return false;
            else return true;
        }

        private bool CanExport()
        {
            if (OutputVideoPath == null || OutputVideoPath == null) return false;
            else return true;
        }
    }
}
