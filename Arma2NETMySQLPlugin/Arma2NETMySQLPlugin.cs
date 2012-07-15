using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.AddIn;
using AddInView;
using Arma2Net.Managed;

namespace Arma2NETMySQLPlugin
{
    //the function name for the plugin (called from Arma side)
    [AddIn("Arma2NETMySQL", Version = "0.1.0.0", Publisher = "firefly2442", Description = "Runs MySQL procedure commands.")]
    public class Arma2NETMySQLPlugin : Arma2NetLongOutputAddIn
    {
        //This method is called when callExtension is used from SQF:
        //"Arma2Net.Unmanaged" callExtension "Arma2NetMySQL ..."
        public override string Run(string args, int maxResultSize)
        {
            //if we haven't setup the database connection and such yet, this will do it
            Startup.StartupConnection();

            IList<object> arguments;
            if (Format.TrySqfAsCollection(args, out arguments) && arguments.Count >= 2 && arguments[0] != null && arguments[1] != null)
            {
                string database = arguments[0] as string;
                string procedure = arguments[1] as string;
                string parameters = arguments[2] as string;
                //strip out [] characters at the beginning and end
                if (parameters[0].ToString() == "[" && parameters[parameters.Length - 1].ToString() == "]")
                {
                    parameters = parameters.Substring(1, parameters.Length - 2);
                }
                List<string> split = new List<string>();
                if (parameters != null)
                {
                    split = parameters.Split(',').ToList<string>();
                }

                Logger.addMessage(Logger.LogType.Info, "Received - Database: " + database + " Procedure: " + procedure + " Parameters: " + parameters.ToString());

                if (MySQL.dbs.SQLProviderExists(database))
                {
                    IEnumerable<string[][]> returned = MySQL.dbs.getSQLProvider(database).RunProcedure(procedure, split.ToArray(), maxResultSize);
                    return Arma2Net.Managed.Format.ObjectAsSqf(returned);
                }
                else
                {
                    Logger.addMessage(Logger.LogType.Warning, "The database: " + database + " is not loaded in through the Databases.txt file.");
                }

                //Logger.addMessage(Logger.LogType.Info, "Returning false object");
                return Arma2Net.Managed.Format.ObjectAsSqf(false);
            }
            else
            {
                Logger.addMessage(Logger.LogType.Error, "The number and/or format of the arguments passed in doesn't match.");
                throw new ArgumentException();
            }
        }
    }

    //the function name for the plugin (called from Arma side)
    [AddIn("Arma2NETMySQLCommand", Version = "0.1.0.0", Publisher = "firefly2442", Description = "Runs raw MySQL commands")]
    public class Arma2NETMySQLPluginCommand : Arma2NetLongOutputAddIn
    {
        //This method is called when callExtension is used from SQF:
        //"Arma2Net.Unmanaged" callExtension "Arma2NetMySQLCommand ..."
        public override string Run(string args, int maxResultSize)
        {
            //if we haven't setup the database connection and such yet, this will do it
            Startup.StartupConnection();

            IList<object> arguments;
            if (Format.TrySqfAsCollection(args, out arguments) && arguments.Count == 2 && arguments[0] != null && arguments[1] != null)
            {
                string database = arguments[0] as string;
                string mysql_command = arguments[1] as string;

                Logger.addMessage(Logger.LogType.Info, "Received - Database: " + database + " MySQL Command: " + mysql_command.ToString());

                if (MySQL.dbs.SQLProviderExists(database))
                {
                    IEnumerable<string[][]> returned = MySQL.dbs.getSQLProvider(database).RunCommand(mysql_command, maxResultSize);
                    return Arma2Net.Managed.Format.ObjectAsSqf(returned);
                }
                else
                {
                    Logger.addMessage(Logger.LogType.Warning, "The database: " + database + " is not loaded in through the Databases.txt file.");
                }

                //Logger.addMessage(Logger.LogType.Info, "Returning false object");
                return Arma2Net.Managed.Format.ObjectAsSqf(false);
            }
            else
            {
                Logger.addMessage(Logger.LogType.Error, "The number and/or format of the arguments passed in doesn't match.");
                throw new ArgumentException();
            }
        }
    }
}
