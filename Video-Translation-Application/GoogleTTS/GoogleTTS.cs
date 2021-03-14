using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;

namespace VideoTranslationTool.TextToSpeechModule
{
    [Export(typeof(TextToSpeech))]
    /// <summary>
    /// Public class <c>GoogleTTS</c> for Google-TTS-API (GoogleTranslate Hack)
    /// </summary>
    public class GoogleTTS : TextToSpeech
    {
        #region Members
        private Dictionary<string, string> _languageCodeDictionary; // Language - LanguageCode
        #endregion Members

        #region Constructors
        /// <summary>
        /// Constructor of <c>GoogleTTS</c> class
        /// </summary>
        public GoogleTTS() : base(name: nameof(GoogleTTS)) { }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// See <see cref="TextToSpeech.LoadSupportedVoices"/>
        /// </summary>
        /// <returns>
        /// See <see cref="TextToSpeech.LoadSupportedVoices"/>
        /// </returns>
        protected override Dictionary<string, List<string>> LoadSupportedVoices()
        {
            string filePath = @"GoogleTTS_SupportedLanguages.txt"; // due to the fact the python tts uses google translate no voice or country can be selected

            // Read Supported Languages from file
            string file = File.ReadAllText(filePath);

            // Split after each new line
            string[] lines = file.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            // Discard first and second entry (SourceUrl \n Language - LanguageCode)
            lines = lines[2..lines.Length];

            // Clear previous entries
            _languageCodeDictionary = new Dictionary<string, string>();
            Dictionary<string, List<string>> supportedVoices = new();

            // Split line in language ([0]), language code ([2])
            foreach (string line in lines)
            {
                string[] languageParts = line.Split(" \t", StringSplitOptions.RemoveEmptyEntries);

                string language = languageParts[0];
                string languageCode = languageParts[1];

                // If language entry is new -> generate default voice entry
                if (_languageCodeDictionary.TryAdd(language, languageCode))
                {
                    string voiceName = "Default";
                    string voiceGender = "Unknown";
                    string voice = $"{voiceName} ({voiceGender})";
                    supportedVoices.TryAdd(language, new List<string>() { voice });
                }
            }

            return supportedVoices;
        }

        /// <summary>
        /// See <see cref="TextToSpeech.Synthesize(string, string, string)"/>
        /// </summary>
        /// <param name="text">
        /// See <see cref="TextToSpeech.Synthesize(string, string, string)"/>
        /// </param>
        /// <param name="language">
        /// See <see cref="TextToSpeech.Synthesize(string, string, string)"/>
        /// </param>
        /// <param name="voice">
        /// See <see cref="TextToSpeech.Synthesize(string, string, string)"/>
        /// </param>
        /// <returns>
        /// See <see cref="TextToSpeech.Synthesize(string, string, string)"/>
        /// </returns>
        public override string Synthesize(string text, string language, string voice)
        {
            /* Arguments */
            string languageCode = _languageCodeDictionary[language];
            string outputAudioPath = Path.GetTempPath() + "SynthesizedAudio.mp3";

            /* Transform arguments https://www.btelligent.com/blog/best-practice-arbeiten-in-python-mit-pfaden-teil-1/ */
            string outputAudioPath_Unix = outputAudioPath.Replace(@"\", "/");
            string text_Unix = text.Replace("\r\n", "\n");

            /* Executable */
            // Option 1) Python script
            string executable = @"C:\ProgramData\Anaconda3\envs\TTS\python.exe";
            string script = @"D:\GitRepos\Masterthesis\git\Text-To-Speech\GoogleTTS_Script.py";
            string arguments = $"\"{script}\" \"{text_Unix}\" \"{languageCode}\" \"{outputAudioPath_Unix}\"";

            //// Option 2) Generated executable
            //string executable = @"GoogleTTS.exe";
            //string arguments = $"\"{text}\" \"{languageCode}\" \"{outputAudioPath_Unix}\"";

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
            else return outputAudioPath;
        }
        #endregion Methods
    }
}
