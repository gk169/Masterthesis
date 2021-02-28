using System.Collections.Generic;

namespace User_Interface
{
    public class SpeechToTextViewModel : ViewModel
    {
        private readonly List<SpeechToText> _supportedModules;
        private SpeechToText _module;

        public SpeechToTextViewModel()
        {
            _supportedModules = new List<SpeechToText>()
            {
                new DeepSpeech(), new Sphinx(), new GoogleSTT(),
            };

            _module = _supportedModules[0];
        }

        public List<SpeechToText> SupportedModules
        {
            get
            {
                return _supportedModules;
            }
        }

        public SpeechToText Module
        {
            get { return _module; }
            set
            {
                _module = value;
                OnPropertyChanged(nameof(Module));
            }
        }
    }
}
