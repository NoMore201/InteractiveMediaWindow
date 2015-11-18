﻿//------------------------------------------------------------------------------
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

        private KinectSensor kinectSensor;
        private BodyFrameReader bodyReader;
        private Body[] bodies;
        private float lastAveragePosition;
        private int frameCounter;
        private bool hasPointed = false;

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

            lastAveragePosition = 0;

            frameCounter = 0;
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
                        Debugger.Log(1, "Body count = ", frame.BodyCount.ToString());
                        this.body.Text = "a Body has been found!";
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

                    this.secondJointX.Text = second.Position.X.ToString();
                    this.secondJointY.Text = second.Position.Y.ToString();
                    this.secondJointZ.Text = second.Position.Z.ToString();

                    this.thirdJointX.Text = third.Position.X.ToString();
                    this.thirdJointY.Text = third.Position.Y.ToString();
                    this.thirdJointZ.Text = third.Position.Z.ToString();

                    this.fourthJointX.Text = fourth.Position.X.ToString();
                    this.fourthJointY.Text = fourth.Position.Y.ToString();
                    this.fourthJointZ.Text = fourth.Position.Z.ToString();

                    /*this.fifthJointX.Text=fifth.Position.X;
                    this.fifthJointY.Text=fifth.Position.Y;
                    this.fifthJointZ.Text=fifth.Position.Z;*/

                    if (checkPointing(near))
                    {
                        float pointedX = calculateX(near.Joints[JointType.WristRight].Position, near.Joints[JointType.HandTipRight].Position);
                        float pointedY = calculateX(near.Joints[JointType.WristRight].Position, near.Joints[JointType.HandTipRight].Position);
                        this.hand.Text = "IsPointing! X= " + pointedX.ToString() + "Y= " + pointedY;
                    }
                }
            }
        }

        private bool checkPointing(Body near)
        {
            bool isPointing=false;

            Joint first = near.Joints[JointType.ShoulderRight];
            Joint second = near.Joints[JointType.ElbowRight];
            Joint third = near.Joints[JointType.HandTipRight];
            Joint fourth = near.Joints[JointType.HandRight];
            Joint fifth = near.Joints[JointType.WristRight];
            
            if(!hasPointed && first.Position.Z - third.Position.Z > 0.15)
            {
                float averagePosition = (third.Position.X + third.Position.Y + third.Position.Z)/3;

                if (Math.Abs(averagePosition - lastAveragePosition) < 0.015f)
                {
                    frameCounter++;
                }
                else if (frameCounter == 150)
                {
                    isPointing = true;
                    frameCounter = 0;
                    hasPointed = true;
                }
                else
                {
                    frameCounter = 0;
                    lastAveragePosition = averagePosition;
                }  
            } else
            {
                hasPointed = false;
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

        /// <summary>
        /// Handles the event which the sensor becomes unavailable (E.g. paused, closed, unplugged).
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
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
