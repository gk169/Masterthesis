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
        private Dictionary<string, Dictionary<string, string>> _voiceIdDictionary; // "Language - VoiceNames - IDs"
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
            #region Inputs
            string supportedLanguagesFilePath = Path.GetTempPath() + "PyTTSx3_SupportedVoices.txt";

            /* Transform arguments (win paths to unix paths) https://www.btelligent.com/blog/best-practice-arbeiten-in-python-mit-pfaden-teil-1/ */
            string supportedLanguagesFilePath_Unix = supportedLanguagesFilePath.Replace(@"\", "/");
            #endregion Inputs

            #region Process
            #region Option 1: Python script
            
            string executable = "cmd.exe";

            string script = @"E:\206309_Gann_Kevin\git\Text-To-Speech\PyTTSx3\GetVoices\PyTTSx3_GetVoices.py";
            string pyttsx3GetVoicesArguments = $"\"{script}\" \"{supportedLanguagesFilePath_Unix}\"";

            string arguments = "/c " + @"C:\ProgramData\Anaconda3\Scripts\activate.bat" + "&&" + "activate PyTTSx3" + "&&" + "python " + pyttsx3GetVoicesArguments;
            
            #endregion Option 1: Python script

            #region Option 2: Executable
            /*
            string executable = @"E:\206309_Gann_Kevin\git\Text-To-Speech\PyTTSx3\GetVoices\dist\PyTTSx3_GetVoices\PyTTSx3_GetVoices.exe";
            string arguments = $"\"{supportedLanguagesFilePath_Unix}\"";
            */
            #endregion Option 2: Executable            

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
            #endregion Process

            #region Outputs
            if (errors != "") throw new Exception(errors);
            
            string supportedLanguageFile = File.ReadAllText(supportedLanguagesFilePath);

            _voiceIdDictionary = new Dictionary<string, Dictionary<string, string>>(); // Remove old dictionary

            foreach (string line in supportedLanguageFile.Split("\r\n", StringSplitOptions.RemoveEmptyEntries))
            {
                string[] lineParts = line.Split(" \t");
                string name = lineParts[0];
                string id = lineParts[1];
                string language = lineParts[2].Split("(")[0];

                if (_voiceIdDictionary.ContainsKey(language)) _voiceIdDictionary[language].TryAdd(name, id); // If language exists -> try to add new name
                else _voiceIdDictionary.TryAdd(language, new Dictionary<string, string>() { { name, id } }); // If language doenst exist -> try to add new language entry
            }

            Dictionary<string, List<string>> supportedVoices = new Dictionary<string, List<string>>();
            foreach (string language in _voiceIdDictionary.Keys)
            {
                List<string > voiceNames = new List<string>();
                foreach (string voiceName in _voiceIdDictionary[language].Keys) voiceNames.Add(voiceName);
                supportedVoices.TryAdd(language, voiceNames);
            }

            return supportedVoices;
            #endregion Output
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
            #region Inputs
            /* Arguments
             * 1: Text
             * 2: VoiceID
             * 3: OutputAudioPath
             */

            string outputAudioPath = Path.GetTempPath() + "SynthesizedAudio.wav";
            string voiceId = _voiceIdDictionary[language][voice];

            /* Transform arguments https://www.btelligent.com/blog/best-practice-arbeiten-in-python-mit-pfaden-teil-1/ */
            string outputAudioPath_Unix = outputAudioPath.Replace(@"\", "/");
            text = text.Replace("\r\n", "\n");

            string inputTextPath = Path.GetTempPath() + "ToTranslateText.txt";
            string inputTextPath_Unix = inputTextPath.Replace(@"\", "/");

            File.WriteAllText(inputTextPath, text);
            #endregion Inputs

            #region Process
            #region Option 1: Python script

            string executable = "cmd.exe";
            string script = @"E:\206309_Gann_Kevin\git\Text-To-Speech\PyTTSx3\PyTTSx3_Script.py";

            string pyttsx3Arguments = $"\"{script}\" \"{inputTextPath_Unix}\" \"{voiceId}\" \"{outputAudioPath_Unix}\"";

            string arguments = "/c " + @"C:\ProgramData\Anaconda3\Scripts\activate.bat" + "&&" + "activate PyTTSx3" + "&&" + "python " + pyttsx3Arguments;
            
            #endregion Option 1: Python script

            #region Option 2: Executable
            /*
            string executable = @"E:\206309_Gann_Kevin\git\Text-To-Speech\PyTTSx3\dist\PyTTSx3_Script\PyTTSx3_Script.exe";
            string arguments = $"\"{text}\" \"{voiceId}\" \"{outputAudioPath_Unix}\"";
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
            /* Handle errors and output */
            if (errors != "") throw new Exception(errors);
            else return outputAudioPath;
            #endregion Outputs
        }
        #endregion Methods
    }
}
