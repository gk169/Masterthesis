using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace VideoTranslationTool.SpeechToTextModule
{
    [Export(typeof(SpeechToText))]
    /// <summary>
    /// Public class <c>GoogleSTT</c> to transcribe audio with Google STT API
    /// </summary>
    public class GoogleSTT : SpeechToText
    {
        #region Members
        private Dictionary<string, string> _languageCodeDictionary; // Language - LanguageCode
        #endregion Members

        #region Constructors
        /// <summary>
        /// Constructor of class <c>GoogleSTT</c>
        /// </summary>
        public GoogleSTT() : base(name: nameof(GoogleSTT)) { }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// See <see cref="SpeechToText.LoadSupportedAudioLanguages"/>
        /// </summary>
        /// <returns>
        /// See <see cref="SpeechToText.LoadSupportedAudioLanguages"/>
        /// </returns>
        protected override List<string> LoadSupportedAudioLanguages()
        {
            string filePath = @"GoogleSTT_SupportedLanguages.txt";

            // Read Supported Languages from file
            string file = File.ReadAllText(filePath);

            // Split after each new line
            string[] lines = file.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            // Discard first and second entry (SourceUrl \n Language - ISO6391Code)
            lines = lines[2..lines.Length];

            _languageCodeDictionary = new Dictionary<string, string>();

            // Split line in language ([0]) and language code ([1])
            foreach (string line in lines)
            {
                string[] languageParts = line.Split(" \t", StringSplitOptions.RemoveEmptyEntries);
                _languageCodeDictionary.TryAdd(languageParts[0], languageParts[1]);
            }

            // Get all languages
            return _languageCodeDictionary.Keys.ToList();
        }

        /// <summary>
        /// See <see cref="SpeechToText.Transcribe(string, string)"/>
        /// </summary>
        /// <param name="inputAudioPath">
        /// See <see cref="SpeechToText.Transcribe(string, string)"/>
        /// </param>
        /// <param name="audioLanguage">
        /// See <see cref="SpeechToText.Transcribe(string, string)"/>
        /// </param>
        /// <returns>
        /// See <see cref="SpeechToText.Transcribe(string, string)"/>
        /// </returns>
        public override string Transcribe(string inputAudioPath, string audioLanguage)
        {
            // Provide arguments
            string outputTextPath = Path.GetTempPath() + "TranscribedText.txt";
            string audioLanguageCode = _languageCodeDictionary[audioLanguage];

            // Transform paths https://www.btelligent.com/blog/best-practice-arbeiten-in-python-mit-pfaden-teil-1/
            string inputAudioPath_Unix = inputAudioPath.Replace(@"\", "/");
            string outputTextPath_Unix = outputTextPath.Replace(@"\", "/");

            // Provide executable

            // Test only
            string executable = @"C:\ProgramData\Anaconda3\envs\SpeechRecognition\python.exe";
            string script = @"D:\GitRepos\Masterthesis\git\Speech-To-Text\SpeechRecognition\GoogleSTT_Script.py";
            string arguments = $"\"{script}\" \"{inputAudioPath_Unix}\" \"{audioLanguageCode}\" \"{outputTextPath_Unix}\"";

            // Final solution
            //string executable = @"D:\GitRepos\Masterthesis\git\Text-To-Text-Translation\dist\Script_GoogleTranslatorApi.exe";
            //string arguments = $"\"{inputAudioPath_Unix}\" \"{inputVideoPath_Unix}\" \"{outputVideoPath_Unix}\"";

            // Create process info
            ProcessStartInfo processStartInfo = new()
            {
                FileName = executable,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
            };

            // Execute process and get output
            string errors = "";
            using (Process process = Process.Start(processStartInfo)) { errors = process.StandardError.ReadToEnd(); }

            // Handle errors and outputs
            if (errors != "") throw new Exception(errors);
            else return File.ReadAllText(outputTextPath);
        }
        #endregion Methods
    }
}
