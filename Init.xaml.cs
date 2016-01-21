using System.Windows;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    /// <summary>
    /// Logica di interazione per Init.xaml
    /// </summary>
    public partial class Init : Window
    {
        public Init()
        {
            InitializeComponent();
        }

        private void hueClick(object sender, RoutedEventArgs e)
        {
            HueDebug hd = new HueDebug();
            hd.Show();
        }

        private void kinClick(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
        }

        private void demoClick(object sender, RoutedEventArgs e)
        {
            Demo dm = new Demo();
            dm.Show();
        }

        private void startCalibration(object sender, RoutedEventArgs e)
        {
            CalibrationWindow cw = new CalibrationWindow();
            cw.Show();
        }

        private void manageDb(object sender, RoutedEventArgs e)
        {
            DatabaseWindow cw = new DatabaseWindow();
            cw.Show();
        }
    }
}
