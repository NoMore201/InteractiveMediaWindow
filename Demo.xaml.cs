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

        public static int MAX_FRAMES_PAUSE = 25;
        public static string FIRST_HUE = "4";
        public static string SECOND_HUE = "3";

        KinectController kc;
        Task hands;
        bool isPointed;
        private HueController hue;

        private DemoIdle idle;
        private DemoTrailer trailer;
        private DemoInformation information;
        private DemoRelateds relatedWindow;

        private KinectCoreWindow kWin;

        private DbFileManager db;

        private SortedList<int, Model.Product> windowProducts;

        private bool paused;
        private bool checkedButton;

        private int counterFrames;

        private uint mainHandId;

        private int currentProduct;

        BitmapImage playimage;
        BitmapImage pauseimage;

        public bool MediaEndedAssigned { get; private set; }
        public bool PointerMovedAssigned { get; private set; }

        private int state;

        private Model.Product relatedBackup;

        public Demo()
        {
            InitializeComponent();

            // variable initialization
            isPointed = false;
            checkedButton = false;
            paused = false;
            counterFrames = 0;
            mainHandId = 0;
            PointerMovedAssigned = false;
            MediaEndedAssigned = false;

            relatedBackup = null;

            // object initialization
            IdleAnimateFirstHand();
            kc = new KinectController();
            hue = new HueController("192.168.0.2");
            hue.Connect();
            hue.isBright = true;
            kc.bodyReader.FrameArrived += HandleFrame;
            idle = new DemoIdle();
            this.contentControl.Content = idle;
            KinectRegion.SetKinectRegion(this, kinectRegion);

            App app = ((App)Application.Current);
            app.KinectRegion = kinectRegion;
            
            windowProducts = new SortedList<int, Model.Product>();
            db = new DbFileManager();

            state = 1;

            playimage = new BitmapImage(new Uri("Images\\play.png", UriKind.Relative));
            pauseimage = new BitmapImage(new Uri("Images\\pause.png", UriKind.Relative));


            InitProducts();
        }

        private void KWin_PointerMoved(object sender, KinectPointerEventArgs e)
        {
            if (state == 2)
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
            } else if (state == 3)
            {
                if (mainHandId == 0 && e.CurrentPoint.Properties.IsPrimary &&
                                e.CurrentPoint.Properties.IsEngaged)
                {
                    mainHandId = e.CurrentPoint.PointerId;
                }

                if (e.CurrentPoint.PointerId == mainHandId)
                {
                    CheckPointInThirdButtons(e.CurrentPoint.Position.X, e.CurrentPoint.Position.Y);
                }
                if (e.CurrentPoint.PointerId == mainHandId && !e.CurrentPoint.Properties.IsEngaged)
                {
                    mainHandId = 0;
                }
            }
            else if (state == 4)
            {
                if (mainHandId == 0 && e.CurrentPoint.Properties.IsPrimary &&
                                e.CurrentPoint.Properties.IsEngaged)
                {
                    mainHandId = e.CurrentPoint.PointerId;
                }

                if (e.CurrentPoint.PointerId == mainHandId)
                {
                    CheckPointInFourthButtons(e.CurrentPoint.Position.X, e.CurrentPoint.Position.Y);
                }
                if (e.CurrentPoint.PointerId == mainHandId && !e.CurrentPoint.Properties.IsEngaged)
                {
                    mainHandId = 0;
                }
            }
        }

        private void HandleFrame(object sender, BodyFrameArrivedEventArgs e)
        {
            if (state == 1)
            {
                kc.Controller_FrameArrived(sender, e);

                int zoneP = kc.GetPointedZone();

                if (zoneP != 0 && !isPointed)
                {
                    if (zoneP == 1 || zoneP == 3)
                    {
                        Model.LightColors colors = windowProducts[1].GetColors();
                        hue.isDoubleActive = true;
                        Task.Run(async () => {
                            await hue.SendDoubleColorCommand(colors.color1, colors.color2, FIRST_HUE);
                        });
                        Task.Run(async () => {
                            await hue.TurnOffDelayed(SECOND_HUE);
                        });
                        hue.TurnOff(SECOND_HUE);
                        currentProduct = 1;
                        StartTrailer(windowProducts[1].GetTrailer());
                        isPointed = true;
                    }
                    else
                    {
                        Model.LightColors colors = windowProducts[2].GetColors();
                        hue.isDoubleActive = true;
                        Task.Run(async () => {
                            await hue.SendDoubleColorCommand(colors.color1, colors.color2, SECOND_HUE);
                        });
                        Task.Run(async () => {
                            await hue.TurnOffDelayed(FIRST_HUE);
                        });
                        hue.TurnOff(FIRST_HUE);
                        StartTrailer(windowProducts[2].GetTrailer());
                        currentProduct = 2;
                        isPointed = true;
                    }
                }
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
                hue.SendColor("FFFFFF", 0, SECOND_HUE);
                hue.TurnOff(FIRST_HUE);
            }));
        }

        private void HideSecondHand()
        {
            Dispatcher.Invoke(new Action(() => {
                idle.rightHand.Visibility = Visibility.Hidden;
                idle.leftHand.Visibility = Visibility.Visible;
                hue.SendColor("FFFFFF", 0, FIRST_HUE);
                hue.TurnOff(SECOND_HUE);
            }));
        }

        private void StartTrailer(string url)
        {
            hue.isBright = false;
            kc.Reset();
            counterFrames = 0;
            mainHandId = 0;
            paused = false;
            state = 2;
            isPointed = true;
            trailer = new DemoTrailer("trailers\\" + url);
            contentControl.Content = trailer;
            checkedButton = false;
            trailer.mediaElement.Play();
            if (!MediaEndedAssigned)
            {
                trailer.mediaElement.MediaEnded += MediaElement_MediaEnded;
                MediaEndedAssigned = true;
            }
            kWin = KinectCoreWindow.GetForCurrentThread();
            if (!PointerMovedAssigned)
            {
                kWin.PointerMoved += KWin_PointerMoved;
                PointerMovedAssigned = true;
            }
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            SkipTrailer();
        }

        private void InformationBackButton()
        {
            contentControl.Content = idle;
            isPointed = false;
            hue.isDoubleActive = false;
            IdleAnimateFirstHand();
            state = 1;
        }

        private void CheckPointInButton(float X, float Y)
        {
            X = X * (float) this.Width;
            Y = Y * (float) this.Height;

            if (X > trailer.play_pause.Margin.Left-50 && X < trailer.play_pause.Margin.Left + trailer.play_pause.Width + 50 && 
                    Y > ((this.Height / 2) - (trailer.play_pause.Height / 2) - 50) && Y < ((this.Height / 2) + (trailer.play_pause.Height / 2)))
            {
                if (counterFrames > MAX_FRAMES_PAUSE && !checkedButton)
                {
                    PauseTrailer();
                    checkedButton = true;
                    counterFrames = 0;
                    trailer.play_pause.Opacity = 0.25;
                }
                else if (!checkedButton)
                {
                    counterFrames++; 
                    trailer.play_pause.Opacity+=0.03;
                }
            }
            else if (X < (this.Width-(trailer.skip.Margin.Right) + 50) && X > (this.Width - (trailer.skip.Margin.Right) - 50 - trailer.skip.Width) &&
                    Y > ((this.Height / 2) - (trailer.skip.Height / 2) - 50) && Y < ((this.Height / 2) + (trailer.skip.Height / 2) + 50))
            {
                if (counterFrames > MAX_FRAMES_PAUSE && !checkedButton)
                {
                    checkedButton = true;
                    counterFrames = 0;
                    trailer.skip.Opacity = 0.25;
                    SkipTrailer();
                }
                else if(!checkedButton)
                {
                    counterFrames++;
                    trailer.skip.Opacity+=0.03;
                }    
                
            } else 
            {
                counterFrames = 0;
                trailer.play_pause.Opacity = 0.25;
                trailer.skip.Opacity = 0.25;
                checkedButton = false;
            }
        }

        private void CheckPointInThirdButtons(float X, float Y)
        {
            X = X * (float)this.Width;
            Y = Y * (float)this.Height;

            // Related Button
            if (X > this.Width - information.related.ActualWidth - 85 && X < this.Width - 15 &&
                    Y > 15 && Y < ((information.related.Height) + 75))
            {
                if (counterFrames > MAX_FRAMES_PAUSE && !checkedButton)
                {
                    checkedButton = true;
                    counterFrames = 0;
                    information.related.Opacity = 0.25;
                    if (relatedBackup != null)
                    {
                        windowProducts[currentProduct] = relatedBackup;
                    }
                    GoToRelated();
                }
                else if (!checkedButton)
                {
                    counterFrames++;
                    information.related.Opacity += 0.03;
                }
            }

            // Restart Button
            else if (X > this.Width - information.exit.Width - 85 && X < this.Width - 15 &&
                    Y > ((this.Height / 2) - (information.restart.Height / 2) - 15) && Y < ((this.Height / 2) + (information.restart.Height / 2) + 15) && windowProducts[currentProduct].GetTrailer()!="")
            {
                if (counterFrames > MAX_FRAMES_PAUSE && !checkedButton)
                {
                    checkedButton = true;
                    counterFrames = 0;
                    information.restart.Opacity = 0.25;
                    StartTrailer(windowProducts[currentProduct].GetTrailer());
                }
                else if (!checkedButton)
                {
                    counterFrames++;
                    information.restart.Opacity += 0.03;
                }

            }

            // Exit Button
            else if (X > this.Width - information.exit.ActualWidth - 85 && X < this.Width - 15 &&
                   Y < this.Height - 15 && Y > ( this.Height - (information.exit.Height) - 75))
            {
                if (counterFrames > MAX_FRAMES_PAUSE && !checkedButton)
                {
                    checkedButton = true;
                    counterFrames = 0;
                    information.exit.Opacity = 0.25;
                    InformationBackButton();
                }
                else if (!checkedButton)
                {
                    counterFrames++;
                    information.exit.Opacity += 0.03;
                }

            }
            else
            {
                counterFrames = 0;
                information.exit.Opacity = 0.25;
                information.restart.Opacity = 0.25;
                information.related.Opacity = 0.25;
                checkedButton = false;
            }
        }

        private void CheckPointInFourthButtons(float X, float Y)
        {
            X = X * (float)this.Width;
            Y = Y * (float)this.Height;
        }

        private void GoToRelated()
        {
            state = 4;
            relatedWindow = new DemoRelateds(GetRelateds(windowProducts[currentProduct]));
            contentControl.Content = relatedWindow;
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
            state = 3;
            isPointed = false;
            trailer.mediaElement.Stop();
            information = new DemoInformation(windowProducts[currentProduct]);
            contentControl.Content = information;
            hue.isBright = true;
        }

        private void InitProducts()
        {
            foreach (Model.Movie movie in db.movies)
                if (movie.Position != 0)
                    windowProducts.Add(movie.Position, new Model.Product(movie));

            foreach (Model.Music movie in db.musics)
                if (movie.Position != 0)
                    windowProducts.Add(movie.Position, new Model.Product(movie, db.GetTracklist(movie.ID)));

            foreach (Model.Book movie in db.books)
                if (movie.Position != 0)
                    windowProducts.Add(movie.Position, new Model.Product(movie));
        }

        public void ShowButtonsOnTrailer()
        {
                trailer.play_pause.Visibility = Visibility.Visible;
                trailer.skip.Visibility = Visibility.Visible;
                trailer.skip.Opacity = 0.25;
                trailer.play_pause.Opacity = 0.25;
            
        }

        private void HideButtonsOnTrailer()
        {
                trailer.play_pause.Visibility = Visibility.Hidden;
                trailer.skip.Visibility = Visibility.Hidden;
        }

        private void OnWindowClose(object sender, EventArgs e)
        {
            hue.TurnOff(FIRST_HUE);
            hue.TurnOff(SECOND_HUE);
        }

        private List<Model.Product> GetRelateds(Model.Product prod)
        {
            int type = prod.GetTyp();
            string[] favrelateds = prod.GetFavouriteRelateds();
            List<Model.Product> relateds = new List<Model.Product>();

            if (type==1)
            {
                foreach (Model.Movie mov in db.movies)
                    if (StringArrayContains(favrelateds, mov.ID + ""))
                        relateds.Add(new Model.Product(mov));
            }
            else if (type==2)
            {
                foreach (Model.Book mov in db.books)
                    if (StringArrayContains(favrelateds, mov.ID + ""))
                        relateds.Add(new Model.Product(mov));
            }
            else if (type==3)
            {
                foreach (Model.Music mov in db.musics)
                    if (StringArrayContains(favrelateds, mov.ID + ""))
                        relateds.Add(new Model.Product(mov));
            }

            return relateds;
        }

        private bool StringArrayContains(string[] arr, string test)
        {
            foreach (string s in arr)
                if (test == s)
                    return true;

            return false;
        }
    }
}
