using System.Collections.Generic;

namespace User_Interface
{
    public class GoogleTTS : TextToSpeech
    {
        public GoogleTTS() : base(name: nameof(GoogleTTS))
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
