using System.Collections.Generic;

namespace User_Interface
{
    public class TextToSpeechViewModel : ViewModel
    {
        private readonly List<TextToSpeech> _supportedModules;
        private TextToSpeech _module;

        public string DisplayName => "Step 3:\nText-To-Speech-Translation\n(Speech Synthesis)";

        public List<TextToSpeech> SupportedModules
        {
            get { return _supportedModules; }
        }

        public TextToSpeech Module
        {
            get { return _module; }
            set
            {
                _module = value;
                OnPropertyChanged(nameof(Module));
                OnPropertyChanged(nameof(Language));
                OnPropertyChanged(nameof(Voice));
            }
        }

        public TextToSpeechViewModel()
        {
            _supportedModules = new List<TextToSpeech>()
            {
                new TensorFlowTTS(), new Pyttsx3(), new RTVC(), new GoogleTTS(),
            };

            _module = _supportedModules[0];
        }

        public List<string> SupportedLanguages
        {
            get { return _module.SupportedLanguages; }
        }

        public List<string> SupportedVoices
        {
            get { return _module.SupportedVoices; }
        }

        public string Language
        {
            get { return _module.Language; }
            set
            {
                _module.Language = value;
                OnPropertyChanged(nameof(Language));
                OnPropertyChanged(nameof(Voice));
            }
        }

        public string Voice
        {
            get { return _module.Voice; }
            set
            {
                _module.Voice = value;
                OnPropertyChanged(nameof(Voice));
            }
        }

        public string Text
        {
            get { return _module.Text; }
            set
            {
                _module.Text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
    }
}
