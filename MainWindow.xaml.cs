namespace Microsoft.Samples.Kinect.BodyBasics
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using Microsoft.Kinect;

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

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {

            InitializeComponent();

            kc = new KinectController();
            kc.bodyReader.FrameArrived += Reader_FrameArrived;
        }

        public void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            kc.Controller_FrameArrived(sender, e);

            // Fill all text boxes
            FillBoxes();

            // the offset is modified by calibration window
            offsetXBox.Text = kc.offsetX.ToString();
            offsetYBox.Text = kc.offsetY.ToString();
            if (kc.Arm == ArmPointing.Right)
            {
                float pointedX = kc.GetPointedX();
                float pointedY = kc.GetPointedY();

                pointedPoint.Text = "X= " + pointedX.ToString() + "\nY= " + pointedY.ToString();
                zoneBox.Text = kc.GetPointedZone().ToString();
            }
            else if (kc.Arm == ArmPointing.Left)
            {
                float pointedX = kc.GetPointedX();
                float pointedY = kc.GetPointedY();

                pointedPoint.Text = "X= " + pointedX.ToString() + "\nY= " + pointedY.ToString();
                zoneBox.Text = kc.GetPointedZone().ToString();
            }
            else
                this.pointedPoint.Text = "notPointing";
        }

        private void FillBoxes()
        {
            // left arm
            leftFirstJointX.Text = kc.ShoulderLeft.Position.X.ToString();
            leftFirstJointY.Text = kc.ShoulderLeft.Position.Y.ToString();
            leftFirstJointZ.Text = kc.ShoulderLeft.Position.Z.ToString();
            leftFirstJointTracked.Text = kc.ShoulderLeft.TrackingState.ToString();
            leftSecondJointX.Text = kc.ElbowLeft.Position.X.ToString();
            leftSecondJointY.Text = kc.ElbowLeft.Position.Y.ToString();
            leftSecondJointZ.Text = kc.ElbowLeft.Position.Z.ToString();
            leftSecondJointTracked.Text = kc.ElbowLeft.TrackingState.ToString();
            leftThirdJointX.Text = kc.HandTipLeft.Position.X.ToString();
            leftThirdJointY.Text = kc.HandTipLeft.Position.Y.ToString();
            leftThirdJointZ.Text = kc.HandTipLeft.Position.Z.ToString();
            leftThirdJointTracked.Text = kc.HandTipLeft.TrackingState.ToString();
            leftFourthJointX.Text = kc.HandLeft.Position.X.ToString();
            leftFourthJointY.Text = kc.HandLeft.Position.Y.ToString();
            leftFourthJointZ.Text = kc.HandLeft.Position.Z.ToString();
            leftFourthJointTracked.Text = kc.HandLeft.TrackingState.ToString();
            leftFifthJointX.Text = kc.WristLeft.Position.X.ToString();
            leftFifthJointY.Text = kc.WristLeft.Position.Y.ToString();
            leftFifthJointZ.Text = kc.WristLeft.Position.Z.ToString();
            leftFifthJointTracked.Text = kc.WristLeft.TrackingState.ToString();

            // right arm
            rightFirstJointX.Text = kc.ShoulderLeft.Position.X.ToString();
            rightFirstJointY.Text = kc.ShoulderLeft.Position.Y.ToString();
            rightFirstJointZ.Text = kc.ShoulderLeft.Position.Z.ToString();
            rightFirstJointTracked.Text = kc.ShoulderLeft.TrackingState.ToString();
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

    }
}
