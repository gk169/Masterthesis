using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;

namespace VideoTranslationTool.TextToSpeechModule
{
    [Export(typeof(TextToSpeech))]
    /// <summary>
    /// Public class <c>PyTTSx3</c> for python tts module
    /// </summary>
    public class PyTTSx3 : TextToSpeech
    {
        #region Members
        private Dictionary<string, string> _voiceIdDictionary; // "voiceId-voiceName (voiceGender)" - "voiceID"
        #endregion Members

        #region Constructors
        /// <summary>
        /// Constructor for <c>PyTTSx3</c> class
        /// </summary>
        public PyTTSx3() : base(name: nameof(PyTTSx3)) { }
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
            string filePath = @"PyTTSx3_SupportedVoices.txt";

            // Read Supported Languages from file
            string file = File.ReadAllText(filePath);

            // Split after each new line
            string[] lines = file.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            // Discard first and second entry (SourceUrl \n Language - Name - Gender - ID)
            lines = lines[2..lines.Length];

            // Clear previous entries
            _voiceIdDictionary = new Dictionary<string, string>();
            Dictionary<string, List<string>> supportedVoices = new();

            // Split line in language ([0]), voiceName ([1]), voiceGender ([2]) and voiceId ([3])
            foreach (string line in lines)
            {
                string[] lineParts = line.Split(" \t", StringSplitOptions.RemoveEmptyEntries);

                string language = lineParts[0];
                string voiceName = lineParts[1];
                string voiceGender = lineParts[2];
                string voiceId = lineParts[3];

                string voice = $"{voiceId}-{voiceName} ({voiceGender})";

                _voiceIdDictionary.TryAdd(voice, voiceId);

                // Try to add (language - list of voices) as new entry to dictionary
                if (!supportedVoices.TryAdd(language, new List<string>() { voice }))
                {
                    // if it fails -> add voice to list of voices
                    supportedVoices[language].Add(voice);
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
            string outputAudioPath = Path.GetTempPath() + "SynthesizedAudio.mp3";
            string voiceId = _voiceIdDictionary[voice];

            /* Transform arguments https://www.btelligent.com/blog/best-practice-arbeiten-in-python-mit-pfaden-teil-1/ */
            string outputAudioPath_Unix = outputAudioPath.Replace(@"\", "/");
            string text_Unix = text.Replace("\r\n", "\n");

            /* Executable */
            // Option 1) Python script
            string executable = @"C:\ProgramData\Anaconda3\envs\TTS\python.exe";
            string script = @"D:\GitRepos\Masterthesis\git\Text-To-Speech\PyTTSx3_Script.py";
            string arguments = $"\"{script}\" \"{text_Unix}\" \"{voiceId}\" \"{outputAudioPath_Unix}\"";

            //// Option 2) Generated executable
            //string executable = @"PyTTSx3.exe";
            //string arguments = $"\"{text_Unix}\" \"{voiceId}\" \"{outputAudioPath_Unix}\"";

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
