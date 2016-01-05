using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Kinect.Input;
using Microsoft.Kinect.Wpf.Controls;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    /// <summary>
    /// Logica di interazione per CalibrationWindow.xaml
    /// </summary>
    public partial class CalibrationWindow : Window
    {
        private const float STILL_THRESHOLD = 0.03f;
        private const int COUNTER_THRESHOLD = 60;
        public MainWindow win;
        private KinectSensor sensor;
        private BodyFrameReader bodyReader;
        private UiTools uitools;
        private Body[] bodies;

        public CalibrationWindow(MainWindow win, KinectSensor sens)
        {
            InitializeComponent();
            this.uitools = new UiTools(win);
            this.win = win;
            sensor = sens;
            this.bodyReader = sensor.BodyFrameSource.OpenReader();
            this.bodyReader.FrameArrived += Calibration_FrameArrived;
            this.Closing += CalibrationWindow_Closing;
        }

        private void CalibrationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.bodyReader != null)
            {
                // BodyFrameReader is IDisposable
                this.bodyReader.Dispose();
                this.bodyReader = null;
            }
        }

        private void Calibration_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
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

                    Body near = uitools.getNearestBody(this.bodies);

                    if (uitools.checkPointingRight(near, STILL_THRESHOLD, COUNTER_THRESHOLD))
                    {
                        float pointedX = uitools.calculateX(near.Joints[JointType.ShoulderRight].Position, near.Joints[JointType.HandTipRight].Position);
                        float pointedY = uitools.calculateY(near.Joints[JointType.ShoulderRight].Position, near.Joints[JointType.HandTipRight].Position);
                        win.offsetX = pointedX;
                        win.offsetY = pointedY;
                        this.Close();
                    }
                    else if (uitools.checkPointingLeft(near, STILL_THRESHOLD, COUNTER_THRESHOLD))
                    {
                        float pointedX = uitools.calculateX(near.Joints[JointType.ShoulderLeft].Position, near.Joints[JointType.HandTipLeft].Position);
                        float pointedY = uitools.calculateY(near.Joints[JointType.ShoulderLeft].Position, near.Joints[JointType.HandTipLeft].Position);
                        win.offsetX = pointedX;
                        win.offsetY = pointedY;
                        this.Close();
                    }
                }
            }
        }

    }
}
