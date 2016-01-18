using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Microsoft.Kinect.Input;
using Microsoft.Kinect.Wpf.Controls;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    class UiTools
    {

        public bool hasPointed;
        public MainWindow win;
        public int frameCounterRight;
        public int frameCounterLeft;
        public float lastAveragePositionRight;
        public float lastAveragePositionLeft;

        public UiTools(MainWindow win)
        {
            this.hasPointed = false;
            this.win = win;
            this.lastAveragePositionRight = 0;
            this.lastAveragePositionLeft = 0;
            frameCounterRight = 0;
            frameCounterLeft = 0;
        }

        public void PopulateLeft(Body near)
        {
            Joint first = near.Joints[JointType.ShoulderRight];
            Joint second = near.Joints[JointType.ElbowRight];
            Joint third = near.Joints[JointType.HandTipRight];
            Joint fourth = near.Joints[JointType.HandRight];
            Joint fifth = near.Joints[JointType.WristRight];

            win.leftFirstJointX.Text = first.Position.X.ToString();
            win.leftFirstJointY.Text = first.Position.Y.ToString();
            win.leftFirstJointZ.Text = first.Position.Z.ToString();
            win.leftFirstJointTracked.Text = first.TrackingState.ToString();

            win.leftSecondJointX.Text = second.Position.X.ToString();
            win.leftSecondJointY.Text = second.Position.Y.ToString();
            win.leftSecondJointZ.Text = second.Position.Z.ToString();
            win.leftSecondJointTracked.Text = second.TrackingState.ToString();

            win.leftThirdJointX.Text = third.Position.X.ToString();
            win.leftThirdJointY.Text = third.Position.Y.ToString();
            win.leftThirdJointZ.Text = third.Position.Z.ToString();
            win.leftThirdJointTracked.Text = third.TrackingState.ToString();

            win.leftFourthJointX.Text = fourth.Position.X.ToString();
            win.leftFourthJointY.Text = fourth.Position.Y.ToString();
            win.leftFourthJointZ.Text = fourth.Position.Z.ToString();
            win.leftFourthJointTracked.Text = fourth.TrackingState.ToString();

            win.leftFifthJointX.Text = fifth.Position.X.ToString();
            win.leftFifthJointY.Text = fifth.Position.Y.ToString();
            win.leftFifthJointZ.Text = fifth.Position.Z.ToString();
            win.leftFifthJointTracked.Text = fifth.TrackingState.ToString();
        }

        public bool checkPointingRight(Body near, float still, float counter)
        {

            win.handstate.Text = "Right";
            bool isPointing = false;

            Joint first = near.Joints[JointType.ShoulderRight];

            Joint third = near.Joints[JointType.HandRight];
            win.spallamano.Text = ((first.Position.Z - third.Position.Z) + Math.Abs(first.Position.X - third.Position.X)).ToString();

            DataLog dl = new DataLog();

            if (!hasPointed && near.HandRightState != HandState.Closed &&
                near.HandRightState != HandState.Open &&
                (first.Position.Z - third.Position.Z) + Math.Abs(first.Position.X - third.Position.X) > KinectController.ACTIVATION_THRESHOLD)
            {
                float averagePosition = (third.Position.X + third.Position.Y) / 2;

                win.body.Text = Math.Abs(averagePosition - lastAveragePositionRight).ToString();

                if (Math.Abs(averagePosition - lastAveragePositionRight) < still)
                {
                    frameCounterRight++;
                }
                else
                {
                    frameCounterRight = 0;
                    lastAveragePositionRight = averagePosition;
                }
                if (frameCounterRight > counter)
                {
                    isPointing = true;
                    hasPointed = true;
                }

            }
            else if ((first.Position.Z - third.Position.Z) + Math.Abs(first.Position.X - third.Position.X) < KinectController.ACTIVATION_THRESHOLD)
            {
                hasPointed = false;
                frameCounterRight = 0;
            }

            if (frameCounterRight > counter)
            {
                isPointing = true;
            }

            return isPointing;
        }

        public bool checkPointingLeft(Body near, float still, float counter)
        {
            win.handstate.Text = "Left";
            bool isPointing = false;

            Joint first = near.Joints[JointType.ShoulderLeft];

            Joint third = near.Joints[JointType.HandLeft];

            win.spallamano.Text = ((first.Position.Z - third.Position.Z) + Math.Abs(first.Position.X - third.Position.X)).ToString();

            DataLog dl = new DataLog();


            if (!hasPointed && near.HandLeftState != HandState.Closed &&
                near.HandLeftState != HandState.Open &&
                (first.Position.Z - third.Position.Z) + Math.Abs(first.Position.X - third.Position.X) > KinectController.ACTIVATION_THRESHOLD)
            {
                float averagePosition = (third.Position.X + third.Position.Y) / 2;

                win.body.Text = Math.Abs(averagePosition - lastAveragePositionLeft).ToString();


                if (Math.Abs(averagePosition - lastAveragePositionLeft) < still)
                {
                    frameCounterLeft++;
                }
                else
                {
                    frameCounterLeft = 0;
                    lastAveragePositionLeft = averagePosition;
                }
                if (frameCounterLeft > counter)
                {
                    isPointing = true;
                    hasPointed = true;
                }

            }
            else if ((first.Position.Z - third.Position.Z) + Math.Abs(first.Position.X - third.Position.X) < KinectController.ACTIVATION_THRESHOLD)
            {
                hasPointed = false;
                frameCounterLeft = 0;
            }

            if (frameCounterLeft > counter)
            {
                isPointing = true;
            }

            return isPointing;
        }

        public float calculateX(CameraSpacePoint one, CameraSpacePoint two)
        {
            return one.X + ((one.Z * one.X - (one.Z * two.X)) / (two.Z - one.Z));
        }

        public float calculateY(CameraSpacePoint one, CameraSpacePoint two)
        {
            return one.Y + ((one.Z * one.Y - (one.Z * two.Y)) / (two.Z - one.Z));
        }

        public Body getNearestBody(Body[] bodies)
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

        public float getNearestPointZ(IReadOnlyDictionary<JointType, Joint> joints)
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
