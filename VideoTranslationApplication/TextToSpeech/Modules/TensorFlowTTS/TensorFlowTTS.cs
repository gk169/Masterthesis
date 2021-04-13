using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace VideoTranslationTool.TextToSpeechModule
{
    /// <summary>
    /// Abstract class <c>TensorFlowTTS</c> as parent class for all TTS modules included in TensorFlowTTS
    /// </summary>
    public abstract class TensorFlowTTS : TextToSpeech
    {
        #region Members        
        private Dictionary<string, string> _processorPathDictionary;                                // Language - ProcessorPath
        private Dictionary<string, Dictionary<string, string>> _synthesizerConfigPathDictionary;    // Language - Voice - SynthesizerConfigPath
        private Dictionary<string, Dictionary<string, string>> _synthesizerWeightPathDictionary;    // Language - Voice - SynthesizerWeightPath
        private Dictionary<string, Dictionary<string, string>> _vocoderConfigPathDictionary;        // Language - Voice - VocoderConfigPath
        private Dictionary<string, Dictionary<string, string>> _vocoderWeightPathDictionary;        // Language - Voice - VocoderWeightPath
        #endregion Members

        #region Properties        
        /// <summary>
        /// Property to get the name of the synthesizer
        /// </summary>
        protected abstract string SynthesizerName { get; }
        
        /// <summary>
        /// Property to get the name of the vocoder
        /// </summary>
        protected abstract string VocoderName { get; }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Constructor of <c>TensorFlowTTS</c> class
        /// </summary>
        /// <param name="name">
        /// See <see cref="TextToSpeech.TextToSpeech(string)"/>
        /// </param>
        public TensorFlowTTS(string name) : base(name: name) { }
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
            _processorPathDictionary = new Dictionary<string, string>();
            _synthesizerWeightPathDictionary = new Dictionary<string, Dictionary<string, string>>();
            _synthesizerConfigPathDictionary = new Dictionary<string, Dictionary<string, string>>();
            _vocoderWeightPathDictionary = new Dictionary<string, Dictionary<string, string>>();
            _vocoderConfigPathDictionary = new Dictionary<string, Dictionary<string, string>>();

            // Check processor path
            string processorDirectory = @"E:\206309_Gann_Kevin\weights\TensorFlowTTS\Processor";

            foreach (string folder in Directory.GetDirectories(processorDirectory))
            {
                string language = new DirectoryInfo(folder).Name;

                string processorPath = "";

                string[] filePaths = Directory.GetFiles(folder);

                foreach (string file in filePaths)
                {
                    if (Path.GetExtension(file) is ".json") processorPath = file;
                }

                _processorPathDictionary.TryAdd(language, processorPath);
            }

            // Check vocoder path
            string vocoderDirectory = @"E:\206309_Gann_Kevin\weights\TensorFlowTTS\" + VocoderName;

            foreach (string vocoderLanguageFolder in Directory.GetDirectories(vocoderDirectory))
            {
                string language = new DirectoryInfo(vocoderLanguageFolder).Name;

                foreach (string vocoderVoiceFolder in Directory.GetDirectories(vocoderLanguageFolder))
                {
                    string voiceName = new DirectoryInfo(vocoderVoiceFolder).Name;
                    string vocoderWeightPath = "";
                    string vocoderConfigPath = "";

                    foreach (string file in Directory.GetFiles(vocoderVoiceFolder))
                    {
                        if (Path.GetExtension(file) is ".h5") vocoderWeightPath = file;
                        if (Path.GetExtension(file) is ".yaml") vocoderConfigPath = file;
                    }

                    if (vocoderWeightPath is not "" && vocoderConfigPath is not "")
                    {
                        if (_vocoderWeightPathDictionary.ContainsKey(language)) _vocoderWeightPathDictionary[language].TryAdd(voiceName, vocoderWeightPath);
                        else _vocoderWeightPathDictionary.TryAdd(language, new Dictionary<string, string>() { { voiceName, vocoderWeightPath } });

                        if (_vocoderConfigPathDictionary.ContainsKey(language)) _vocoderConfigPathDictionary[language].TryAdd(voiceName, vocoderWeightPath);
                        else _vocoderConfigPathDictionary.TryAdd(language, new Dictionary<string, string>() { { voiceName, vocoderConfigPath } });
                    }
                }
            }

            // Check synthesizer path
            string synthesizerDirectory = @"E:\206309_Gann_Kevin\weights\TensorFlowTTS\" + SynthesizerName;

            foreach (string synthesizerLanguageFolder in Directory.GetDirectories(synthesizerDirectory))
            {
                string language = new DirectoryInfo(synthesizerLanguageFolder).Name;

                foreach (string synthesizerVoiceFolder in Directory.GetDirectories(synthesizerLanguageFolder))
                {
                    string voiceName = new DirectoryInfo(synthesizerVoiceFolder).Name;
                    string synthesizerWeightPath = "";
                    string synthesizerConfigPath = "";

                    foreach (string file in Directory.GetFiles(synthesizerVoiceFolder))
                    {
                        if (Path.GetExtension(file) is ".h5") synthesizerWeightPath = file;
                        if (Path.GetExtension(file) is ".yaml") synthesizerConfigPath = file;
                    }

                    if (synthesizerWeightPath is not "" && synthesizerConfigPath is not "")
                    {
                        if (_synthesizerWeightPathDictionary.ContainsKey(language)) _synthesizerWeightPathDictionary[language].TryAdd(voiceName, synthesizerWeightPath);
                        else _synthesizerWeightPathDictionary.TryAdd(language, new Dictionary<string, string>() { { voiceName, synthesizerWeightPath } });

                        if (_synthesizerConfigPathDictionary.ContainsKey(language)) _synthesizerConfigPathDictionary[language].TryAdd(voiceName, synthesizerConfigPath);
                        else _synthesizerConfigPathDictionary.TryAdd(language, new Dictionary<string, string>() { { voiceName, synthesizerConfigPath } });
                    }
                }
            }

            // Check if vocoder and synthesizer are present
            Dictionary<string, List<string>> supportedVoices = new Dictionary<string, List<string>>();

            foreach (string language in _processorPathDictionary.Keys)
            {
                if (_synthesizerWeightPathDictionary.ContainsKey(language)
                    && _synthesizerConfigPathDictionary.ContainsKey(language)
                    && _vocoderWeightPathDictionary.ContainsKey(language)
                    && _vocoderConfigPathDictionary.ContainsKey(language))
                {
                    foreach (string voiceName in _synthesizerWeightPathDictionary[language].Keys)
                    {
                        if (_synthesizerConfigPathDictionary[language].ContainsKey(voiceName)
                            && _vocoderWeightPathDictionary[language].ContainsKey(voiceName)
                            && _vocoderConfigPathDictionary[language].ContainsKey(voiceName))
                        {
                            if (supportedVoices.ContainsKey(language)) supportedVoices[language].Add(voiceName);
                            else supportedVoices.TryAdd(language, new List<string>() { voiceName });
                        }

                    }

                }
            }

            // Return supported voices
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
            #region Inputs
            string processorPath = _processorPathDictionary[language];

            string synthesizerWeightPath = _synthesizerWeightPathDictionary[language][voice];
            string synthesizerConfigPath = _synthesizerConfigPathDictionary[language][voice];

            string vocoderWeightPath = _vocoderWeightPathDictionary[language][voice];
            string vocoderConfigPath = _vocoderConfigPathDictionary[language][voice];

            string outputAudioPath = Path.GetTempPath() + "SynthesizedAudio.wav";

            /* Transform arguments (win paths to unix paths) https://www.btelligent.com/blog/best-practice-arbeiten-in-python-mit-pfaden-teil-1/ */
            processorPath = processorPath.Replace(@"\", "/");
            synthesizerWeightPath = synthesizerWeightPath.Replace(@"\", "/");
            synthesizerConfigPath = synthesizerConfigPath.Replace(@"\", "/");
            vocoderWeightPath = vocoderWeightPath.Replace(@"\", "/");
            vocoderConfigPath = vocoderConfigPath.Replace(@"\", "/");
            text = text.Replace("\r\n", "\n");

            string inputTextPath = Path.GetTempPath() + "ToTranslateText.txt";
            string inputTextPath_Unix = inputTextPath.Replace(@"\", "/");

            File.WriteAllText(inputTextPath, text);
            #endregion Inputs

            #region Process
            /* arguements:
             * 1: text
             * 2: SynthesizerName
             * 3: SynthesizerWeightPath
             * 4: SynthesizerConfigPath
             * 5: VocoderName
             * 6: VocoderWeightPath
             * 7: VocoderConfigPath
             * 8: ProcessorPath
             * 9: AudioOutputFilePath
             */
            #region Option 1: Python script

            string executable = "cmd.exe";

            string script = @"E:\206309_Gann_Kevin\git\Text-To-Speech\TensorFlowTTS\TensorFlowTTS_Script.py";
            string tensorflowttsArguments = $"\"{script}\" \"{inputTextPath_Unix}\" \"{SynthesizerName}\" \"{synthesizerWeightPath}\" \"{synthesizerConfigPath}\" \"{VocoderName}\" \"{vocoderWeightPath}\" \"{vocoderConfigPath}\" \"{processorPath}\" \"{outputAudioPath}\"";

            string arguments = "/c " + @"C:\ProgramData\Anaconda3\Scripts\activate.bat" + "&&" + "activate TensorFlowTTS" + "&&" + "python " + tensorflowttsArguments;
            // NOTE: requires another check for errors!
            
            #endregion Option 1: Python script

            #region Option 2: Executable
            /*
            string executable = @"E:\206309_Gann_Kevin\git\Text-To-Speech\TensorFlowTTS\dist\TensorFlowTTS_Script\TensorFlowTTS_Script.exe";
            string arguments = $"\"{text}\" \"{SynthesizerName}\" \"{synthesizerWeightPath}\" \"{synthesizerConfigPath}\" \"{VocoderName}\" \"{vocoderWeightPath}\" \"{vocoderConfigPath}\" \"{processorPath}\" \"{outputAudioPath}\"";
            // NOTE: requires another check for errors!
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
            if (errors != "") throw new Exception(errors);              // error check script
            //if (errors.Contains("Error")) throw new Exception(errors);  // check if exe is used
            else return outputAudioPath;
            #endregion Outputs
        }
        #endregion Methods
    }
}
