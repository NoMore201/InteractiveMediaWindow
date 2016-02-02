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
using Microsoft.Samples.Kinect.BodyBasics.Model;

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
        private int counterFramesBody;

        private uint mainHandId;
        private uint lastMainHandId;

        private int currentProduct;

        private ulong nearest;

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
            counterFramesBody = 0;
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

            idle.label.Opacity = 0;

            kWin = KinectCoreWindow.GetForCurrentThread();
            if (!PointerMovedAssigned)
            {
                kWin.PointerMoved += KWin_PointerMoved;
                kWin.PointerExited += KWin_PointerExited;
                kWin.PointerEntered += KWin_PointerEntered;
                PointerMovedAssigned = true;
            }

            /*
            this.MouseMove += Demo_MouseMove;

            currentProduct = 1;
            StartTrailer(windowProducts[currentProduct].GetTrailer());
            isPointed = true;*/

        }

        private void KWin_PointerEntered(object sender, KinectPointerEventArgs e)
        {
            idle.label.Opacity = 1;
        }

        private void Demo_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (state == 2)
            {
                ShowButtonsOnTrailer();
                CheckPointInButton((float) (e.GetPosition(this).X/this.Width), (float) (e.GetPosition(this).Y/this.Height));
            } else if (state == 3)
            {
                CheckPointInThirdButtons((float)(e.GetPosition(this).X / this.Width), (float)(e.GetPosition(this).Y / this.Height));
            } else if (state == 4)
            {
                CheckPointInFourthButtons((float)(e.GetPosition(this).X / this.Width), (float)(e.GetPosition(this).Y / this.Height));
            }
        }

        private void KWin_PointerMoved(object sender, KinectPointerEventArgs e)
        {
            if (state == 2)
            {
                if (mainHandId == 0 && e.CurrentPoint.Properties.IsPrimary &&
                e.CurrentPoint.Properties.IsEngaged)
                {
                    mainHandId = e.CurrentPoint.PointerId;
                    lastMainHandId = mainHandId;
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
                    lastMainHandId = mainHandId;
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
                    
                    if(lastMainHandId==0)
                        lastMainHandId = mainHandId;
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
                        currentProduct = 1;
                        Model.LightColors colors = windowProducts[currentProduct].GetColors();
                        hue.isDoubleActive = true;
                        Task.Run(async () => {
                            await hue.SendDoubleColorCommand(colors.color1, colors.color2, FIRST_HUE);
                        });
                        Task.Run(async () => {
                            await hue.TurnOffDelayed(SECOND_HUE);
                        });
                        hue.TurnOff(SECOND_HUE);
                        StartTrailer(windowProducts[currentProduct].GetTrailer());
                        isPointed = true;
                        nearest = kc.indexNearest;
                    }
                    else
                    {
                        currentProduct = 2;
                        Model.LightColors colors = windowProducts[currentProduct].GetColors();
                        hue.isDoubleActive = true;
                        Task.Run(async () => {
                            await hue.SendDoubleColorCommand(colors.color1, colors.color2, SECOND_HUE);
                        });
                        Task.Run(async () => {
                            await hue.TurnOffDelayed(FIRST_HUE);
                        });
                        hue.TurnOff(FIRST_HUE);
                        StartTrailer(windowProducts[currentProduct].GetTrailer());
                        isPointed = true;
                        nearest = kc.indexNearest;
                    }
                }


            }
            
            else
            {
                kc.Controller_FrameArrived(sender, e);

                if (!kc.nearest.IsTracked)
                {
                    counterFramesBody++;
                    if(counterFramesBody>100)
                        InformationBackButton();
                } else
                {
                    counterFramesBody = 0;
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
            
        }

        
        private void KWin_PointerExited(object sender, KinectPointerEventArgs e)
        {
            /*
            if(e.CurrentPoint.PointerId== lastMainHandId)
            {
                InformationBackButton();
                lastMainHandId = 0;
            }       
            */

            idle.label.Opacity = 0;
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            SkipTrailer();
        }

        private void InformationBackButton()
        {
            counterFrames = 0;
            counterFramesBody = 0;
            contentControl.Content = idle;
            isPointed = false;
            hue.isDoubleActive = false;
            IdleAnimateFirstHand();
            state = 1;

            if (relatedBackup != null)
                windowProducts[currentProduct] = relatedBackup;

            relatedBackup = null;
            hue.isBright = true;
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
                        relatedBackup = null;
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
            else if (X > this.Width - information.restart.Width - 85 && X < this.Width - 15 &&
                    Y > ((this.Height / 2) - (information.restart.ActualHeight / 2)) && Y < ((this.Height / 2) + (information.restart.ActualHeight / 2)) && windowProducts[currentProduct].GetTrailer()!=null)
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

            if(Y<200 && X < 200)
            {
                if (counterFrames > MAX_FRAMES_PAUSE)
                {
                    counterFrames = 0;
                    relatedWindow.back.Opacity = 0.25;
                    SkipTrailer();
                }
                else 
                {
                    counterFrames++;
                    relatedWindow.back.Opacity += 0.03;
                }
            } else if (Y>(this.Height - 300)){
                if (X < 220 && relatedWindow.backw.Opacity != 0)
                {
                    if (counterFrames > MAX_FRAMES_PAUSE)
                    {
                        counterFrames = 0;
                        relatedWindow.backw.Opacity = 0.25;
                        relatedWindow.BackPage();
                    }
                    else
                    {
                        counterFrames++;
                        relatedWindow.backw.Opacity += 0.03;
                    }
                } else if (X>300 && X < 650)
                {
                    if (counterFrames > MAX_FRAMES_PAUSE)
                    {
                        counterFrames = 0;
                        relatedWindow.firstIm.Opacity = 0.25;
                        GoToRelatedInfo(relatedWindow.relateds[relatedWindow.beginTo]);
                    }
                    else
                    {
                        counterFrames++;
                        relatedWindow.firstIm.Opacity += 0.03;
                    }
                } else if (X > 770 && X < 1120 && relatedWindow.secondIm.Opacity!=0)
                {
                    if (counterFrames > MAX_FRAMES_PAUSE)
                    {
                        counterFrames = 0;
                        relatedWindow.secondIm.Opacity = 0.25;
                        GoToRelatedInfo(relatedWindow.relateds[relatedWindow.beginTo+1]);
                    }
                    else
                    {
                        counterFrames++;
                        relatedWindow.secondIm.Opacity += 0.03;
                    }
                } else if (X > this.Width - 620 && X < this.Width - 270 && relatedWindow.thirdIm.Opacity != 0)
                {
                    if (counterFrames > MAX_FRAMES_PAUSE)
                    {
                        counterFrames = 0;
                        relatedWindow.thirdIm.Opacity = 0.25;
                        GoToRelatedInfo(relatedWindow.relateds[relatedWindow.beginTo+2]);
                    }
                    else
                    {
                        counterFrames++;
                        relatedWindow.thirdIm.Opacity += 0.03;
                    }
                } else if(X > this.Width - 220 && relatedWindow.image1.Opacity!=0)
                {
                    if (counterFrames > MAX_FRAMES_PAUSE)
                    {
                        counterFrames = 0;
                        relatedWindow.image1.Opacity = 0.25;
                        relatedWindow.GoToNextPage();
                    }
                    else
                    {
                        counterFrames++;
                        relatedWindow.image1.Opacity += 0.03;
                    }
                }
            }
        }

        private void GoToRelatedInfo(Product product)
        {
            relatedBackup = windowProducts[currentProduct];
            windowProducts[currentProduct] = product;
            SkipTrailer();
        }

        private void GoToRelated()
        {
            state = 4;
            relatedWindow = new DemoRelateds(GetRelateds(windowProducts[currentProduct]));
            contentControl.Content = relatedWindow;
            isPointed = true;
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
            isPointed = true;
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
                        relateds.Add(new Model.Product(mov, db.GetTracklist(mov.ID)));
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
