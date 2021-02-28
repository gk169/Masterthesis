using System.Collections.Generic;

namespace User_Interface
{
    public class SpeechToVideoViewModel: ViewModel
    {
        private readonly List<SpeechToVideo> _supportedModules;
        private SpeechToVideo _module;

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
    }
}
