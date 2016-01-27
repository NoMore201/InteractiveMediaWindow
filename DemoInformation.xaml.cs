﻿using Microsoft.Samples.Kinect.BodyBasics.Model;
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
    /// Logica di interazione per DemoInformation.xaml
    /// </summary>
    public partial class DemoInformation : UserControl
    {
        void InitDemoInformation(Music prod)
        {
            InitializeComponent();
            string absolute_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(),
                "covers\\" + prod.Cover);
            Uri videoUri = new Uri(absolute_path);
            BitmapImage cover = new BitmapImage(videoUri);
            image.Source = cover;
            album_title.Content = prod.Name;
            artist.Content = prod.Artists;
            
        }

        void InitDemoInformation(Movie prod)
        {
            InitializeComponent();
            string absolute_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(),
                "covers\\" + prod.Cover);
            Uri videoUri = new Uri(absolute_path);
            BitmapImage cover = new BitmapImage(videoUri);
            image.Source = cover;
            album_title.Content = prod.Cover;
            artist.Content = prod.Director;

        }

        void InitDemoInformation(Book prod)
        {
            InitializeComponent(); 
            string absolute_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(),
                 "covers\\" + prod.Cover);
            Uri videoUri = new Uri(absolute_path);
            BitmapImage cover = new BitmapImage(videoUri);
            image.Source = cover;
            album_title.Content = prod.Cover;
            artist.Content = prod.Writers;

        }

        public DemoInformation(Product prod)
        {
            if (prod.movie != null)
                InitDemoInformation(prod.movie);
            else if (prod.music != null)
                InitDemoInformation(prod.music);
            else if (prod.book != null)
                InitDemoInformation(prod.book);
        }
    }
}
