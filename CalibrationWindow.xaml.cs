using System.Windows;
using Microsoft.Kinect;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    /// <summary>
    /// Logica di interazione per CalibrationWindow.xaml
    /// </summary>
    public partial class CalibrationWindow : Window
    {
        private KinectController kc;

        private const string OPT_FILE = "calibration.json";

        public CalibrationWindow()
        {
            InitializeComponent();
            kc = new KinectController();
            kc.bodyReader.FrameArrived += Calibration_FrameArrived;
        }

        private void Calibration_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            kc.Controller_FrameArrived(sender, e);

            if (kc.Arm != ArmPointing.Nothing)
            {
                float pointedX = kc.GetPointedX();
                float pointedY = kc.GetPointedY();

                DataLog.ToConsole(pointedX.ToString() + " " + pointedY.ToString());

                if (!File.Exists(OPT_FILE))
                    File.Create(OPT_FILE).Close();

                List<float> l = new List<float>();

                l.Add(pointedX);
                l.Add(pointedY);

                StreamWriter cal_file = File.CreateText(OPT_FILE);
                JsonTextWriter cal_writer = new JsonTextWriter(cal_file);
                string data = JsonConvert.SerializeObject(l);
                cal_writer.WriteRaw(data);
                cal_file.Close();

                this.Close();
            }
        }
    }
}
