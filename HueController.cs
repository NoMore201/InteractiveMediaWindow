﻿using Q42.HueApi;
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
            client = new LocalHueClient(ip);
            isConnectionAvailable = false;
        }

        public async void Connect()
        {
            try
            {
                appKey = await client.RegisterAsync("InteractiveMediaW", "NoMore");
                isConnectionAvailable = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task SendDoubleColorCommand(string colorBegin, string colorEnd, string num)
        {
            try
            {
                SendColor(colorBegin, 5, num);
                Thread.Sleep(5500);
                await SendDoubleColorCommand(colorEnd, colorBegin, num);
            } catch (Exception ex)
            {
                throw ex;
            }
                
        }

        public void SendColor(string color, double transitionTime, string num)
        {
            if (isConnectionAvailable)
            {
                var command = new LightCommand();
                command.TurnOn().TransitionTime = TimeSpan.FromSeconds(transitionTime);
                command.SetColor(color);
                client.SendCommandAsync(command, new List<string> { num });
            }
            else
            {
                throw new Exception("There is no connection to Hue Bridge");
            }
        }

        public void SendColor(string color, double transitionTime, byte brightness, string num)
        {
            if (isConnectionAvailable)
            {
                var command = new LightCommand();
                command.TurnOn().TransitionTime = TimeSpan.FromSeconds(transitionTime);
                command.SetColor(color);
                command.Brightness = brightness;
                client.SendCommandAsync(command, new List<string> { num });
            }
            else
            {
                throw new Exception("There is no connection to Hue Bridge");
            }
        }

        /// <summary>
        /// Sends alert command with default color
        /// </summary>
        /// <param name="num">Hue to send command to</param>
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

        /// <summary>
        /// Turns OFF the HUE
        /// </summary>
        /// <param name="num">Hue to send command to</param>
        public void TurnOff(string num)
        {
            if (isConnectionAvailable)
            {
                var command = new LightCommand();
                command.TurnOff();
                client.SendCommandAsync(command, new List<string> { num });
            }
            else
            {
                throw new Exception("There is no connection to Hue Bridge");
            }
        }

        /// <summary>
        /// Sends alert command
        /// </summary>
        /// <param name="color">Color of the alert</param>
        /// <param name="num">Hue to send command to</param>
        public void SendAlert(string color, string num)
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
