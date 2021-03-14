using System.Collections.Generic;

namespace VideoTranslationTool.TextToSpeechModule
{
    public class RTVC : TextToSpeech
    {
        public RTVC() : base(name: nameof(RTVC)) { }

        public override string Synthesize(string text, string language, string voice)
        {
            // args: 1 = AudioSorcePath (wav), 2 = Text, 3 = weightsPath , 4=audiotargetPath (wav)
            throw new System.NotImplementedException();
        }

        protected override Dictionary<string, List<string>> LoadSupportedVoices()
        {
            // Remove previous dictionary
            _weightsPathDictionary = new Dictionary<string, string>();

            // Load weights paths and translations
            // Expected: marianMtPath\<SourceLanguage-TargetLanguage>\
            string marianMtPath = @"D:\GitRepos\Masterthesis\weights\MarianMT";

            string[] folderPaths = Directory.GetDirectories(marianMtPath);

            Dictionary<string, List<string>> supportedTranslations = new();

            foreach (string folder in folderPaths)
            {
                string folderName = new DirectoryInfo(folder).Name;
                string[] folderNameParts = folderName.Split("-");

                string sourceLanguage = folderNameParts[0];
                string targetLanguage = folderNameParts[1];

                _weightsPathDictionary.TryAdd(folderName, folder);

                // Try to add (source language - list of target languages) as new entry to dictionary
                if (!supportedTranslations.TryAdd(sourceLanguage, new List<string>() { targetLanguage }))
                {
                    // if it fails -> add target language to list of target languages
                    supportedTranslations[sourceLanguage].Add(targetLanguage);
                }
            }

            return supportedTranslations;



            throw new System.NotImplementedException();
        }
    }
}
