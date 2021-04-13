using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace VideoTranslationTool.UserControls
{
    /// <summary>
    /// Interaktionslogik für VideoPlayer.xaml
    /// </summary>
    public partial class VideoPlayer : UserControl
    {
        #region Members
        private bool _userIsDraggingSlider = false;

        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register(nameof(FilePath), typeof(string), typeof(VideoPlayer),
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
        /// Public constructor of <c>VideoPlayer</c> class
        /// </summary>
        public VideoPlayer()
        {
            InitializeComponent();

            // Set DataContext to be able to bind to local FilePath property
            VideoPlayerGrid.DataContext = this;

            // Set and start timer
            DispatcherTimer timer = new();
            timer.Interval = TimeSpan.FromSeconds(0.25);
            timer.Tick += UpdateProgressSlider;
            timer.Start();
            Player.MediaOpened += Media_Opened;
            Slider.Minimum = 0;
            Slider.Maximum = 1;
            TimeTotalTextbox.Text = "00:00";
        }

        private void Media_Opened(object sender, RoutedEventArgs e)
        {
            Slider.Maximum = Player.NaturalDuration.TimeSpan.TotalSeconds;
            TimeTotalTextbox.Text = Player.NaturalDuration.TimeSpan.ToString(@"mm\:ss");            
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Private method <c>OnFilePathChanged</c> reacts to a change of the file
        /// </summary>
        private static void OnFilePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VideoPlayer videoPlayer = d as VideoPlayer;
            videoPlayer.OnFilePathChanged(e);
        }

        /// <summary>
        /// Private method <c>OnFilePathChanged</c> reacts to a change of the file
        /// </summary>
        private void OnFilePathChanged(DependencyPropertyChangedEventArgs e)
        {
            Player.Stop();
            Player.Close();

            if (e.NewValue is not null and not "")
            {
                Player.Source = new Uri(e.NewValue.ToString());
                Player.Play();
                Player.Stop();
            }
        }

        /// <summary>
        /// Private method <c>Pause</c> reacts to the pause button clicked
        /// </summary>
        private void Pause(object sender, RoutedEventArgs e) => Player.Pause();

        /// <summary>
        /// Private method <c>Play</c> reacts to the play button clicked
        /// </summary>
        private void Play(object sender, RoutedEventArgs e) => Player.Play();

        /// <summary>
        /// Private method <c>Slider_DragCompleted</c> reacts to the slider drag ended
        /// </summary>
        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Player.Position = TimeSpan.FromSeconds(Slider.Value);
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
        private void Stop(object sender, RoutedEventArgs e) => Player.Stop();

        /// <summary>
        /// Private method <c>UpdateProgressSlider</c> is called every timer tick to update slider display fields
        /// </summary>
        private void UpdateProgressSlider(object sender, EventArgs e)
        {
            // Dont update while slider is in drag state
            if (!_userIsDraggingSlider)
            {
                double value = 0;                
                string timePlayed = "00:00";                

                if (Player.NaturalDuration.HasTimeSpan)
                {
                    value = Player.Position.TotalSeconds;                    
                    timePlayed = Player.Position.ToString(@"mm\:ss");                    
                }

                Slider.Value = value;                
                TimePlayedTextbox.Text = timePlayed;                
            }
        }
        #endregion Methods
    }
}
