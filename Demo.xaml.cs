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

        public Demo()
        {
            InitializeComponent();
            isPointed = false;
            Example();
            kc = new KinectController();
            kc.bodyReader.FrameArrived += HandleFrame;
        }

        private void HandleFrame(object sender, BodyFrameArrivedEventArgs e)
        {
            kc.Controller_FrameArrived(sender, e);

            if (kc.Arm != ArmPointing.Nothing)
            {
                isPointed = true;
                
                int zoneP = kc.GetPointedZone();
                if (zoneP == 1 || zoneP == 3)
                {
                    this.leftHand.Visibility = Visibility.Visible;
                    this.rightHand.Visibility = Visibility.Hidden;
                    DataLog.ToConsole(zoneP.ToString() + " " + kc.Arm);
                } else
                {
                    this.leftHand.Visibility = Visibility.Hidden;
                    this.rightHand.Visibility = Visibility.Visible;
                    DataLog.ToConsole(zoneP.ToString() + " " + kc.Arm);
                }
            } else if (isPointed)
            {
                //Example();
                isPointed = false;
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        public void Example()
        {
            hands = Task.Run(new Action(() =>
            {
                if (!isPointed)
                {
                    HideFirstHand();
                    Thread.Sleep(1000);
                    Example2();
                }
            }));
        }

        public void Example2()
        {
            hands = Task.Run(new Action(() =>
            {
                if (!isPointed)
                {
                    HideSecondHand();
                    Thread.Sleep(1000);
                    Example();
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
