using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Kinect;
using Microsoft.Kinect.Input;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Kinect.Wpf.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    /// <summary>
    /// Logica di interazione per Demo.xaml
    /// </summary>
    public partial class Demo : Window
    {

        public static int MAX_FRAMES_PAUSE = 40;
        public static string FIRST_HUE = "3";
        public static string SECOND_HUE = "4";

        KinectController kc;
        Task hands;
        bool isPointed;
        private HueController hue;

        private DemoIdle idle;
        private DemoTrailer trailer;
        private DemoInformation information;

        private KinectCoreWindow kWin;

        private DbFileManager db;

        private SortedList<int, Model.Product> windowProducts;

        private bool paused;
        private bool checkedButton;

        private int counterFrames;
        private int pointerID;

        private uint mainHandId;

        BitmapImage playimage;
        BitmapImage pauseimage;

        public Demo()
        {
            InitializeComponent();

            // variable initialization
            isPointed = false;
            checkedButton = false;
            paused = false;
            counterFrames = 0;
            mainHandId = 0;

            // object initialization
            IdleAnimateFirstHand();
            kc = new KinectController();
            hue = new HueController("192.168.0.2");
            hue.Connect();
            kc.bodyReader.FrameArrived += HandleFrame;
            idle = new DemoIdle();
            this.contentControl.Content = idle;
            KinectRegion.SetKinectRegion(this, kinectRegion);

            App app = ((App)Application.Current);
            app.KinectRegion = kinectRegion;

            windowProducts = new SortedList<int, Model.Product>();
            db = new DbFileManager();

            playimage = new BitmapImage(new Uri("Images\\play.png", UriKind.Relative));
            pauseimage = new BitmapImage(new Uri("Images\\pause.png", UriKind.Relative));

            InitProducts();
        }

        private void KWin_PointerMoved(object sender, KinectPointerEventArgs e)
        {       
            if (mainHandId == 0 && e.CurrentPoint.Properties.IsPrimary &&
                e.CurrentPoint.Properties.IsEngaged)
            {
                mainHandId = e.CurrentPoint.PointerId;
                ShowButtonsOnTrailer();
            }

            if (e.CurrentPoint.PointerId == mainHandId)
            {
                CheckPointInButton(e.CurrentPoint.Position.X, e.CurrentPoint.Position.Y);
            }
            if (e.CurrentPoint.PointerId == mainHandId && !e.CurrentPoint.Properties.IsEngaged)
            {
                mainHandId = 0;
                HideButtonsOnTrailer();
            }
        }

        private void HandleFrame(object sender, BodyFrameArrivedEventArgs e)
        {
            kc.Controller_FrameArrived(sender, e);

            int zoneP = kc.GetPointedZone();

            if (zoneP != 0 && !isPointed)
            {
                DataLog.Log(DataLog.DebugLevel.Message, zoneP.ToString());
                if (zoneP == 1 || zoneP == 3)
                {
                    Model.LightColors colors = windowProducts[1].GetColors();
                    hue.SendDoubleColorCommand(colors.color1, colors.color2, FIRST_HUE);
                    hue.TurnOff(SECOND_HUE);
                    StartTrailer(windowProducts[1].GetTrailer());
                    kc.bodyReader.FrameArrived += null;
                } else
                {
                    Model.LightColors colors = windowProducts[2].GetColors();
                    hue.SendDoubleColorCommand(colors.color1, colors.color2, SECOND_HUE);
                    hue.TurnOff(FIRST_HUE);
                    StartTrailer(windowProducts[2].GetTrailer());
                    kc.bodyReader.FrameArrived += null;
                }

                isPointed = true;
            } else if (zoneP == 0 && isPointed)
            {
                isPointed = false;
            } 
        }

        private void IdleAnimateFirstHand()
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

        private void IdleAnimateSecondHand()
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

        private void HideFirstHand()
        {
            Dispatcher.Invoke(new Action(() => {
                idle.leftHand.Visibility = Visibility.Hidden;
                idle.rightHand.Visibility = Visibility.Visible;
            }));
        }

        private void HideSecondHand()
        {
            Dispatcher.Invoke(new Action(() => {
                idle.rightHand.Visibility = Visibility.Hidden;
                idle.leftHand.Visibility = Visibility.Visible;
            }));
        }

        private void StartTrailer(string url)
        {
            isPointed = true;
            trailer = new DemoTrailer("trailers\\" + url);
            contentControl.Content = trailer;
            trailer.mediaElement.Play();
            trailer.mediaElement.MediaEnded += MediaElement_MediaEnded;
            kWin = KinectCoreWindow.GetForCurrentThread();
            kWin.PointerMoved += KWin_PointerMoved;
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            information = new DemoInformation();
            contentControl.Content = information;
            information.button1.Click += InformationBackButton;
        }

        private void InformationBackButton(object sender, RoutedEventArgs e)
        {
            contentControl.Content = idle;
            kc.bodyReader.FrameArrived += HandleFrame;
        }

        private void CheckPointInButton(float X, float Y)
        {
            X = X * (float) this.Width;
            Y = Y * (float) this.Height;

            if (X > trailer.play_pause.Margin.Left-50 && X < trailer.play_pause.Margin.Left + trailer.play_pause.Width + 50 &&
                    Y > ((this.Height / 2) - (trailer.play_pause.Height / 2) - 50) && Y < ((this.Height / 2) + (trailer.play_pause.Height / 2)))
            {
                if (counterFrames > MAX_FRAMES_PAUSE)
                {
                    PauseTrailer();
                    checkedButton = true;
                    counterFrames = 0;
                }
                else
                {
                    counterFrames++;
                }
            }
            else if (X < (this.Width-(trailer.skip.Margin.Right) + 50) && X > (this.Width - (trailer.skip.Margin.Right) - 50 - trailer.skip.Width) &&
                    Y > ((this.Height / 2) - (trailer.skip.Height / 2) - 50) && Y < ((this.Height / 2) + (trailer.skip.Height / 2) + 50))
            {
                if (counterFrames > MAX_FRAMES_PAUSE)
                {
                    SkipTrailer();
                    checkedButton = true;
                    counterFrames = 0;
                }
                else
                    counterFrames++;
                
            } else
            {
                counterFrames = 0;
            }
        }

        private void PauseTrailer()
        {
            if (paused)
            {
                trailer.mediaElement.Play();
                trailer.play_pause.Source = pauseimage;
                paused = false;
            }
            else
            {
                trailer.mediaElement.Pause();
                trailer.play_pause.Source = playimage;
                paused = true;
            }
        }

        private void SkipTrailer()
        {
            DataLog.Log(DataLog.DebugLevel.Message, "Skipped trailer");
            isPointed = false;
            trailer.mediaElement.Stop();
            information = new DemoInformation();
            contentControl.Content = information;
            information.button1.Click += InformationBackButton;
        }

        private void InitProducts()
        {
            foreach (Model.Movie movie in db.movies)
                if (movie.Position != 0)
                    windowProducts.Add(movie.Position, new Model.Product(movie));

            foreach (Model.Music movie in db.musics)
                if (movie.Position != 0)
                    windowProducts.Add(movie.Position, new Model.Product(movie));

            foreach (Model.Book movie in db.books)
                if (movie.Position != 0)
                    windowProducts.Add(movie.Position, new Model.Product(movie));
        }

        public void ShowButtonsOnTrailer()
        {
            trailer.play_pause.Visibility = Visibility.Visible;
            trailer.skip.Visibility = Visibility.Visible;
        }

        private void HideButtonsOnTrailer()
        {
            trailer.play_pause.Visibility = Visibility.Hidden;
            trailer.skip.Visibility = Visibility.Hidden;
        }

    }
}
