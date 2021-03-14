using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VideoTranslationTool.SpeechToTextModule
{
    public class Sphinx : SpeechToText
    {
        private Dictionary<string, string> _languageCodeDictionary; // Language - LanguageCode

        public Sphinx() : base(name: nameof(Sphinx)) { }

        public override string Transcribe(string audioPath, string audioLanguage)
        {
            //TODO
            return audioPath;
        }

        protected override List<string> LoadSupportedAudioLanguages()
        {
            /// hot to handle this with the weights at different location than usual?
            string filePath = @"1_SpeechToText\Sphinx\Sphinx_SupportedLanguages.txt";

            // Read Supported Languages from file
            string file = File.ReadAllText(filePath);

            // Split after each new line
            string[] lines = file.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            // Discard first and second entry (SourceUrl \n Language - BCP47Code - ...)
            lines = lines[2..lines.Length];

            _languageCodeDictionary = new Dictionary<string, string>();

            // Split line in language ([0]) and language code ([1]) and rest
            foreach (string line in lines)
            {
                string[] languageParts = line.Split(" \t", StringSplitOptions.RemoveEmptyEntries);

                _languageCodeDictionary.TryAdd(languageParts[0], languageParts[1]);
            }

            return _languageCodeDictionary.Keys.ToList();
        }
    }
}
