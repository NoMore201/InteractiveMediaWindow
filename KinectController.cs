using Microsoft.Kinect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Samples.Kinect.BodyBasics
{

    public enum ArmPointing
    {
        Right = 1,
        Left = 2,
        Nothing = 3
    }

    class KinectController
    {
        public static float ACTIVATION_THRESHOLD = 0.5f;

        private const float STILL_THRESHOLD = 0.04f;
        private const int COUNTER_THRESHOLD = 30;
        private const float BORDERX_THRESHOLD = 0.1f;
        private const float BORDERY_THRESHOLD = 0f;
        private const float MAXX = 1.5f;
        private const float MAXY = 1f;
        private const string OPT_FILE = "calibration.json";

        public KinectSensor kinectSensor;
        public BodyFrameReader bodyReader;
        public Body[] bodies;
        public Body nearest;

        //parameters
        public ArmPointing Arm;

        public Joint ShoulderLeft;
        public Joint ElbowLeft;
        public Joint HandTipLeft;
        public Joint HandLeft;
        public Joint WristLeft;

        public Joint ShoulderRight;
        public Joint ElbowRight;
        public Joint HandTipRight;
        public Joint HandRight;
        public Joint WristRight;

        public float lastAveragePositionRight;
        public float lastAveragePositionLeft;
        public int frameCounterRight;
        public int frameCounterLeft;
        public ulong indexNearest;

        private bool hasPointed;

        public float offsetX, offsetY;

        public KinectController()
        {
            kinectSensor = KinectSensor.GetDefault();

            // open the reader for the body frames
            bodyReader = kinectSensor.BodyFrameSource.OpenReader();
            kinectSensor.Open();

            Arm = ArmPointing.Nothing;
            hasPointed = false;
            lastAveragePositionLeft = 0f;
            lastAveragePositionRight = 0f;
            frameCounterLeft = 0;
            frameCounterRight = 0;

            if (!File.Exists(OPT_FILE))
            {
                offsetX = 0;
                offsetY = 0;
            } else
            {
                string data = File.ReadAllText(OPT_FILE);
                List <float> offset = JsonConvert.DeserializeObject<List<float>>(data);
                offsetX = offset[0];
                offsetY = offset[1];
            }
        }

        public void Controller_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
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

                    nearest = getNearestBody(this.bodies);

                    // populate all variables for right
                    ShoulderRight = nearest.Joints[JointType.ShoulderRight];
                    ElbowRight = nearest.Joints[JointType.ElbowRight];
                    HandTipRight = nearest.Joints[JointType.HandTipRight];
                    HandRight = nearest.Joints[JointType.HandRight];
                    WristRight = nearest.Joints[JointType.WristRight];

                    // populate all variables for left
                    ShoulderLeft = nearest.Joints[JointType.ShoulderLeft];
                    ElbowLeft = nearest.Joints[JointType.ElbowLeft];
                    HandTipLeft = nearest.Joints[JointType.HandTipLeft];
                    HandLeft = nearest.Joints[JointType.HandLeft];
                    WristLeft = nearest.Joints[JointType.WristLeft];

                    if(!checkPointingLeft())
                        checkPointingRight();
                }
            }
        }


        public bool checkPointingRight()
        {
            bool isPointing = false;
            if (!hasPointed && nearest.HandRightState != HandState.Closed &&
                nearest.HandRightState != HandState.Open &&
                (ShoulderRight.Position.Z - HandRight.Position.Z) +
                Math.Abs(ShoulderRight.Position.X - HandRight.Position.X) > ACTIVATION_THRESHOLD)
            {
                float averagePosition = (HandRight.Position.X + HandRight.Position.Y) / 2;

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
                    Arm = ArmPointing.Right;
                }

            }
            else if ((ShoulderRight.Position.Z - HandRight.Position.Z) +
                Math.Abs(ShoulderRight.Position.X - HandRight.Position.X) < ACTIVATION_THRESHOLD)
            {
                hasPointed = false;
                frameCounterRight = 0;
                Arm = ArmPointing.Nothing;
            }

            if (frameCounterRight > COUNTER_THRESHOLD)
            {
                isPointing = true;
                Arm = ArmPointing.Right;
            }

            return isPointing;
        }

        public bool checkPointingLeft()
        {
            bool isPointing = false;

            if (!hasPointed && nearest.HandLeftState != HandState.Closed &&
                nearest.HandLeftState != HandState.Open &&
                (ShoulderLeft.Position.Z - HandLeft.Position.Z) + 
                Math.Abs(ShoulderLeft.Position.X - HandLeft.Position.X) > ACTIVATION_THRESHOLD)
            {
                float averagePosition = (HandLeft.Position.X + HandLeft.Position.Y) / 2;

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
                    Arm = ArmPointing.Left;
                }

            }
            else if ((ShoulderLeft.Position.Z - HandLeft.Position.Z) + 
                Math.Abs(ShoulderLeft.Position.X - HandLeft.Position.X) < ACTIVATION_THRESHOLD)
            {
                hasPointed = false;
                frameCounterLeft = 0;
                Arm = ArmPointing.Nothing;
            }

            if (frameCounterLeft > COUNTER_THRESHOLD)
            {
                isPointing = true;
                Arm = ArmPointing.Left;
            }

            return isPointing;
        }

        public float GetPointedX()
        {
            if (Arm == ArmPointing.Right)
            {
                float pointedX = calculateX(ShoulderRight.Position,
                    HandTipRight.Position) - offsetX;
                return pointedX;
            }
            else if (Arm == ArmPointing.Left)
            {
                float pointedX = calculateX(ShoulderLeft.Position,
                    HandTipLeft.Position) - offsetX;
                return pointedX;
            }
            return 0f;
        }

        public float GetPointedY()
        {
            if (Arm == ArmPointing.Right)
            {
                float pointedY = calculateY(ShoulderRight.Position,
                    HandTipRight.Position) - offsetY;
                return pointedY;
            }
            else if (Arm == ArmPointing.Left)
            {
                float pointedY = calculateY(ShoulderLeft.Position,
                    HandTipLeft.Position) - offsetY;
                return pointedY;
            }
            return 0f;
        }

        public int GetPointedZone()
        {
            if (Arm != ArmPointing.Nothing) {
                float pointedX = GetPointedX();
                float pointedY = GetPointedY();
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
            return 0;
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
            int counter = 0;
            foreach (Body b in bodies)
            {
                if (b.IsTracked)
                {
                    if (!nearest.IsTracked)
                    {
                        nearest = b;
                        indexNearest = b.TrackingId;
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
                            indexNearest = b.TrackingId;
                        }
                    }

                }
                counter++;
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

        public void Reset()
        {
            Arm = ArmPointing.Nothing;
            hasPointed = false;
            lastAveragePositionLeft = 0f;
            lastAveragePositionRight = 0f;
            frameCounterLeft = 0;
            frameCounterRight = 0;
        }

    }
}
