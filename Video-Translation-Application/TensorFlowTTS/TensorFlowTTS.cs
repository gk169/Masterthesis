using System.Collections.Generic;

namespace VideoTranslationTool.TextToSpeechModule
{
    public abstract class TensorFlowTTS : TextToSpeech
    {
        private readonly string _synthesizerName;
        private readonly string _vocoderName;


        public TensorFlowTTS(string synthesizerName, string vocoderName) : base(name: nameof(TensorFlowTTS)) { }

        public override string Synthesize(string text, string language, string voice)
        {
            // args:
            // 1=text
            // 2=FeatureGenName
            //3=FeatureGenWeights
            //4=FeatureGenConfig
            //5=VocoderName
            //6=VocoderWeights
            //7=VocoderConfig
            //8=Processor
            //9=AudioOutputFilePath

            ///* Arguments */
            //string sourceLanguageCode = _languageCodeDictionary[sourceLanguage];
            //string targetLanguageCode = _languageCodeDictionary[targetLanguage];
            //string outputTextPath = Path.GetTempPath() + "TranslatedText.txt";

            ///* Transform arguments https://www.btelligent.com/blog/best-practice-arbeiten-in-python-mit-pfaden-teil-1/ */
            //string outputTextPath_Unix = outputTextPath.Replace(@"\", "/");
            //string sourceText_Unix = sourceText.Replace("\r\n", "\n");

            ///* Executable */

            ////// Option 1) Python script
            ////string executable = @"C:\ProgramData\Anaconda3\python.exe";
            ////string script = @"D:\GitRepos\Masterthesis\git\Text-To-Text-Translation\GoogleTranslate_Script.py";
            ////string arguments = $"\"{script}\" \"{sourceLanguageCode}\" \"{targetLanguageCode}\" \"{sourceText_Unix}\" \"{outputTextPath_Unix}\"";

            //// Option 2) Generated executable
            //string executable = @"GoogleTranslate.exe";
            //string arguments = $"\"{sourceLanguageCode}\" \"{targetLanguageCode}\" \"{sourceText_Unix}\" \"{outputTextPath_Unix}\"";

            ///* Process executable */
            //ProcessStartInfo processStartInfo = new()
            //{
            //    FileName = executable,
            //    Arguments = arguments,
            //    UseShellExecute = false,
            //    CreateNoWindow = true,
            //    RedirectStandardError = true,
            //};

            //string errors = "";
            //using (Process process = Process.Start(processStartInfo)) { errors = process.StandardError.ReadToEnd(); }

            ///* Handle errors and output */
            //if (errors != "") throw new Exception(errors);
            //else return File.ReadAllText(outputTextPath);

            throw new System.NotImplementedException();
        }

        protected override Dictionary<string, List<string>> LoadSupportedVoices()
        {
            throw new System.NotImplementedException();
        }
    }
}
