﻿using System;
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

        public DemoRelateds()
        {
            InitializeComponent();
        }

        public DemoRelateds(List<Model.Product> relateds)
        {
            InitializeComponent();
            FillRelateds(relateds, 0);

        }

        public void FillRelateds(List<Model.Product> relateds, int beginTo)
        {
            string absolute_path;
            Uri videoUri;
            BitmapImage cover;

            if (relateds.Count > 0 + beginTo)
            {
                label.Content = relateds[0].GetName();
                absolute_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(),
                     "covers\\" + relateds[0].GetCover());
                videoUri = new Uri(absolute_path);
                cover = new BitmapImage(videoUri);
                image.Source = cover;
                firstIm.Opacity = 0.25;
            }
            else
            {
                label.Content = "";
                image.Source = null;
                firstIm.Opacity = 0;
            }

            if (relateds.Count > 1 + beginTo)
            {
                label2.Content = relateds[1].GetName();
                absolute_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(),
                     "covers\\" + relateds[1].GetCover());
                videoUri = new Uri(absolute_path);
                cover = new BitmapImage(videoUri);
                image2.Source = cover;
            }
            else
            {
                label2.Content = "";
                image2.Source = null;
                secondIm.Opacity = 0;
            }

            if (relateds.Count > 2 + beginTo)
            {
                label3.Content = relateds[2].GetName();
                absolute_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(),
                     "covers\\" + relateds[2].GetCover());
                videoUri = new Uri(absolute_path);
                cover = new BitmapImage(videoUri);
                image3.Source = cover;
            }
            else
            {
                label3.Content = "";
                image3.Source = null;
                thirdIm.Opacity = 0;
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
    }
}
