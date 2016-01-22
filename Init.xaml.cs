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
            mw.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mw.WindowState = WindowState.Maximized;
            mw.Show();
        }

        private void demoClick(object sender, RoutedEventArgs e)
        {
            Demo dm = new Demo();
            dm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            dm.WindowState = WindowState.Maximized;
            dm.Show();
        }

        private void startCalibration(object sender, RoutedEventArgs e)
        {
            CalibrationWindow cw = new CalibrationWindow();
            cw.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            cw.WindowState = WindowState.Maximized;
            cw.Show();
        }

        private void manageDb(object sender, RoutedEventArgs e)
        {
            DatabaseWindow cw = new DatabaseWindow();
            cw.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            cw.WindowState = WindowState.Maximized;
            cw.Show();
        }
    }
}
