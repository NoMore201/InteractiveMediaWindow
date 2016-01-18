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
    /// Logica di interazione per Demo.xaml
    /// </summary>
    public partial class Demo : Window
    {
        public static Demo DemoReference;
        bool pointed;

        public Demo()
        {
            InitializeComponent();
            if (DemoReference == null)
                DemoReference = this;
            //Timer timer1 = new Timer( ShowLeftHand, DemoReference, 0, 1000);
            //Timer timer = new Timer(ShowRightHand, this, 1001, 1000);
            Example();
        }

        public void ShowLeftHand(Object o)
        {
            Demo demo = (Demo) o ;
            demo.rightHand.Visibility = Visibility.Visible;
            demo.leftHand.Visibility = Visibility.Hidden;
        }

        public void ShowRightHand(Object o)
        {
            Demo demo = (Demo)o;
            demo.rightHand.Visibility = Visibility.Hidden;
            demo.leftHand.Visibility = Visibility.Visible;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        public void Example()
        {
            Task.Run(new Action(() =>
            {
                HideFirstHand();
                Thread.Sleep(5000);
                HideSecondHand();
            }));
        }

        public void HideFirstHand()
        {
            Dispatcher.Invoke(new Action(() => { this.leftHand.Visibility = Visibility.Hidden; }));
        }

        public void HideSecondHand()
        {
            Dispatcher.Invoke(new Action(() => { this.rightHand.Visibility = Visibility.Hidden; }));
        }

    }
}
