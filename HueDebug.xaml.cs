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
        public static HueDebug hueReference;
        public ILocalHueClient client;
        public string appKey;

        public HueDebug()
        {
            InitializeComponent();
            if (hueReference == null)
            {
                hueReference = this;
            }
        }

        private void testButton(object sender, RoutedEventArgs e)
        {
            var command = new LightCommand();
            command.TurnOn().SetColor("D64937");
            command.Alert = Alert.Once;
            client.SendCommandAsync(command, new List<string> { "1" });
        }

        private void connectButton(object sender, RoutedEventArgs e)
        {
            initClient();
        }

        private async Task initClient()
        {
            try {
                client = new LocalHueClient("192.168.0.2:80");
                appKey = await client.RegisterAsync("asd", "lol");
                HueDebug.hueReference.textBlock.Text += "SUCCESFULLY CONNECTED" + "\n";
            } catch (Exception ex)
            {
                HueDebug.hueReference.textBlock.Text += ex.Message + "\n";
            }
        }
    }
}
