using System.Collections.Generic;

namespace User_Interface
{
    public class GoogleTranslate : TextToText
    {
        public GoogleTranslate() : base(name: nameof(GoogleTranslate))
        {
        }

        protected override Dictionary<string, List<string>> LoadSupportedLanguages()
        {
            // TODO load dynamically
            return new Dictionary<string, List<string>>()
            {
                {"de", new List<string>(){"en"} },
                {"en", new List<string>(){"de"} },
            };
        }
    }
}
