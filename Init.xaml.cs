using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
    }
}
