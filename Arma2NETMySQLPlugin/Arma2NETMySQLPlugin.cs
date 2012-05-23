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
    [AddIn("Arma2NETMySQL")] //the function name for the plugin (called from Arma side)
    public class Arma2NETMySQLPlugin : Arma2NetAddIn
    {
        Logger logger_object = null;

        const string version = "0.1";

        Databases dbs = null;

        //This method is called when callExtension is used from SQF:
        //"Arma2Net.Unmanaged" callExtension "Arma2NetMySQL ..."
        public override string Run(string args)
        {
            IList<object> arguments;
            if (Format.SqfAsCollection(args, out arguments) && arguments.Count >= 2 && arguments[0] != null && arguments[1] != null)
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

                if (dbs.SQLProviderExists(database))
                {
                    IEnumerable<string[]> returned = dbs.getSQLProvider(database).RunProcedure(procedure, split.ToArray());
                    Logger.addMessage(Logger.LogType.Info, "Returning to Arma2: " + returned.ToString());
                    return Arma2Net.Managed.Format.ObjectAsSqf(returned);
                }
                else
                {
                    Logger.addMessage(Logger.LogType.Warning, "The database: " + database + " is not loaded in through the Databases.txt file.");
                }

                return Arma2Net.Managed.Format.ObjectAsSqf(false);
            }
            else
            {
                throw new FunctionNotFoundException();
            }
        }

        public Arma2NETMySQLPlugin()
        {
            //constructor

            //Start up logging
            logger_object = new Logger();

            Logger.addMessage(Logger.LogType.Info, "Arma2NETMySQL Plugin Started.");
            Logger.addMessage(Logger.LogType.Info, "Version number: " + version);

            //Load in Databases.txt file
            //This also sets up the SQLProvider associated with the database
            Logger.addMessage(Logger.LogType.Info, "Loading databases...");
            dbs = new Databases();
        }
    }
}
