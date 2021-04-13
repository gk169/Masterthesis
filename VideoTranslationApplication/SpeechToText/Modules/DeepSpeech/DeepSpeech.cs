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
    /// Public class <c>DeepSpeech</c> to transcribe audio with DeepSpeech net
    /// </summary>
    public class DeepSpeech : SpeechToText
    {
        #region Members
        private Dictionary<string, string> _scorerPathDictionary;   // Language - Scorer path
        private Dictionary<string, string> _weightsPathDictionary;  // Language - Weights path
        #endregion Members

        #region Constructors
        /// <summary>
        /// Constructor of class <c>DeepSpeech</c>
        /// </summary>
        public DeepSpeech() : base(nameof(DeepSpeech)) { }
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
            // Reset old values
            _weightsPathDictionary = new Dictionary<string, string>();
            _scorerPathDictionary = new Dictionary<string, string>();

            // Load new weights, scorers and languages from directory
            // Expected: deepSpeechDataPath/<Language (Country)/<...>.scorer and <...>.pbmm
            string deepSpeechDataPath = @"E:\206309_Gann_Kevin\weights\DeepSpeech";

            string[] folderPaths = Directory.GetDirectories(deepSpeechDataPath);

            foreach (string folder in folderPaths)
            {
                string[] filePaths = Directory.GetFiles(folder);

                string weightsPath = "";
                string scorerPath = "";

                foreach (string file in filePaths)
                {
                    if (Path.GetExtension(file) == ".scorer") scorerPath = file;
                    if (Path.GetExtension(file) == ".pbmm") weightsPath = file;
                }

                if (weightsPath != "" && scorerPath != "")
                {
                    string language = new DirectoryInfo(folder).Name;
                    _weightsPathDictionary.TryAdd(language, weightsPath);
                    _scorerPathDictionary.TryAdd(language, scorerPath);
                }
            }

            return _weightsPathDictionary.Keys.ToList();
        }

        /// <summary>
        /// See <see cref="SpeechToText.Transcribe(string, string)"/>
        /// </summary>
        /// <param name="audioPath">
        /// See <see cref="SpeechToText.Transcribe(string, string)""/>
        /// </param>
        /// <param name="audioLanguage">
        /// See <see cref="SpeechToText.Transcribe(string, string)""/>
        /// </param>
        /// <returns>
        /// See <see cref="SpeechToText.Transcribe(string, string)""/>
        /// </returns>
        public override string Transcribe(string audioPath, string audioLanguage)
        {
            #region Inputs
            // If file is mp3 -> convert to wav
            if (Path.GetExtension(audioPath) == ".mp3")
            {
                string audioPath_wav = Path.GetTempPath() + "ToTranscribe.wav";
                AudioConverter.Mp3ToWav(audioPath, audioPath_wav);
                audioPath = audioPath_wav;
            }

            // Provide arguments
            string outputTextPath = Path.GetTempPath() + "TranscribedText.txt";

            string weightsPath = _weightsPathDictionary[audioLanguage];
            string scorerPath = _scorerPathDictionary[audioLanguage];

            // Transform paths https://www.btelligent.com/blog/best-practice-arbeiten-in-python-mit-pfaden-teil-1/
            string inputAudioPath_Unix = audioPath.Replace(@"\", "/");
            string outputTextPath_Unix = outputTextPath.Replace(@"\", "/");
            string weightsPath_Unix = weightsPath.Replace(@"\", "/");
            string scorerPath_Unix = scorerPath.Replace(@"\", "/");
            #endregion Inputs

            #region Process
            #region Option 1: Python script
            
            string executable = "cmd.exe";

            string script = @"E:\206309_Gann_Kevin\git\Speech-To-Text\DeepSpeech\DeepSpeech_Script.py";
            string deepspeechArguments = $"\"{script}\" \"{weightsPath_Unix}\" \"{scorerPath_Unix}\" \"{inputAudioPath_Unix}\" \"{outputTextPath_Unix}\"";

            string arguments = "/c " + @"C:\ProgramData\Anaconda3\Scripts\activate.bat" + "&&" + "activate DeepSpeech" + "&&" + "python " + deepspeechArguments;
            
            #endregion Option 1: Python script

            #region Option 2: Executable - not working yet!
            /*
            string executable = @"E:\206309_Gann_Kevin\git\Speech-To-Text\DeepSpeech\dist\DeepSpeech_Script.exe";
            string arguments = $"\"{weightsPath_Unix}\" \"{scorerPath_Unix}\" \"{inputAudioPath_Unix}\" \"{outputTextPath_Unix}\"";
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
            if (errors.Contains("Error")) throw new Exception(errors);
            else return File.ReadAllText(outputTextPath);
            #endregion Outputs
        }
        #endregion Methods
    }
}
