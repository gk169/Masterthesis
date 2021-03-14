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
        private readonly MediaPlayer _mediaPlayer = new();
        private string _filePath;
        private bool _userIsDraggingSlider = false;

        public static readonly DependencyProperty FilePathProperty = 
            DependencyProperty.Register(nameof(FilePath), typeof(string), typeof(AudioPlayer),
                new PropertyMetadata("", new PropertyChangedCallback(OnFilePathChanged)));

        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set
            {
                SetValue(FilePathProperty, value);
                //_filePath = value;
                //_mediaPlayer.Stop();
                //_mediaPlayer.Open(new Uri(_filePath));
            }
        }

        private static void OnFilePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AudioPlayer audioPlayer = d as AudioPlayer;
            audioPlayer.OnFilePathChanged(e);
        }

        private void OnFilePathChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                _filePath = e.NewValue.ToString();
                _mediaPlayer.Stop();
                _mediaPlayer.Open(new Uri(_filePath));
            }
        }

        public AudioPlayer()
        {
            InitializeComponent();

            DispatcherTimer timer = new();
            timer.Interval = TimeSpan.FromSeconds(0.25);
            timer.Tick += UpdateProgressSlider;
            timer.Start();
        }

        private void UpdateProgressSlider(object sender, EventArgs e)
        {
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

        private void Play(object sender, RoutedEventArgs e) => _mediaPlayer.Play();

        private void Pause(object sender, RoutedEventArgs e) => _mediaPlayer.Pause();

        private void Stop(object sender, RoutedEventArgs e) => _mediaPlayer.Stop();

        private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            _userIsDraggingSlider = true;
        }

        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            _mediaPlayer.Position = TimeSpan.FromSeconds(Slider.Value);
            _userIsDraggingSlider = false;
        }
    }
}
