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
            string marianMtPath = @"E:\206309_Gann_Kevin\weights\MarianMT";

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
            #region Inputs
            string weightsPath = _weightsPathDictionary[$"{sourceLanguage}-{targetLanguage}"];
            string outputTextPath = Path.GetTempPath() + "TranslatedText.txt";
            string inputTextPath = Path.GetTempPath() + "ToTranslateText.txt";

            // Transform arguments https://www.btelligent.com/blog/best-practice-arbeiten-in-python-mit-pfaden-teil-1/
            string outputTextPath_Unix = outputTextPath.Replace(@"\", "/");
            string weightsPath_Unix = weightsPath.Replace(@"\", "/");

            string sourceText_Unix = sourceText.Replace("\r\n", "\n");
            string inputTextPath_Unix = inputTextPath.Replace(@"\", "/");

            File.WriteAllText(inputTextPath, sourceText_Unix);
            #endregion Inputs

            #region Process
            #region Option 1: Python script

            string executable = "cmd.exe";

            string script = @"E:\206309_Gann_Kevin\git\Text-To-Text\MarianMT\MarianMT_Script.py";
            string marianmtArguments = $"\"{script}\" \"{weightsPath_Unix}\" \"{inputTextPath_Unix}\" \"{outputTextPath_Unix}\"";

            string arguments = "/c " + @"C:\ProgramData\Anaconda3\Scripts\activate.bat" + "&&" + "activate MarianMT" + "&&" + "python " + marianmtArguments;
            
            #endregion Option 1: Python script

            #region Option 2: Executable
            /*
            string executable = @"E:\206309_Gann_Kevin\git\Text-To-Text\MarianMT\dist\MarianMT_Script.exe";
            string arguments = $"\"{weightsPath_Unix}\" \"{sourceText_Unix}\" \"{outputTextPath_Unix}\"";
            */
            #endregion Option 2: Executable

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
            if (errors.Contains("Error")) throw new Exception(errors);
            else return File.ReadAllText(outputTextPath);
            #endregion Outputs
        }
        #endregion Methods
    }
}
