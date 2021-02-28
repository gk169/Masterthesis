using System.Collections.Generic;

namespace User_Interface
{
    public class DeepSpeech : SpeechToText
    {
        public DeepSpeech() : base(nameof(DeepSpeech))
        { }

        protected override List<string> LoadSupportedLanguages()
        {
            //TODO load dynamically
            return new List<string>()
            {
                "de","en",
            };
        }
    }
}
