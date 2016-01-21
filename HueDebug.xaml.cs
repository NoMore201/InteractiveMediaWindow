using System;
using System.Windows;


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
