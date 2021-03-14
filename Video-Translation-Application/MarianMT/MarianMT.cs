using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;

namespace VideoTranslationTool.TextToTextModule
{
    [Export(typeof(TextToText))]
    /// <summary>
    /// Public class <c>MarianMT</c> for MarianMT translation net
    /// </summary>
    public class MarianMT : TextToText
    {
        #region Members
        private Dictionary<string, string> _weightsPathDictionary; // "SourceLanguage-TargetLanguage" - weights path
        #endregion Members

        #region Constructors
        /// <summary>
        /// Constructor of class <c>MarianMT</c>
        /// </summary>
        public MarianMT() : base(name: nameof(MarianMT)) { }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// See <see cref="TextToText.LoadSupportedTranslations"/>
        /// </summary>
        /// <returns>
        /// See <see cref="TextToText.LoadSupportedTranslations"/>
        /// </returns>
        protected override Dictionary<string, List<string>> LoadSupportedTranslations()
        {
            // Remove previous dictionary
            _weightsPathDictionary = new Dictionary<string, string>();

            // Load weights paths and translations
            // Expected: marianMtPath\<SourceLanguage-TargetLanguage>\
            string marianMtPath = @"D:\GitRepos\Masterthesis\weights\MarianMT";

            string[] folderPaths = Directory.GetDirectories(marianMtPath);

            Dictionary<string, List<string>> supportedTranslations = new();

            foreach (string folder in folderPaths)
            {
                string folderName = new DirectoryInfo(folder).Name;
                string[] folderNameParts = folderName.Split("-");

                string sourceLanguage = folderNameParts[0];
                string targetLanguage = folderNameParts[1];

                _weightsPathDictionary.TryAdd(folderName, folder);

                // Try to add (source language - list of target languages) as new entry to dictionary
                if (!supportedTranslations.TryAdd(sourceLanguage, new List<string>() { targetLanguage }))
                {
                    // if it fails -> add target language to list of target languages
                    supportedTranslations[sourceLanguage].Add(targetLanguage);
                }
            }

            return supportedTranslations;
        }

        /// <summary>
        /// See <see cref="TextToText.Translate(string, string, string)"/>
        /// </summary>
        /// <param name="sourceText">
        /// See <see cref="TextToText.Translate(string, string, string)"/>
        /// </param>
        /// <param name="sourceLanguage">
        /// See <see cref="TextToText.Translate(string, string, string)"/>
        /// </param>
        /// <param name="targetLanguage">
        /// See <see cref="TextToText.Translate(string, string, string)"/>
        /// </param>
        /// <returns>
        /// See <see cref="TextToText.Translate(string, string, string)"/>
        /// </returns>
        public override string Translate(string sourceText, string sourceLanguage, string targetLanguage)
        {
            /* Arguments */
            string weightsPath = _weightsPathDictionary[$"{sourceLanguage}-{targetLanguage}"];
            string outputTextPath = Path.GetTempPath() + "TranslatedText.txt";

            /* Transform arguments https://www.btelligent.com/blog/best-practice-arbeiten-in-python-mit-pfaden-teil-1/ */
            string outputTextPath_Unix = outputTextPath.Replace(@"\", "/");
            string weightsPath_Unix = weightsPath.Replace(@"\", "/");

            string sourceText_Unix = sourceText.Replace("\r\n", "\n");

            /* Executable */

            //// Option 1) Python script
            string executable = @"C:\ProgramData\Anaconda3\envs\MarianMT\python.exe";
            string script = @"D:\GitRepos\Masterthesis\git\Text-To-Text\MarianMT_Script.py";
            string arguments = $"\"{script}\" \"{weightsPath_Unix}\" \"{sourceText_Unix}\" \"{outputTextPath_Unix}\"";

            // Option 2) Generated executable
            //string executable = @"MarianMT.exe";
            //string arguments = $"\"{weightsPath_Unix}\" \"{sourceText_Unix}\" \"{outputTextPath_Unix}\"";

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
