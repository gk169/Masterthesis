using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using VideoTranslationTool.FileUtils;

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
            #region Inputs
            string outputTextPath = Path.GetTempPath() + "TranscribedText.txt";
            string audioLanguageCode = _languageCodeDictionary[audioLanguage];

            // If file is mp3 -> convert to wav
            if (Path.GetExtension(inputAudioPath) == ".mp3")
            {
                string audioPath_wav = Path.GetTempPath() + "ToTranscribe.wav";
                AudioConverter.Mp3ToWav(inputAudioPath, audioPath_wav);
                inputAudioPath = audioPath_wav;
            }
            else if (Path.GetExtension(inputAudioPath) is ".wav") // Make sure that wav pcm is used
            {
                string audioPath_wav = Path.GetTempPath() + "ToTranscribe.wav";
                AudioConverter.WavToWavPcm(inputAudioPath, audioPath_wav);
                inputAudioPath = audioPath_wav;
            }

            // Transform paths https://www.btelligent.com/blog/best-practice-arbeiten-in-python-mit-pfaden-teil-1/
            string inputAudioPath_Unix = inputAudioPath.Replace(@"\", "/");
            string outputTextPath_Unix = outputTextPath.Replace(@"\", "/");
            #endregion Inputs

            #region Process
            #region Option 1: Python script
            
            string executable = "cmd.exe";

            string script = @"E:\206309_Gann_Kevin\git\Speech-To-Text\GoogleSTT\GoogleSTT_Script.py";
            string googlesttArguments = $"\"{script}\" \"{inputAudioPath_Unix}\" \"{audioLanguageCode}\" \"{outputTextPath_Unix}\"";

            string arguments = "/c " + @"C:\ProgramData\Anaconda3\Scripts\activate.bat" + "&&" + "activate GoogleSTT" + "&&" + "python " + googlesttArguments;
            
            #endregion Option 1: Python script

            #region Option 2: Executable
            /*
            string executable = @"E:\206309_Gann_Kevin\git\Speech-To-Text\GoogleSTT\dist\GoogleSTT_Script.exe";
            string arguments = $"\"{inputAudioPath_Unix}\" \"{audioLanguageCode}\" \"{outputTextPath_Unix}\"";
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

            // Execute process and get output
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
