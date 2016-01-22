using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Kinect;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

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
        private DemoIdle idle;
        private DemoTrailer trailer;

        private List<Model.Movie> movies;
        private List<Model.Book> books;
        private List<Model.Music> musics;
        private List<Model.Tracklist> tracklists;

        private SortedList<int, Model.Product> windowProducts;

        public Demo()
        {
            InitializeComponent();
            isPointed = false;
            IdleAnimateFirstHand();
            kc = new KinectController();
            hue = new HueController("192.168.0.2");
            hue.Connect();
            kc.bodyReader.FrameArrived += HandleFrame;
            idle = new DemoIdle();
            this.contentControl.Content = idle;

            ReadFile();
        }

        private void HandleFrame(object sender, BodyFrameArrivedEventArgs e)
        {
            kc.Controller_FrameArrived(sender, e);
     
            int zoneP = kc.GetPointedZone();

            if (zoneP!=0 && !isPointed)
            {
                idle.label.Content += zoneP.ToString(); ;
                if (zoneP == 1 || zoneP == 3)
                {
                    Model.LightColors colors = windowProducts[windowProducts.IndexOfKey(1)].GetColors();
                    hue.SendDoubleColorCommand(colors.color1, colors.color2, "1");
                    hue.TurnOff("7");
                    StartTrailer(windowProducts[windowProducts.IndexOfKey(1)].GetTrailer());
                } else
                {
                    Model.LightColors colors = windowProducts[windowProducts.IndexOfKey(2)].GetColors();
                    hue.SendDoubleColorCommand(colors.color1, colors.color2, "1");
                    hue.SendAlert("3344FF", "7");
                    hue.TurnOff("1");
                    StartTrailer(windowProducts[windowProducts.IndexOfKey(2)].GetTrailer());
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
                idle.leftHand.Visibility = Visibility.Hidden;
                idle.rightHand.Visibility = Visibility.Visible;
            }));
        }

        public void HideSecondHand()
        {
            Dispatcher.Invoke(new Action(() => {
                idle.rightHand.Visibility = Visibility.Hidden;
                idle.leftHand.Visibility = Visibility.Visible;
            }));
        }

        public void StartTrailer(string url)
        {
                isPointed = true;
                trailer = new DemoTrailer("trailers\\" + url);
                contentControl.Content = trailer;
                trailer.PlayTrailer();
        }

        private void ReadFile()
        {
            string data_movie = File.ReadAllText(DatabaseWindow.MOVIE_FILE);
            string data_book = File.ReadAllText(DatabaseWindow.BOOK_FILE);
            string data_music = File.ReadAllText(DatabaseWindow.MUSIC_FILE);
            string data_tracks = File.ReadAllText(DatabaseWindow.TRACK_FILE);
            movies = JsonConvert.DeserializeObject<List<Model.Movie>>(data_movie);
            books = JsonConvert.DeserializeObject<List<Model.Book>>(data_book);
            musics = JsonConvert.DeserializeObject<List<Model.Music>>(data_music);
            tracklists = JsonConvert.DeserializeObject<List<Model.Tracklist>>(data_tracks);

            foreach (Model.Movie movie in movies)
                if (movie.Position != 0)
                    windowProducts.Add(movie.Position, new Model.Product(movie));

            foreach (Model.Music movie in musics)
                if (movie.Position != 0)
                    windowProducts.Add(movie.Position, new Model.Product(movie));

            foreach (Model.Book movie in books)
                if(movie.Position!=0)
                    windowProducts.Add(movie.Position, new Model.Product(movie));
        }

    }
}
