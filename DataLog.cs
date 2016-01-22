using System;
using System.Diagnostics;
using System.IO;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    class DataLog
    {

        public static string LOGFILE = "Logs/logfile.txt";
        public enum DebugLevel
        {
            Error = 1,
            Warning = 2,
            Message = 3
        }

        public static void Log (DebugLevel level, string message)
        {
            ToConsole(level, message);
            ToFile(level, message);
        }

        public static void ToConsole(DebugLevel level, string message)
        {
            Debugger.Log(1, level.ToString(),
                message + "\n");
        }

        public static void ToFile(DebugLevel level, string message)
        {
            Directory.CreateDirectory("Logs");
            StreamWriter file = new StreamWriter(
                LOGFILE, 
                File.Exists(LOGFILE));

            file.WriteLine("[" + DateTime.Now.ToString() + "] " + 
                level.ToString() + ": " + message + "\n");

            file.Close();
        }
    }
}
