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
using Arma2Net.AddInProxy;
using System.IO;

namespace Arma2NETMySQLPlugin
{
    class Startup
    {
        public static Logger logger_object = null;
        public static Boolean started_up = false;

        public static void StartupConnection()
        {
            if (started_up == false)
            {
                //create appdata folder if it doesn't already exist
                var appDataLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Arma2NETMySQL");
                //check to see if the Arma2NETMySQL folder exists, if not create it
                if (!System.IO.Directory.Exists(appDataLocation)) {
                    System.IO.Directory.CreateDirectory(appDataLocation);
                }

                //Start up logging
                logger_object = new Logger();
                Logger.addMessage(Logger.LogType.Info, "Logging started in directory: " + Logger.getLogDir());

                Logger.addMessage(Logger.LogType.Info, "Arma2NETMySQL Plugin Started.");

                //Use AssemblyInfo.cs version number
                //Holy cow this is confusing...
                //http://stackoverflow.com/questions/909555/how-can-i-get-the-assembly-file-version
                //http://all-things-pure.blogspot.com/2009/09/assembly-version-file-version-product.html
                //http://stackoverflow.com/questions/64602/what-are-differences-between-assemblyversion-assemblyfileversion-and-assemblyin
                Logger.addMessage(Logger.LogType.Info, "Version number: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

                //Utils.FileVersion returns the current version that is running
                //Utils.Version returns the API version, when this changes, all plugins need to be recompiled
                //This version call doesn't always seem to play nicely (it causes errors where it can't find the managed.dll)
                //Logger.addMessage(Logger.LogType.Info, "Compiled against Arma2NET Version: " + Utils.FileVersion);

                //Load in Databases.txt file
                //This also sets up the SQLProvider associated with the database
                Logger.addMessage(Logger.LogType.Info, "Loading databases...");
                SQL.dbs = new Databases();

                //set mutex so we know we've started everything up
                started_up = true;
            }
        }

        public static void Unload()
        {
            Logger.addMessage(Logger.LogType.Info, "Unloading plugin.");
            SQL.dbs.shutdown();
            Logger.Stop();
            Startup.started_up = false;
        }
    }
}
