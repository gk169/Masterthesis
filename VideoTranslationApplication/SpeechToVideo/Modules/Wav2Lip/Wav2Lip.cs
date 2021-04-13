using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using VideoTranslationTool.FileUtils;

namespace VideoTranslationTool.SpeechToVideoModule
{
    [Export(typeof(SpeechToVideo))]
    /// <summary>
    /// Public class <c>Wav2Lip</c> to generate speech videos with Wav2Lip net
    /// </summary>
    public class Wav2Lip : SpeechToVideo
    {
        #region Constructors
        /// <summary>
        /// Public constructor of <c>Wav2Lip</c> class
        /// </summary>
        public Wav2Lip() : base(name: nameof(Wav2Lip)) { }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// See <see cref="SpeechToVideo.Generate(string, string)"/>
        /// </summary>
        /// <param name="inputAudioPath">
        /// See <see cref="SpeechToVideo.Generate(string, string)"/>
        /// </param>
        /// <param name="inputVideoPath">
        /// See <see cref="SpeechToVideo.Generate(string, string)"/>
        /// </param>
        /// <returns></returns>
        public override string Generate(string inputAudioPath, string inputVideoPath)
        {
            #region Inputs
            // Provide arguments
            string outputVideoPath_Windows = Path.GetTempPath() + "GeneratedVideo.mp4";
            string wav2lipWeightPath = @"E:\206309_Gann_Kevin\weights\Wav2Lip\wav2lip.pth";

            // If file is mp3 -> convert to wav
            if (Path.GetExtension(inputAudioPath) == ".mp3")
            {
                string audioPath_wav = Path.GetTempPath() + "ToTranscribe.wav";
                AudioConverter.Mp3ToWav(inputAudioPath, audioPath_wav);
                inputAudioPath = audioPath_wav;
            }

            // Transform paths https://www.btelligent.com/blog/best-practice-arbeiten-in-python-mit-pfaden-teil-1/
            string inputAudioPath_Unix = inputAudioPath.Replace(@"\", "/");
            string inputVideoPath_Unix = inputVideoPath.Replace(@"\", "/");
            string wav2lipWeightPath_Unix = wav2lipWeightPath.Replace(@"\", "/");
            string outputVideoPath_Unix = outputVideoPath_Windows.Replace(@"\", "/");
            #endregion Inputs

            #region Process
            /* arguments
             * 1: Wav2LipWeightPath
             * 2: InputVideoPath
             * 3: InputAudioPath
             * 4: OutputVideoPath
             */
            #region Option 1: Python script

            string executable = "cmd.exe";

            string script = @"E:\206309_Gann_Kevin\git\Speech-To-Lip\Wav2Lip_Script.py";

            string wav2lipArguments = $"\"{script}\" \"{wav2lipWeightPath_Unix}\" \"{inputVideoPath_Unix}\" \"{inputAudioPath_Unix}\" \"{outputVideoPath_Unix}\"";

            string arguments = "/c " + @"C:\ProgramData\Anaconda3\Scripts\activate.bat" + "&&" + "activate Wav2Lip" + "&&" + "python " + wav2lipArguments;

            #endregion Option 1: Python script

            #region Option 2: Executable

            //string executable = @"D:\GitRepos\Masterthesis\git\Text-To-Text-Translation\dist\Script_GoogleTranslatorApi.exe";
            //string arguments = $"\"{inputAudioPath_Unix}\" \"{inputVideoPath_Unix}\" \"{outputVideoPath_Unix}\"";

            #endregion Option 2: Executable

            // Create process info
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
            // Handle errors and outputs
            if (errors.Contains("Error")) throw new Exception(errors);
            else return outputVideoPath_Windows;
            #endregion Outputs
        }
        #endregion Methods
    }
}
