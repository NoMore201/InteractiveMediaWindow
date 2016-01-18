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
        private const float STILL_THRESHOLD = 0.04f;
        private const int COUNTER_THRESHOLD = 30;
        private const float BORDERX_THRESHOLD = 0.2f;
        private const float BORDERY_THRESHOLD = 0.1f;
        private const float MAXX = 1.5f;
        private const float MAXY = 1f;

        KinectController kc;
        public float offsetX;
        public float offsetY;


        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {

            InitializeComponent();

            kc = new KinectController();
            kc.bodyReader.FrameArrived += Reader_FrameArrived;

            offsetX = 0f;
            offsetY = 0f;

            this.buttonCalibra.Click += ButtonCalibra_Click;
        }

        private void ButtonCalibra_Click(object sender, RoutedEventArgs e)
        {
            CalibrationWindow cw = new CalibrationWindow(this, kc.kinectSensor);
            cw.Show();
        }

        public void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            kc.Controller_FrameArrived(sender, e);

            // the offset is modified by calibration window
            this.offsetXBox.Text = offsetX.ToString();
            this.offsetYBox.Text = offsetY.ToString();
            if (kc.Arm == ArmPointing.Right)
            {
                float pointedX = kc.calculateX(kc.ShoulderRight.Position,
                    kc.HandTipRight.Position) - offsetX;
                float pointedY = kc.calculateY(kc.ShoulderRight.Position,
                    kc.HandTipRight.Position) - offsetY;

                this.hand.Text = "X= " + pointedX.ToString() + "\nY= " + pointedY.ToString();
                this.zoneBox.Text = zonePointed(pointedX, pointedY).ToString();
            } else if (kc.Arm == ArmPointing.Left)
            {
                float pointedX = kc.calculateX(kc.ShoulderLeft.Position,
                    kc.HandTipLeft.Position) - offsetX;
                float pointedY = kc.calculateY(kc.ShoulderLeft.Position,
                    kc.HandTipLeft.Position) - offsetY;
                this.hand.Text = "X= " + pointedX.ToString() + "\nY= " + pointedY.ToString();
                this.zoneBox.Text = zonePointed(pointedX, pointedY).ToString();
            }
            else
                this.hand.Text = "notPointing";

                //using (BodyFrame frame = e.FrameReference.AcquireFrame())
                //{
                //    this.offsetXBox.Text = offsetX.ToString();
                //    this.offsetYBox.Text = offsetY.ToString();
                //    if (frame != null)
                //    {
                //        if (this.bodies == null)
                //        {
                //            this.bodies = new Body[frame.BodyCount];
                //        }

                //        frame.GetAndRefreshBodyData(this.bodies);

                //        Body near = uitools.getNearestBody(this.bodies);

                //        if (uitools.checkPointingRight(near, STILL_THRESHOLD, COUNTER_THRESHOLD))
                //        {
                //            uitools.PopulateLeft(near);

                //            float pointedX = uitools.calculateX(near.Joints[JointType.ShoulderRight].Position, near.Joints[JointType.HandTipRight].Position) - offsetX;
                //            float pointedY = uitools.calculateY(near.Joints[JointType.ShoulderRight].Position, near.Joints[JointType.HandTipRight].Position) - offsetY;
                //            this.hand.Text = "X= " + pointedX.ToString() + "\nY= " + pointedY.ToString();
                //            this.zoneBox.Text = zonePointed(pointedX, pointedY).ToString();
                //        }
                //        else if (uitools.checkPointingLeft(near, STILL_THRESHOLD, COUNTER_THRESHOLD))
                //        {
                //            uitools.PopulateLeft(near);

                //            float pointedX = uitools.calculateX(near.Joints[JointType.ShoulderLeft].Position, near.Joints[JointType.HandTipLeft].Position) - offsetX;
                //            float pointedY = uitools.calculateY(near.Joints[JointType.ShoulderLeft].Position, near.Joints[JointType.HandTipLeft].Position) - offsetY;
                //            this.hand.Text = "X= " + pointedX.ToString() + "\nY= " + pointedY.ToString();
                //            this.zoneBox.Text = zonePointed(pointedX, pointedY).ToString();
                //        }
                //        else
                //            this.hand.Text = "notPointing";
                //    }
                //    else
                //        this.hand.Text = "no Frame";
                //}
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (kc.bodyReader != null)
            {
                // BodyFrameReader is IDisposable
                kc.bodyReader.Dispose();
                kc.bodyReader = null;
            }

            if (kc.kinectSensor != null)
            {
                kc.kinectSensor.Close();
                kc.kinectSensor = null;
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
