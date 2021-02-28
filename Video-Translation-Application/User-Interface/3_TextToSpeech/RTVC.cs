using System.Collections.Generic;

namespace User_Interface
{
    public class RTVC : TextToSpeech
    {
        public RTVC() : base(name: nameof(RTVC))
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
