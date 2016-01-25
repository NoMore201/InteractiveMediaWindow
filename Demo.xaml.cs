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

        private KinectCoreWindow kWin;

        private DbFileManager db;

        private SortedList<int, Model.Product> windowProducts;

        private bool paused;
        private bool checkedButton;

        private int counterFrames;
        private int pointerID;

        public Demo()
        {
            InitializeComponent();

            // variable initialization
            isPointed = false;
            checkedButton = false;
            paused = false;
            counterFrames = 0;

            // object initialization
            IdleAnimateFirstHand();
            kc = new KinectController();
            hue = new HueController("192.168.0.2");
            hue.Connect();
            kc.bodyReader.FrameArrived += HandleFrame;
            idle = new DemoIdle();
            idle.button.Click += button_Click;
            this.contentControl.Content = idle;
            KinectRegion.SetKinectRegion(this, kinectRegion);
            this.kinectRegion.KinectSensor = kc.kinectSensor;

            windowProducts = new SortedList<int, Model.Product>();
            db = new DbFileManager();

            InitProducts();
        }

        private void KWin_PointerMoved(object sender, KinectPointerEventArgs e)
        {       
            CheckPointInButton(e.CurrentPoint.Position.X, e.CurrentPoint.Position.Y);
        }

        private void HandleFrame(object sender, BodyFrameArrivedEventArgs e)
        {
            kc.Controller_FrameArrived(sender, e);

            int zoneP = kc.GetPointedZone();

            if (zoneP != 0 && !isPointed)
            {
                idle.label.Content += zoneP.ToString(); ;
                if (zoneP == 1 || zoneP == 3)
                {
                    Model.LightColors colors = windowProducts[1].GetColors();
                    hue.SendDoubleColorCommand(colors.color1, colors.color2, "1");
                    hue.TurnOff("7");
                    StartTrailer(windowProducts[1].GetTrailer());
                } else
                {
                    Model.LightColors colors = windowProducts[2].GetColors();
                    hue.SendDoubleColorCommand(colors.color1, colors.color2, "1");
                    hue.SendAlert("3344FF", "7");
                    hue.TurnOff("1");
                    StartTrailer(windowProducts[2].GetTrailer());
                }

                isPointed = true;
            } else if (zoneP == 0 && isPointed)
            {
                isPointed = false;
                //hue.SendColor("FFFFFF", 1f, (byte)150, "7");
                //hue.SendColor("FFFFFF", 1f, (byte)150, "1");
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
            kWin.PointerEntered += KWin_PointerEntered;
            kWin.PointerExited += KWin_PointerExited;
            kWin.PointerMoved += KWin_PointerMoved;
        }

        private void KWin_PointerExited(object sender, KinectPointerEventArgs e)
        {
            HideButtonsOnTrailer();
        }

        private void KWin_PointerEntered(object sender, KinectPointerEventArgs e)
        {
            ShowButtonsOnTrailer();
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

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            // Implementare interfaccia

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            StartTrailer(windowProducts[2].GetTrailer());
            kc.bodyReader.FrameArrived += null;
        }

        private void CheckPointInButton(float X, float Y)
        {
            X = X * (float) this.Width;
            Y = Y * (float) this.Height;

            if (X > trailer.play_pause.Margin.Left-50 && X < trailer.play_pause.Margin.Left + trailer.play_pause.Width + 50 &&
                    Y > ((this.Height / 2) - (trailer.play_pause.Height / 2) - 50) && Y < ((this.Height / 2) + (trailer.play_pause.Height / 2)))
            {
                if (counterFrames>127)
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
            else if (X < trailer.skip.Margin.Right + 50 && X > (trailer.play_pause.Margin.Right + trailer.skip.Width - 50) &&
                    Y > ((this.Height / 2) - (trailer.skip.Height / 2) - 50) && Y < ((this.Height / 2) + (trailer.skip.Height / 2) + 50))
            {
                if (counterFrames > 127)
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
            //DataLog.ToConsole(DataLog.DebugLevel.Message, counterFrames.ToString());
        }

        private void PauseTrailer()
        {
            if (paused)
            {
                trailer.mediaElement.Play();
                paused = false;
            }
            else
            {
                trailer.mediaElement.Pause();
                paused = true;
            }
        }

        private void SkipTrailer()
        {
            //DataLog.ToConsole(DataLog.DebugLevel.Message, "skip");
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
    }
}
