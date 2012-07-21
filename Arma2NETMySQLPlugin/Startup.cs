using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddInView;
using Arma2Net.AddInProxy;

namespace Arma2NETMySQLPlugin
{
    class Startup
    {
        private static Logger logger_object = null;
        private static Boolean started_up = false;

        public static void StartupConnection()
        {
            if (started_up == false)
            {
                //Start up logging
                logger_object = new Logger();

                Logger.addMessage(Logger.LogType.Info, "Arma2NETMySQL Plugin Started.");

                //Use AssemblyInfo.cs version number
                //Holy cow this is confusing...
                //http://stackoverflow.com/questions/909555/how-can-i-get-the-assembly-file-version
                //http://all-things-pure.blogspot.com/2009/09/assembly-version-file-version-product.html
                //http://stackoverflow.com/questions/64602/what-are-differences-between-assemblyversion-assemblyfileversion-and-assemblyin
                Logger.addMessage(Logger.LogType.Info, "Version number: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

                Logger.addMessage(Logger.LogType.Info, "Compiled against Arma2NET Version: " + Utils.Version);

                //Load in Databases.txt file
                //This also sets up the SQLProvider associated with the database
                Logger.addMessage(Logger.LogType.Info, "Loading databases...");
                MySQL.dbs = new Databases();

                //set mutex so we know we've started everything up
                started_up = true;
            }
        }
    }
}
