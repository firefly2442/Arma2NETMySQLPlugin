/*
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/


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

        private static string logDir = null;

        public Logger()
        {
            //Constructor
            if (State == loggerState.Stopped)
            {
                //check to see if the logs folder exists, if not create it
                //check the Arma2 root directory first
                if (System.IO.Directory.Exists("logs"))
                {
                    logDir = Path.GetFullPath("logs/");
                }
                else
                {
                    logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Arma2NETMySQL/logs/");
                    if (!System.IO.Directory.Exists(logDir))
                    {
                        System.IO.Directory.CreateDirectory(logDir);
                    }
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

        public static string getLogDir()
        {
            return logDir;
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
