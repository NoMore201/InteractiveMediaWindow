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

        public Demo()
        {
            InitializeComponent();
            if (DemoReference == null)
                DemoReference = this;
            //Timer timer1 = new Timer( ShowLeftHand, null, 0, 1000);
            //Timer timer = new Timer(ShowRightHand, null, 1001, 1000);
        }

        public void ShowLeftHand()
        {
            Demo.DemoReference.rightHand.Visibility = Visibility.Visible;
            Demo.DemoReference.leftHand.Visibility = Visibility.Hidden;
        }

        public void ShowRightHand(Object o)
        {
            Demo.DemoReference.rightHand.Visibility = Visibility.Hidden;
            Demo.DemoReference.leftHand.Visibility = Visibility.Visible;
        }
    }
}
