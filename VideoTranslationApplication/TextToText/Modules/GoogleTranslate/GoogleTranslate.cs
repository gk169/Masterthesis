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
            #region Inputs
            string sourceLanguageCode = _languageCodeDictionary[sourceLanguage];
            string targetLanguageCode = _languageCodeDictionary[targetLanguage];
            string inputTextPath = Path.GetTempPath() + "ToTranslateText.txt";
            string outputTextPath = Path.GetTempPath() + "TranslatedText.txt";

            // Transform arguments https://www.btelligent.com/blog/best-practice-arbeiten-in-python-mit-pfaden-teil-1/
            string outputTextPath_Unix = outputTextPath.Replace(@"\", "/");
            string inputTextPath_Unix = inputTextPath.Replace(@"\", "/");
            string sourceText_Unix = sourceText.Replace("\r\n", "\n");

            File.WriteAllText(inputTextPath, sourceText_Unix);

            #endregion Inputs

            #region Process
            #region Option 1: Python script

            string executable = "cmd.exe";

            string script = @"E:\206309_Gann_Kevin\git\Text-To-Text\GoogleTranslate\GoogleTranslate_Script.py";
            string googletranslateArguments = $"\"{script}\" \"{sourceLanguageCode}\" \"{targetLanguageCode}\" \"{inputTextPath_Unix}\" \"{outputTextPath_Unix}\"";

            string arguments = "/c " + @"C:\ProgramData\Anaconda3\Scripts\activate.bat" + "&&" + "activate GoogleTranslate" + "&&" + "python " + googletranslateArguments;
            
            #endregion Option 1: Python script

            #region Option 2: Executable
            /*
            string executable = @"E:\206309_Gann_Kevin\git\Text-To-Text\GoogleTranslate\dist\GoogleTranslate_Script.exe";
            string arguments = $"\"{sourceLanguageCode}\" \"{targetLanguageCode}\" \"{sourceText_Unix}\" \"{outputTextPath_Unix}\"";
            */
            #endregion Option 2: Executable

            /* Process executable */
            ProcessStartInfo processStartInfo = new()
            {
                FileName = executable,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = false,
                RedirectStandardError = true,
            };            
            
            string errors = "";
            using (Process process = Process.Start(processStartInfo)) { errors = process.StandardError.ReadToEnd(); }
            #endregion Process

            #region Outputs
            if (errors != "") throw new Exception(errors);
            else return File.ReadAllText(outputTextPath);
            #endregion Outputs
        }
        #endregion Methods
    }
}
