using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Arma2NETMySQLPlugin
{
    class Logger
    {
        public enum loggerState
        {
            Started,
            Stopped
        }

        public enum LogType
        {
            Info,
            Warning,
            Error
        }

        private static loggerState state = loggerState.Stopped;
        public static loggerState State { get { return state; } }

        private static FileStream fs = null;
        private static StreamWriter sw = null;

        public Logger()
        {
            //Constructor
            if (State == loggerState.Stopped)
            {
                //check to see if the logs folder exists, if not create it
                var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Arma2MySQL/logs/");
                if (!System.IO.Directory.Exists(logDir)) {
                    System.IO.Directory.CreateDirectory(logDir);
                }

                //Setup file streams
                DateTime dateValue = new DateTime();
                dateValue = DateTime.Now;
                string fullpath = Path.Combine(logDir, dateValue.ToString("MM-dd-yyyy_HH-mm-ss") + ".log");
                fs = new FileStream(fullpath, FileMode.Append);
                sw = new StreamWriter(fs);

                state = loggerState.Started;
            }
        }

        public static void addMessage(LogType type, string message)
        {
            if (State == loggerState.Started)
            {
                DateTime time = new DateTime();
                time = DateTime.Now;
                string towrite = type.ToString() + ": " + time.ToString("HH:mm:ss - ") + message;

                //This locks file writing for a second, this prevents multiple external threads to be writing to the file
                //using this method at exactly the same time.
                //http://msdn.microsoft.com/en-us/library/c5kehkcz.aspx
                //http://www.dotnetperls.com/lock
                lock (sw)
                {
                    sw.WriteLine(towrite);
                    sw.Flush();
                }
            }
            else
            {
                Console.WriteLine("ERROR: Tried to add message when logger is down.\n**\t{0}", message);
            }
        }

        public static void Stop()
        {
            if (State == loggerState.Started)
            {
                try
                {
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                    state = loggerState.Stopped;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("EXCEPTION: An exception occured while stopping the logger.\n**\t{0}", ex.ToString());
                }
            }
        }
    }
}
