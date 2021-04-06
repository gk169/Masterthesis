﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using VideoTranslationTool.FileUtils;

namespace VideoTranslationTool.TextToSpeechModule
{
    [Export(typeof(TextToSpeech))]
    /// <summary>
    /// Public class <c>RTVC</c> for RealTimeVoiceCloning (https://github.com/CorentinJ/Real-Time-Voice-Cloning)
    /// </summary>
    public class RTVC : TextToSpeech
    {
        #region Members
        private Dictionary<string, string> _encoderPathDictionary;          // "Language" - EncoderPath
        private Dictionary<string, string> _synthesizerPathDictionary;      // "Language" - SynthesizerPath
        private Dictionary<string, string> _vocoderPathDictionary;          // "Language" - VocoderPath
        private Dictionary<string, string> _voiceAudioFilePathDictionary;   // "Language + Voice" - VoicePath
        #endregion Members

        #region Constructors
        /// <summary>
        /// Constructor of class <c>RTVC</c>
        /// </summary>
        public RTVC() : base(name: nameof(RTVC)) { }
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
            // Remove previous dictionaries
            _encoderPathDictionary = new Dictionary<string, string>();
            _synthesizerPathDictionary = new Dictionary<string, string>();
            _vocoderPathDictionary = new Dictionary<string, string>();
            _voiceAudioFilePathDictionary = new Dictionary<string, string>();

            Dictionary<string, List<string>> supportedVoices = new();

            // Load weights paths,languages and voices
            // Expected: rtvcPath\<Language>\encoder.pt ..synthesizer.pt vocoder.pt <VoiceA (Gender)>.wav <VoiceB (Gender)>.mp3
            string rtvcPath = @"E:\206309_Gann_Kevin\weights\RTVC";

            string[] folderPaths = Directory.GetDirectories(rtvcPath);

            foreach (string folder in folderPaths)
            {
                string language = new DirectoryInfo(folder).Name;

                string[] filePaths = Directory.GetFiles(folder);

                string encoderPath = "";
                string synthesizerPath = "";
                string vocoderPath = "";

                Dictionary<string, string> folderVoicePaths = new(); // "Language + Voice" - VoicePath
                List<string> folderVoices = new(); // List of voices

                foreach (string file in filePaths)
                {
                    // Get folders encoder, synthesizer and vocoder
                    switch (Path.GetFileName(file))
                    {
                        case "encoder.pt": encoderPath = file; break;
                        case "synthesizer.pt": synthesizerPath = file; break;
                        case "vocoder.pt": vocoderPath = file; break;
                    }

                    // Get folders voices
                    if (Path.GetExtension(file) is /* ".mp3" or*/ ".wav")
                    {
                        string voice = Path.GetFileNameWithoutExtension(file);
                        if (folderVoicePaths.TryAdd($"{language} + {voice}", file)) folderVoices.Add(voice);
                    }
                }

                // if language is valid -> add encoder, synthesizer, vocoder and voices
                if (encoderPath != "" && synthesizerPath != "" && vocoderPath != "")
                {
                    _encoderPathDictionary.TryAdd(language, encoderPath);
                    _synthesizerPathDictionary.TryAdd(language, synthesizerPath);
                    _vocoderPathDictionary.TryAdd(language, vocoderPath);

                    foreach (var voicePath in folderVoicePaths)
                    {
                        _voiceAudioFilePathDictionary.TryAdd(voicePath.Key, voicePath.Value);
                    }

                    supportedVoices.TryAdd(language, folderVoices);
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
        /// <returns></returns>
        public override string Synthesize(string text, string language, string voice)
        {
            #region Inputs
            string audioSourcePath = _voiceAudioFilePathDictionary[$"{language} + {voice}"];

            // If file is mp3 -> convert to wav
            if (Path.GetExtension(audioSourcePath) == ".mp3")
            {
                string audioPath_wav = Path.GetTempPath() + "ToTranscribe.wav";
                AudioConverter.Mp3ToWav(audioSourcePath, audioPath_wav);
                audioSourcePath = audioPath_wav;
            }

            string encoderPath = _encoderPathDictionary[language];
            string synthesizerPath = _synthesizerPathDictionary[language];
            string vocoderPath = _vocoderPathDictionary[language];

            string outputAudioPath = Path.GetTempPath() + "SynthesizedAudio.wav";

            /* Transform arguments https://www.btelligent.com/blog/best-practice-arbeiten-in-python-mit-pfaden-teil-1/ */
            string inputAudioPath_Unix = audioSourcePath.Replace(@"\", "/");
            string outputAudioPath_Unix = outputAudioPath.Replace(@"\", "/");
            string encoderPath_Unix = encoderPath.Replace(@"\", "/");
            string synthesizerPath_Unix = synthesizerPath.Replace(@"\", "/");
            string vocoderPath_Unix = vocoderPath.Replace(@"\", "/");

            string sourceText_Unix = text.Replace("\r\n", "\n");
            #endregion Inputs

            #region Process
            #region Option 1: Python script

            string executable = "cmd.exe";
            string script = @"E:\206309_Gann_Kevin\git\Text-To-Speech\RTVC\RTVC_Script.py";
            string rtvcArguments = $"\"{script}\" \"{inputAudioPath_Unix}\" \"{sourceText_Unix}\" \"{encoderPath_Unix}\" " +
                               $"\"{synthesizerPath_Unix}\" \"{vocoderPath_Unix}\" \"{outputAudioPath_Unix}\"";

            string arguments = "/c " + @"C:\ProgramData\Anaconda3\Scripts\activate.bat" + "&&" + "activate RTVC" + "&&" + "python " + rtvcArguments;

            #endregion Option 1: Python script

            #region Option 2: Executable - not working yet!
            /*
            string executable = @"RTVC.exe";
            string arguments = $"\"{inputAudioPath_Unix}\" \"{sourceText_Unix}\" \"{encoderPath_Unix}\" " +
                               $"\"{synthesizerPath_Unix}\" \"{vocoderPath_Unix}\" \"{outputAudioPath_Unix}\"";
            */
            #endregion Option 2: Executable

            /* Process executable */
            ProcessStartInfo processStartInfo = new()
            {
                FileName = executable,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = false,//TODO
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