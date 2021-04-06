using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace VideoTranslationTool.UserControls
{
    /// <summary>
    /// Interaktionslogik für AudioPlayer.xaml
    /// </summary>
    public partial class AudioPlayer : UserControl
    {
        #region Members
        private string _filePath;
        private readonly MediaPlayer _mediaPlayer = new();
        private bool _userIsDraggingSlider = false;

        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register(nameof(FilePath), typeof(string), typeof(AudioPlayer),
                new PropertyMetadata("", new PropertyChangedCallback(OnFilePathChanged)));
        #endregion Members

        #region Properties
        /// <summary>
        /// Public property <c>FilePath</c> to get / set the file path of the current video
        /// </summary>
        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Public constructor of <c>AudioPlayer</c> class
        /// </summary>
        public AudioPlayer()
        {
            InitializeComponent();

            // Set and start timer
            DispatcherTimer timer = new();
            timer.Interval = TimeSpan.FromSeconds(0.25);
            timer.Tick += UpdateProgressSlider;
            timer.Start();
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Private method <c>OnFilePathChanged</c> reacts to a change of the file
        /// </summary>
        private static void OnFilePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AudioPlayer audioPlayer = d as AudioPlayer;
            audioPlayer.OnFilePathChanged(e);
        }

        /// <summary>
        /// Private method <c>OnFilePathChanged</c> reacts to a change of the file
        /// </summary>
        private void OnFilePathChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                _filePath = e.NewValue.ToString();
                _mediaPlayer.Stop();
                _mediaPlayer.Open(new Uri(_filePath));
            }
        }

        /// <summary>
        /// Private method <c>Pause</c> reacts to the pause button clicked
        /// </summary>
        private void Pause(object sender, RoutedEventArgs e) => _mediaPlayer.Pause();

        /// <summary>
        /// Private method <c>Play</c> reacts to the play button clicked
        /// </summary>
        private void Play(object sender, RoutedEventArgs e) => _mediaPlayer.Play();

        /// <summary>
        /// Private method <c>Slider_DragCompleted</c> reacts to the slider drag ended
        /// </summary>
        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            _mediaPlayer.Position = TimeSpan.FromSeconds(Slider.Value);
            _userIsDraggingSlider = false;
        }

        /// <summary>
        /// Private method <c>Slider_DragStarted</c> reacts to the slider drag started
        /// </summary>
        private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            _userIsDraggingSlider = true;
        }

        /// <summary>
        /// Private method <c>Stop</c> reacts to the stop button clicked
        /// </summary>
        private void Stop(object sender, RoutedEventArgs e) => _mediaPlayer.Stop();

        /// <summary>
        /// Private method <c>UpdateProgressSlider</c> is called every timer tick to update slider display fields
        /// </summary>
        private void UpdateProgressSlider(object sender, EventArgs e)
        {
            // Dont update while slider is in drag state
            if (!_userIsDraggingSlider)
            {
                double value = 0;
                double maximum = 1;
                string timePlayed = "00:00";
                string timeTotal = "00:00";

                if (_mediaPlayer.NaturalDuration.HasTimeSpan)
                {
                    value = _mediaPlayer.Position.TotalSeconds;
                    maximum = _mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                    timePlayed = _mediaPlayer.Position.ToString(@"mm\:ss");
                    timeTotal = _mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
                }

                Slider.Value = value;
                Slider.Maximum = maximum;
                TimePlayedTextbox.Text = timePlayed;
                TimeTotalTextbox.Text = timeTotal;
            }
        }
        #endregion Methods
    }
}
