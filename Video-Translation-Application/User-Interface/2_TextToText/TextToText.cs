using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User_Interface
{
    public abstract class TextToText
    {
        private readonly string _name;
        private readonly IDictionary <string, List<string>> _supportedLanguages; // SourceLanguage, List<TargetLanguages>
        private string _sourceLanguage;
        private string _targetLanguage;
        private string _sourceText;
        private string _targetText;

        public string Name
        {
            get { return _name; }
        }

        public List<string> SupportedSourceLanguages
        {
            get { return _supportedLanguages.Keys.ToList(); }
        }

        public string SourceLanguage
        {
            get { return _sourceLanguage; }
            set { _sourceLanguage = value; }
        }

        public List<string> SupportedTargetLanguages
        {
            get { return _supportedLanguages[_sourceLanguage]; }
        }

        public string TargetLanguage
        {
            get
            {
                if (!SupportedTargetLanguages.Contains(_targetLanguage))
                {
                    _targetLanguage = SupportedTargetLanguages[0];
                }
                return _targetLanguage;
            }
            set { _targetLanguage = value; }
        }

        public string SourceText
        {
            get { return _sourceText; }
            set { _sourceText = value; }
        }

        public string TargetText
        {
            get { return _targetText; }
            set { _targetText = value; }
        }

        protected TextToText(string name)
        {
            _name = name;
            _supportedLanguages = LoadSupportedLanguages();
            _sourceLanguage = _supportedLanguages.Keys.ToList()[0];
            _targetLanguage = _supportedLanguages[_sourceLanguage][0];
            _sourceText = "Text to be translated ...";
            _targetText = "Translated text ...";
        }

        protected abstract Dictionary<string, List<string>> LoadSupportedLanguages();
    }
}
