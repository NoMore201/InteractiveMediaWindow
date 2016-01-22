using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    /// <summary>
    /// Logica di interazione per Demo.xaml
    /// </summary>
    public partial class Demo : Window
    {
        KinectController kc;
        Task hands;
        bool isPointed;
        private HueController hue;

        public Demo()
        {
            InitializeComponent();
            isPointed = false;
            IdleAnimateFirstHand();
            kc = new KinectController();
            hue = new HueController("192.168.0.2");
            hue.Connect();
            kc.bodyReader.FrameArrived += HandleFrame;
        }

        private void HandleFrame(object sender, BodyFrameArrivedEventArgs e)
        {
            kc.Controller_FrameArrived(sender, e);
     
            int zoneP = kc.GetPointedZone();

            if (zoneP!=0 && !isPointed)
            {
                this.label.Content += zoneP.ToString(); ;
                if (zoneP == 1 || zoneP == 3)
                {
                    hue.SendDoubleColorCommand("FF0000", "00FF00", "1");
                    hue.TurnOff("7");
                } else
                {
                    hue.SendAlert("3344FF", "7");
                    hue.TurnOff("1");
                }

                isPointed = true;
            } else if(zoneP==0 && isPointed)
            {
                isPointed = false;
                hue.SendColor("FFFFFF", 1f, (byte)150, "7");
                hue.SendColor("FFFFFF", 1f, (byte)150, "1");
            }
        }

        public void IdleAnimateFirstHand()
        {
            hands = Task.Run(new Action(() =>
            {
                if (!isPointed)
                {
                    HideFirstHand();
                    Thread.Sleep(1000);
                    IdleAnimateSecondHand();
                }
            }));
        }

        public void IdleAnimateSecondHand()
        {
            hands = Task.Run(new Action(() =>
            {
                if (!isPointed)
                {
                    HideSecondHand();
                    Thread.Sleep(1000);
                    IdleAnimateFirstHand();
                }
            }));
        }

        public void HideFirstHand()
        {
            Dispatcher.Invoke(new Action(() => {
                this.leftHand.Visibility = Visibility.Hidden;
                this.rightHand.Visibility = Visibility.Visible;
            }));
        }

        public void HideSecondHand()
        {
            Dispatcher.Invoke(new Action(() => {
                this.rightHand.Visibility = Visibility.Hidden;
                this.leftHand.Visibility = Visibility.Visible;
            }));
        }

    }
}
