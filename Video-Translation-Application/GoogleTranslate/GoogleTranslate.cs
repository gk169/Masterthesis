using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace VideoTranslationTool.TextToTextModule
{
    [Export(typeof(TextToText))]
    /// <summary>
    /// Public class <c>GoogleTranslate</c> for google translate module
    /// </summary>
    public class GoogleTranslate : TextToText
    {
        #region Members
        private Dictionary<string, string> _languageCodeDictionary; // Language - LanguageCode
        #endregion Members

        #region Constructors
        /// <summary>
        /// Constructor for <c>GoogleTranslate</c> class
        /// </summary>
        public GoogleTranslate() : base(name: nameof(GoogleTranslate)) { }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// See <see cref="TextToText.LoadSupportedTranslations"/>
        /// </summary>
        protected override Dictionary<string, List<string>> LoadSupportedTranslations()
        {
            string filePath = @"GoogleTranslate_SupportedLanguages.txt";

            // Read Supported Languages from file
            string file = File.ReadAllText(filePath);

            // Split after each new line
            string[] lines = file.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            // Discard first and second entry (SourceUrl \n Language - ISO6391Code)
            lines = lines[2..lines.Length];

            // Delete previous dictionary
            _languageCodeDictionary = new Dictionary<string, string>();

            // Split line in language ([0]) and language code ([1])
            foreach (string line in lines)
            {
                string[] languageParts = line.Split(" \t", StringSplitOptions.RemoveEmptyEntries);
                _languageCodeDictionary.TryAdd(languageParts[0], languageParts[1]);
            }

            // Get all languages
            List<string> sourceLanguages = _languageCodeDictionary.Keys.ToList();

            // Allow to translate every language into any
            Dictionary<string, List<string>> translations = new();

            foreach (string sourceLanguage in sourceLanguages)
            {
                int indexOfSourceLanguage = sourceLanguages.IndexOf(sourceLanguage);
                List<string> targetLanguages = sourceLanguages.ToList();
                targetLanguages.RemoveAt(indexOfSourceLanguage);
                translations.Add(sourceLanguage, targetLanguages);
            }

            return translations;
        }

        /// <summary>
        /// See <see cref="TextToText.Translate(string)"/>
        /// </summary>
        public override string Translate(string sourceText, string sourceLanguage, string targetLanguage)
        {
            /* Arguments */
            string sourceLanguageCode = _languageCodeDictionary[sourceLanguage];
            string targetLanguageCode = _languageCodeDictionary[targetLanguage];
            string outputTextPath = Path.GetTempPath() + "TranslatedText.txt";

            /* Transform arguments https://www.btelligent.com/blog/best-practice-arbeiten-in-python-mit-pfaden-teil-1/ */
            string outputTextPath_Unix = outputTextPath.Replace(@"\", "/");
            string sourceText_Unix = sourceText.Replace("\r\n", "\n");

            /* Executable */

            //// Option 1) Python script
            //string executable = @"C:\ProgramData\Anaconda3\python.exe";
            //string script = @"D:\GitRepos\Masterthesis\git\Text-To-Text-Translation\GoogleTranslate_Script.py";
            //string arguments = $"\"{script}\" \"{sourceLanguageCode}\" \"{targetLanguageCode}\" \"{sourceText_Unix}\" \"{outputTextPath_Unix}\"";

            // Option 2) Generated executable
            string executable = @"GoogleTranslate.exe";
            string arguments = $"\"{sourceLanguageCode}\" \"{targetLanguageCode}\" \"{sourceText_Unix}\" \"{outputTextPath_Unix}\"";

            /* Process executable */
            ProcessStartInfo processStartInfo = new()
            {
                FileName = executable,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
            };

            string errors = "";
            using (Process process = Process.Start(processStartInfo)) { errors = process.StandardError.ReadToEnd(); }

            /* Handle errors and output */
            if (errors != "") throw new Exception(errors);
            else return File.ReadAllText(outputTextPath);
        }
        #endregion Methods
    }
}
