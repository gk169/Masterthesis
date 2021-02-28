using System.Collections.Generic;
using System.Linq;

namespace User_Interface
{
    public abstract class TextToSpeech
    {
        private readonly string _name;
        private readonly IDictionary<string, List<string>> _supportedVoices; // Langauge, List<Voice>
        private string _language;
        private string _voice;
        private string _text;

        protected TextToSpeech(string name)
        {
            _name = name;
            _supportedVoices = LoadSupportedVoices();
            _language = _supportedVoices.Keys.ToList()[0];
            _voice = _supportedVoices[_language][0];
            _text = "Text to be synthesized ...";
        }

        public string Name
        {
            get { return _name; }
        }

        public List<string> SupportedLanguages
        {
            get { return _supportedVoices.Keys.ToList(); }
        }

        public List<string> SupportedVoices
        {
            get { return _supportedVoices[_language]; }
        }

        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }

        public string Voice
        {
            get { return _voice; }
            set { _voice = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        protected abstract Dictionary<string, List<string>> LoadSupportedVoices();
    }
}
