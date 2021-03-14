using System;
using System.Diagnostics;
using System.IO;

namespace VideoTranslationTool.SpeechToVideoModule
{
    public class Wav2Lip : SpeechToVideo
    {
        public Wav2Lip() : base(name: nameof(Wav2Lip))
        {
        }

        public override string Generate(string inputAudioPath, string inputVideoPath)
        {
            // Provide arguments
            string outputVideoPath_Windows = Path.GetTempPath() + "GeneratedVideo.mp4";

            // Transform paths https://www.btelligent.com/blog/best-practice-arbeiten-in-python-mit-pfaden-teil-1/
            string inputAudioPath_Unix = inputAudioPath.Replace(@"\", "/");
            string inputVideoPath_Unix = inputVideoPath.Replace(@"\", "/");
            string outputVideoPath_Unix = outputVideoPath_Windows.Replace(@"\", "/");

            // Provide executable

            string executable = @"C:\ProgramData\Anaconda3\python.exe"; // Note: if env is used, the env python has to be here!
            //string executable = @"C:\ProgramData\Anaconda3\envs\TTS\python.exe";
            string script = @"D:\GitRepos\Masterthesis\git\Text-To-Text-Translation\Script_GoogleTranslatorApi.py";
            string arguments = $"\"{script}\" \"{inputAudioPath_Unix}\" \"{inputVideoPath_Unix}\" \"{outputVideoPath_Unix}\"";

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
            else return outputVideoPath_Windows;
        }
    }
}
