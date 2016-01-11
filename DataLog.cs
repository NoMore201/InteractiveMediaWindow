using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    class DataLog
    {
        public static void ToConsole(String message)
        {
            Debugger.Log(1, "INTERACTIVEMEDIAWINDOW: ", message);
        }

        public static void ToFile(String message)
        {
            StreamWriter file = new StreamWriter(
                "..\\..\\..\\DataLogs.txt", 
                File.Exists("..\\..\\..\\DataLogs.txt"
            ));

            file.WriteLine("[" + DateTime.Now.ToString() + "]" + message);

            file.Close();
        }
    }
}
