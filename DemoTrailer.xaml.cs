using Microsoft.Kinect.Wpf.Controls;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    /// <summary>
    /// Logica di interazione per DemoTrailer.xaml
    /// </summary>
    public partial class DemoTrailer : UserControl
    {

        public static string PLAY_ICON = "Images/play.png";
        public static string PAUSE_ICON = "Images/pause.png";

        public DemoTrailer(string videoPath)
        {

            InitializeComponent();
            App app = ((App)Application.Current);
            string absolute_path = Path.Combine(Directory.GetCurrentDirectory(),
                videoPath);
            Uri videoUri = new Uri(absolute_path);
            DataLog.Log(DataLog.DebugLevel.Message,
                "Loader URI for file: " + videoUri.AbsolutePath);
            mediaElement.Source = videoUri;
            play_pause.Visibility = Visibility.Hidden;
            skip.Visibility = Visibility.Hidden;
        }
    }
}
