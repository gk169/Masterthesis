using System.Collections.Generic;

namespace User_Interface
{
    public class Pyttsx3 : TextToSpeech
    {
        public Pyttsx3() : base(name: nameof(Pyttsx3))
        {
        }

        protected override Dictionary<string, List<string>> LoadSupportedVoices()
        {
            // TODO load dynamically
            return new Dictionary<string, List<string>>()
            {
                {"de", new List<string>(){"Hans", "Lisa"}},
                {"en", new List<string>(){"John", "Megan"}},
            };
        }
    }
}
