using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User_Interface
{
    public class TextToTextViewModel : ViewModel
    {
        private readonly List<TextToText> _supportedModules;
        private TextToText _module;

        public TextToTextViewModel()
        {
            _supportedModules = new List<TextToText>()
            {
                new MarianMT(), new GoogleTranslate(),
            };

            _module = _supportedModules[0];
        }

        public List<TextToText> SupportedModules
        {
            get { return _supportedModules; }
        }

        public TextToText Module
        {
            get { return _module; }
            set
            {
                _module = value;
                OnPropertyChanged(nameof(Module));
                OnPropertyChanged(nameof(SupportedSourceLanguages));
                OnPropertyChanged(nameof(SourceLanguage));
                OnPropertyChanged(nameof(SupportedTargetLanguages));
                OnPropertyChanged(nameof(TargetLanguage));
            }
        }

        public List<string> SupportedSourceLanguages
        {
            get { return _module.SupportedSourceLanguages; }
        }

        public List<string> SupportedTargetLanguages
        {
            get { return _module.SupportedTargetLanguages; }
        }

        public string SourceLanguage
        {
            get { return _module.SourceLanguage; }
            set
            {
                _module.SourceLanguage = value;
                OnPropertyChanged(nameof(SourceLanguage));
                OnPropertyChanged(nameof(SupportedTargetLanguages));
                OnPropertyChanged(nameof(TargetLanguage));
            }
        }

        public string TargetLanguage
        {
            get { return _module.TargetLanguage; }
            set
            {
                _module.TargetLanguage = value;
                OnPropertyChanged(nameof(TargetLanguage));
            }
        }

        public string SourceText
        {
            get { return _module.SourceText; }
            set
            {
                _module.SourceText = value;
                OnPropertyChanged(nameof(SourceText));
            }
        }

        public string TargetText
        {
            get { return _module.TargetText; }
            set
            {
                _module.TargetText = value;
                OnPropertyChanged(nameof(TargetText));
            }
        }
    }
}
