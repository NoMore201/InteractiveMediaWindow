//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

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

        private const float ACTIVATION_THRESHOLD = 0.5f;
        private const float STILL_THRESHOLD = 0.04f;
        private const int COUNTER_THRESHOLD = 30;

        private KinectSensor kinectSensor;
        private BodyFrameReader bodyReader;
        private Body[] bodies;
        private float lastAveragePositionRight;
        private float lastAveragePositionLeft;
        private int frameCounterRight;
        private int frameCounterLeft;
        private bool hasPointed;

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

            this.lastAveragePositionRight = 0;
            this.lastAveragePositionLeft = 0;

            this.hasPointed = false;

            frameCounterRight = 0;
            frameCounterLeft = 0;
        }



        private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (BodyFrame frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (this.bodies == null)
                    {
                        this.bodies = new Body[frame.BodyCount];
                    }

                    frame.GetAndRefreshBodyData(this.bodies);

                    Body near = getNearestBody(this.bodies);

                    Joint first = near.Joints[JointType.ShoulderRight];
                    Joint second = near.Joints[JointType.ElbowRight];
                    Joint third = near.Joints[JointType.HandTipRight];
                    Joint fourth = near.Joints[JointType.HandRight];
                    Joint fifth = near.Joints[JointType.WristRight];

                    this.firstJointX.Text = first.Position.X.ToString();
                    this.firstJointY.Text = first.Position.Y.ToString();
                    this.firstJointZ.Text = first.Position.Z.ToString();
                    this.firstJointTracked.Text = first.TrackingState.ToString();

                    this.secondJointX.Text = second.Position.X.ToString();
                    this.secondJointY.Text = second.Position.Y.ToString();
                    this.secondJointZ.Text = second.Position.Z.ToString();
                    this.secondJointTracked.Text = second.TrackingState.ToString();

                    this.thirdJointX.Text = third.Position.X.ToString();
                    this.thirdJointY.Text = third.Position.Y.ToString();
                    this.thirdJointZ.Text = third.Position.Z.ToString();
                    this.thirdJointTracked.Text = third.TrackingState.ToString();

                    this.fourthJointX.Text = fourth.Position.X.ToString();
                    this.fourthJointY.Text = fourth.Position.Y.ToString();
                    this.fourthJointZ.Text = fourth.Position.Z.ToString();
                    this.fourthJointTracked.Text = fourth.TrackingState.ToString();

                    this.fifthJointX.Text = fifth.Position.X.ToString();
                    this.fifthJointY.Text = fifth.Position.Y.ToString();
                    this.fifthJointZ.Text = fifth.Position.Z.ToString();
                    this.fifthJointTracked.Text = fifth.TrackingState.ToString();

                    if (checkPointingRight(near))
                    {
                        float pointedX = calculateX(near.Joints[JointType.WristRight].Position, near.Joints[JointType.HandTipRight].Position);
                        float pointedY = calculateY(near.Joints[JointType.WristRight].Position, near.Joints[JointType.HandTipRight].Position);
                        this.hand.Text = "IsPointing! X= " + pointedX.ToString() + "\nY= " + pointedY.ToString();
                    }
                    else if (checkPointingLeft(near))
                    {
                        float pointedX = calculateX(near.Joints[JointType.WristLeft].Position, near.Joints[JointType.HandTipLeft].Position);
                        float pointedY = calculateY(near.Joints[JointType.WristLeft].Position, near.Joints[JointType.HandTipLeft].Position);
                        this.hand.Text = "IsPointing! X= " + pointedX.ToString() + "\nY= " + pointedY.ToString();
                    }
                    else
                        this.hand.Text = "notPointing";
                }
                else
                    this.hand.Text = "no Frame";
            }
        }

        private bool checkPointingRight(Body near)
        {

            handstate.Text = "Right";
            bool isPointing = false;

            Joint first = near.Joints[JointType.ShoulderRight];

            Joint third = near.Joints[JointType.HandRight];
            this.spallamano.Text = ((first.Position.Z - third.Position.Z) + Math.Abs(first.Position.X - third.Position.X)).ToString();

            DataLog dl = new DataLog();
            
            if (!hasPointed && near.HandRightState != HandState.Closed &&
                near.HandRightState != HandState.Open &&
                (first.Position.Z - third.Position.Z) + Math.Abs(first.Position.X - third.Position.X) > ACTIVATION_THRESHOLD)
            {
                float averagePosition = (third.Position.X + third.Position.Y) / 2;

                this.body.Text = Math.Abs(averagePosition - lastAveragePositionRight).ToString();

                if (Math.Abs(averagePosition - lastAveragePositionRight) < STILL_THRESHOLD)
                {
                    frameCounterRight++;
                }
                else
                {
                    frameCounterRight = 0;
                    lastAveragePositionRight = averagePosition;
                }
                if (frameCounterRight > COUNTER_THRESHOLD)
                {
                    isPointing = true;
                    hasPointed = true;
                }

            }
            else if ((first.Position.Z - third.Position.Z) + Math.Abs(first.Position.X - third.Position.X) < ACTIVATION_THRESHOLD)
            {
                hasPointed = false;
                frameCounterRight = 0;
            }

            if (frameCounterRight > COUNTER_THRESHOLD)
            {
                isPointing = true;
            }

            return isPointing;
        }

        private bool checkPointingLeft(Body near)
        {
            this.handstate.Text = "Left";
            bool isPointing = false;

            Joint first = near.Joints[JointType.ShoulderLeft];

            Joint third = near.Joints[JointType.HandLeft];

            this.spallamano.Text = ((first.Position.Z - third.Position.Z) + Math.Abs(first.Position.X - third.Position.X)).ToString();

            DataLog dl = new DataLog();

            
            if (!hasPointed && near.HandLeftState != HandState.Closed &&
                near.HandLeftState != HandState.Open &&
                (first.Position.Z - third.Position.Z) + Math.Abs(first.Position.X - third.Position.X) > ACTIVATION_THRESHOLD)
            {
                float averagePosition = (third.Position.X + third.Position.Y) / 2;

                this.body.Text = Math.Abs(averagePosition - lastAveragePositionLeft).ToString();
                

                if (Math.Abs(averagePosition - lastAveragePositionLeft) < STILL_THRESHOLD)
                {
                    frameCounterLeft++;
                }
                else
                {
                    frameCounterLeft = 0;
                    lastAveragePositionLeft = averagePosition;
                }
                if (frameCounterLeft > COUNTER_THRESHOLD)
                {
                    isPointing = true;
                    hasPointed = true;
                }

            }
            else if ((first.Position.Z - third.Position.Z) + Math.Abs(first.Position.X - third.Position.X) < ACTIVATION_THRESHOLD)
            {
                hasPointed = false;
                frameCounterLeft = 0;
            }

            if (frameCounterLeft > COUNTER_THRESHOLD)
            {
                isPointing = true;
            }

            return isPointing;
        }

        private float calculateX(CameraSpacePoint one, CameraSpacePoint two)
        {
            return one.X + ((one.Z * one.X - (one.Z * two.X)) / (two.Z - one.Z));
        }

        private float calculateY(CameraSpacePoint one, CameraSpacePoint two)
        {
            return one.Y + ((one.Z * one.Y - (one.Z * two.Y)) / (two.Z - one.Z));
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

        private Body getNearestBody(Body[] bodies)
        {
            Body nearest = bodies[0];
            foreach (Body b in bodies)
            {
                if (b.IsTracked)
                {
                    if (!nearest.IsTracked)
                    {
                        nearest = b;
                    }
                    else
                    {
                        IReadOnlyDictionary<JointType, Joint> joints = b.Joints;
                        IReadOnlyDictionary<JointType, Joint> nearestJoints = nearest.Joints;

                        float nearestPointOriginal = getNearestPointZ(joints);
                        float nearestPoint = getNearestPointZ(nearestJoints);

                        if (nearestPointOriginal < nearestPoint)
                        {
                            nearest = b;
                        }
                    }

                }
            }
            return nearest;
        }

        private float getNearestPointZ(IReadOnlyDictionary<JointType, Joint> joints)
        {
            float result = 100;
            foreach (JointType jointType in joints.Keys)
            {
                CameraSpacePoint position = joints[jointType].Position;
                if (position.Z > 0.1f && position.Z < result)
                {
                    result = position.Z;
                }
            }

            return result;
        }
    }
}
