using Microsoft.Samples.Kinect.BodyBasics.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    /// <summary>
    /// Logica di interazione per DemoInformation.xaml
    /// </summary>
    public partial class DemoInformation : UserControl
    {
        public DemoInformation(Music prod)
        {
            InitializeComponent();
            BitmapImage cover = new BitmapImage(new Uri("covers\\" + prod.Cover, UriKind.Relative));
            image.Source = cover;
            album_title.Content = prod.Cover;
            artist.Content = prod.Artists;
            
        }
    }
}
