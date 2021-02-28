namespace User_Interface
{
    public abstract class SpeechToVideo
    {
        private readonly string _name;

        // audio
        // video

        protected SpeechToVideo(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
