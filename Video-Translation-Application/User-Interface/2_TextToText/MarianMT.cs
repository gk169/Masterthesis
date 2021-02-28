using System.Collections.Generic;

namespace User_Interface
{
    public class MarianMT : TextToText
    {
        public MarianMT() : base(name: nameof(MarianMT))
        {
        }

        protected override Dictionary<string, List<string>> LoadSupportedLanguages()
        {
            // TODO load dynamically
            return new Dictionary<string, List<string>>()
            {
                {"de", new List<string>(){"en", "fr"} },
                {"en", new List<string>(){"de", "es"} },
            };
        }
    }
}
