using System;
using System.Collections.Generic;
using System.IO;
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
    /// Logica di interazione per DemoRelateds.xaml
    /// </summary>
    public partial class DemoRelateds : UserControl
    {
        public List<Model.Product> relateds;
        public int beginTo;

        public DemoRelateds()
        {
            InitializeComponent();
        }

        public DemoRelateds(List<Model.Product> relatedList)
        {
            InitializeComponent();
            relateds=relatedList;
            beginTo = 0;
            FillRelateds();

        }

        public void FillRelateds()
        {
            string absolute_path;
            Uri videoUri;
            BitmapImage cover;

            if (relateds.Count > 0 + beginTo)
            {
                tb1.Text = relateds[0 + beginTo].GetName();
                absolute_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(),
                     "covers\\" + relateds[0 + beginTo].GetCover());
                videoUri = new Uri(absolute_path);
                cover = new BitmapImage(videoUri);
                image.Source = cover;
                firstIm.Opacity = 0.25;
                if(relateds[0 + beginTo].GetTyp() == 3)
                {
                    image1.Height = 380;
                    tba1.Text = relateds[0 + beginTo].music.Artists;
                }
            }
            else
            {
                tb1.Text = "";
                image.Source = null;
                firstIm.Opacity = 0;
                tba1.Text = "";
            }

            if (relateds.Count > 1 + beginTo)
            {
                tb2.Text = relateds[1 + beginTo].GetName();
                absolute_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(),
                     "covers\\" + relateds[1 + beginTo].GetCover());
                videoUri = new Uri(absolute_path);
                cover = new BitmapImage(videoUri);
                image2.Source = cover;
                secondIm.Opacity = 0.25;
                if (relateds[0 + beginTo].GetTyp() == 3)
                {
                    image2.Height = 380;
                    tba2.Text = relateds[1 + beginTo].music.Artists;
                }
            }
            else
            {
                tb2.Text = "";
                image2.Source = null;
                secondIm.Opacity = 0;
                tba2.Text = "";
            }

            if (relateds.Count > 2 + beginTo)
            {
                tb3.Text = relateds[2 + beginTo].GetName();
                absolute_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(),
                     "covers\\" + relateds[2 + beginTo].GetCover());
                videoUri = new Uri(absolute_path);
                cover = new BitmapImage(videoUri);
                image3.Source = cover;
                thirdIm.Opacity = 0.25;
                if (relateds[0 + beginTo].GetTyp() == 3)
                {
                    image3.Height = 380;
                    tba3.Text = relateds[2 + beginTo].music.Artists;
                }
            }
            else
            {
                tb3.Text = "";
                image3.Source = null;
                thirdIm.Opacity = 0;
                tba3.Text = "";
            }

            if (relateds.Count > 3 + beginTo)
            {
                image1.Opacity = 0.25;
            }
            else
            {
                image1.Opacity = 0;
            }

            if (beginTo == 0)
            {
                backw.Opacity = 0;
            }
            else
            {
                backw.Opacity = 0.25;
            }
        }

        public void GoToNextPage()
        {
            beginTo += 3;
            FillRelateds();
        }

        public void BackPage()
        {
            beginTo -= 3;
            FillRelateds();
        }
    }
}
