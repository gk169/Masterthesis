using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User_Interface
{
    public abstract class SpeechToText
    {
        private readonly string _name;
        private readonly List<string> _supportedLanguages;
        private string _audioLanguage;

        public string Name
        {
            get { return _name; }
        }

        public List<string> SupportedLanguages
        {
            get { return _supportedLanguages; }
        }

        public string AudioLanguage
        {
            get { return _audioLanguage; }
            set { _audioLanguage = value; }
        }

        protected SpeechToText(string name)
        {
            _name = name;
            _supportedLanguages = LoadSupportedLanguages();
            _audioLanguage = _supportedLanguages[0];
        }

        protected abstract List<string> LoadSupportedLanguages();
    }
}
