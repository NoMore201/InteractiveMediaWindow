using Q42.HueApi;
using Q42.HueApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    class HueController
    {
        private ILocalHueClient client;
        private string appKey;
        private bool isConnectionAvailable;

        /// <summary>
        /// Base constructor which will search for bridgeIp
        /// </summary>
        public HueController()
        {
            // needs implementation
            isConnectionAvailable = false;
        }

        /// <summary>
        /// If the Bridge ip is known, use this constructor
        /// </summary>
        /// <param name="ip">Ip of the HueBridge (devices with the link button)</param>
        public HueController(string ip)
        {
            client = new LocalHueClient("ip");
            isConnectionAvailable = false;
        }

        public void Connect()
        {
            try
            {
                Task<string> t = client.RegisterAsync("InteractiveMediaWindow", "NoMore");
                appKey = t.Result;
                isConnectionAvailable = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async void SendDoubleColorCommand(string colorBegin, string colorEnd, string num)
        {
            try
            {
                SendColorCommand(colorBegin, num);
                Thread.Sleep(5500);
                SendDoubleColorCommand(colorEnd, colorBegin, num);
            } catch (Exception ex)
            {
                throw ex;
            }
                
        }

        public void SendColorCommand(string color, string num)
        {
            if (isConnectionAvailable)
            {
                var command = new LightCommand();
                command.TurnOn().TransitionTime = TimeSpan.FromSeconds(5);
                command.SetColor(color);
                client.SendCommandAsync(command, new List<string> { num });
            }
            else
            {
                throw new Exception("There is no connection to Hue Bridge");
            }
        }

        public void SendAlert(string num)
        {
            if (isConnectionAvailable)
            {
                var command = new LightCommand();
                command.TurnOn().Alert = Alert.Multiple;
                client.SendCommandAsync(command, new List<string> { num });
            }
            else
            {
                throw new Exception("There is no connection to Hue Bridge");
            }
        }

        public void SendLightness(byte intensity, string num)
        {
            if (isConnectionAvailable)
            {
                var command = new LightCommand();
                command.Brightness = intensity;
                client.SendCommandAsync(command, new List<string> { num });
            }
            else
            {
                throw new Exception("There is no connection to Hue Bridge");
            }
        }

        public void SendAlertColor(string color, string num)
        {
            if (isConnectionAvailable)
            {
                var command = new LightCommand();
                command.TurnOn().Alert = Alert.Multiple;
                command.SetColor(color);
                client.SendCommandAsync(command, new List<string> { num });
            }
            else
            {
                throw new Exception("There is no connection to Hue Bridge");
            }
        }
    }
}
