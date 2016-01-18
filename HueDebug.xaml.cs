using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Logica di interazione per HueDebug.xaml
    /// </summary>
    public partial class HueDebug : Window
    {
        public static HueDebug HueReference;

        private HueController hueControl;

        public HueDebug()
        {
            InitializeComponent();
            if (HueReference == null)
            {
                HueReference = this;
            }
            hueControl = null;
        }

        private void testButton(object sender, RoutedEventArgs e)
        {
            hueControl.SendDoubleColorCommand("FFFFFFF", "FF3333", "1");
        }

        private void connectButton(object sender, RoutedEventArgs e)
        {
            if (hueControl == null)
            {
                hueControl = new HueController("192.168.0.2");
            }
            try
            {
                hueControl.Connect();
            } catch (Exception ex)
            {
                this.textBlock.Text += ex.Message + "\n";
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            hueControl.SendColor("12FF34", 4, "1");
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            hueControl.SendAlert("23FF43", "1");
        }
    }
}
