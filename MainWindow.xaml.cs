namespace Microsoft.Samples.Kinect.BodyBasics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Input;
    using Microsoft.Kinect.Wpf.Controls;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow : Window
    {
        public static float ACTIVATION_THRESHOLD = 0.5f;

        private const float STILL_THRESHOLD = 0.04f;
        private const int COUNTER_THRESHOLD = 30;
        private const float BORDERX_THRESHOLD = 0.2f;
        private const float BORDERY_THRESHOLD = 0.1f;
        private const float MAXX = 1.5f;
        private const float MAXY = 1f;

        private KinectSensor kinectSensor;
        private BodyFrameReader bodyReader;
        private UiTools uitools;
        private Body[] bodies;
        public float offsetX;
        public float offsetY;


        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            this.kinectSensor = KinectSensor.GetDefault();

            this.InitializeComponent();

            // open the reader for the body frames
            this.bodyReader = this.kinectSensor.BodyFrameSource.OpenReader();
            this.kinectSensor.Open();

            this.uitools = new UiTools(this);

            this.offsetX = 0f;
            this.offsetY = 0f;

            this.buttonCalibra.Click += ButtonCalibra_Click;

        }

        private void ButtonCalibra_Click(object sender, RoutedEventArgs e)
        {
            CalibrationWindow cw = new CalibrationWindow(this, kinectSensor);
            cw.Show();
        }

        public void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (BodyFrame frame = e.FrameReference.AcquireFrame())
            {
                this.offsetXBox.Text = offsetX.ToString();
                this.offsetYBox.Text = offsetY.ToString();
                if (frame != null)
                {
                    if (this.bodies == null)
                    {
                        this.bodies = new Body[frame.BodyCount];
                    }

                    frame.GetAndRefreshBodyData(this.bodies);

                    Body near = uitools.getNearestBody(this.bodies);

                    if (uitools.checkPointingRight(near, STILL_THRESHOLD, COUNTER_THRESHOLD))
                    {
                        uitools.PopulateLeft(near);

                        float pointedX = uitools.calculateX(near.Joints[JointType.ShoulderRight].Position, near.Joints[JointType.HandTipRight].Position) - offsetX;
                        float pointedY = uitools.calculateY(near.Joints[JointType.ShoulderRight].Position, near.Joints[JointType.HandTipRight].Position) - offsetY;
                        this.hand.Text = "X= " + pointedX.ToString() + "\nY= " + pointedY.ToString();
                        this.zoneBox.Text = zonePointed(pointedX, pointedY).ToString();
                    }
                    else if (uitools.checkPointingLeft(near, STILL_THRESHOLD, COUNTER_THRESHOLD))
                    {
                        uitools.PopulateLeft(near);

                        float pointedX = uitools.calculateX(near.Joints[JointType.ShoulderLeft].Position, near.Joints[JointType.HandTipLeft].Position) - offsetX;
                        float pointedY = uitools.calculateY(near.Joints[JointType.ShoulderLeft].Position, near.Joints[JointType.HandTipLeft].Position) - offsetY;
                        this.hand.Text = "X= " + pointedX.ToString() + "\nY= " + pointedY.ToString();
                        this.zoneBox.Text = zonePointed(pointedX, pointedY).ToString();
                    }
                    else
                        this.hand.Text = "notPointing";
                }
                else
                    this.hand.Text = "no Frame";
            }
        }

        // Handle the windows once it is loaded
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.bodyReader != null)
            {
                this.bodyReader.FrameArrived += this.Reader_FrameArrived;
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.bodyReader != null)
            {
                // BodyFrameReader is IDisposable
                this.bodyReader.Dispose();
                this.bodyReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            Debugger.Log(1, "Sensor available? ", e.IsAvailable.ToString());
        }

        private int zonePointed(float pointedX, float pointedY)
        {
            if (pointedX < -BORDERX_THRESHOLD && pointedY < -BORDERY_THRESHOLD &&
                  pointedX > -MAXX && pointedY > -MAXY)
                return 3;
            else if (pointedX > BORDERX_THRESHOLD && pointedY < -BORDERY_THRESHOLD &&
                  pointedX < MAXX && pointedY > -MAXY)
                return 4;
            else if (pointedX > BORDERX_THRESHOLD && pointedY > BORDERY_THRESHOLD &&
                  pointedX < MAXX && pointedY < MAXY)
                return 2;
            else if (pointedX < -BORDERX_THRESHOLD && pointedY > BORDERY_THRESHOLD &&
                  pointedX > -MAXX && pointedY < MAXY)
                return 1;
            else
                return 0;
        }

        private void buttonDB_Click(object sender, RoutedEventArgs e)
        {
            DatabaseWindow cw = new DatabaseWindow();
            cw.Show();
        }

        private void buttonCalibra_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
