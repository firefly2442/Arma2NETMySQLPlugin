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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using Arma2Net;

namespace Arma2NETMySQLPlugin
{
    //the function name for the plugin (called from Arma side)
    [Addin("Arma2NETMySQL", Version = "0.1.0.0", Author = "firefly2442", Description = "Runs MySQL procedure commands.")]
    public class Arma2NETMySQLPlugin : Addin
    {
        //This method is called when callExtension is used from SQF:
        //"Arma2NET" callExtension "<Arma2NetMySQL> [arguments]"
        public override string Invoke(string args, int maxResultSize)
        {
            //if we haven't setup the database connection and such yet, this will do it
            Startup.StartupConnection();

            IList<object> arguments;
            if (Format.TrySqfAsCollection(args, out arguments) && arguments.Count >= 2 && arguments[0] != null && arguments[1] != null)
            {
                string database = arguments[0] as string;
                string procedure = arguments[1] as string;
                string parameters = arguments[2] as string;
                parameters = Util.stripSquareBrackets(parameters);

                List<string> split = new List<string>();
                if (parameters != null)
                {
                    split = parameters.Split(',').ToList<string>();
                }

                Logger.addMessage(Logger.LogType.Info, "Received - Database: " + database + " Procedure: " + procedure + " Parameters: " + parameters.ToString());

                if (SQL.dbs.SQLProviderExists(database))
                {
                    IEnumerable<string[][]> returned = SQL.dbs.getSQLProvider(database).RunProcedure(procedure, split.ToArray(), maxResultSize);
                    return Format.ObjectAsSqf(returned);
                }
                else
                {
                    Logger.addMessage(Logger.LogType.Warning, "The database: " + database + " is not loaded in through the Databases.config file.");
                }
            }
            else
            {
                Logger.addMessage(Logger.LogType.Error, "The number and/or format of the arguments passed in to Arma2NETMySQL doesn't match.");
            }
            return Format.ObjectAsSqf("Error");
        }
    }

    //the function name for the plugin (called from Arma side)
    [Addin("Arma2NETMySQLCommand", Version = "0.1.0.0", Author = "firefly2442", Description = "Runs raw MySQL/SQLite commands")]
    public class Arma2NETMySQLPluginCommand : Addin
    {
        //This method is called when callExtension is used from SQF:
        //"Arma2NET" callExtension "<Arma2NetMySQL> [arguments]"
        public override string Invoke(string args, int maxResultSize)
        {
            //if we haven't setup the database connection and such yet, this will do it
            Startup.StartupConnection();

            IList<object> arguments;
            if (Format.TrySqfAsCollection(args, out arguments) && arguments.Count == 2 && arguments[0] != null && arguments[1] != null)
            {
                string database = arguments[0] as string;
                string sql_command = arguments[1] as string;

                Logger.addMessage(Logger.LogType.Info, "Received - Database: " + database + " SQL Query: " + sql_command);

                if (SQL.dbs.SQLProviderExists(database))
                {
                    IEnumerable<string[][]> returned = SQL.dbs.getSQLProvider(database).RunCommand(sql_command, maxResultSize);
                    return Format.ObjectAsSqf(returned);
                }
                else
                {
                    Logger.addMessage(Logger.LogType.Warning, "The database: " + database + " is not loaded in through the Databases.config file.");
                }
            }
            else
            {
                Logger.addMessage(Logger.LogType.Error, "The number and/or format of the arguments passed in to Arma2NETMySQLCommand doesn't match.");
            }
            return Format.ObjectAsSqf("Error");
        }
    }

    //the function name for the plugin (called from Arma side)
    [Addin("Arma2NETMySQLCommandAsync", Version = "0.1.0.0", Author = "firefly2442", Description = "Runs asynchronous raw MySQL/SQLite commands")]
    public class Arma2NETMySQLPluginCommandAsync : Addin
    {
        public Arma2NETMySQLPluginCommandAsync()
        {
            InvocationMethod = new AsyncAddinInvocationMethod(this);
        }

        //AsyncAddIn - when you want to pass data from the game and immediately return null
        // then, subsequent checks by the game check to see if the data can be returned.
        //On the SQF side, this means that we can only do one call at a time...

        //This method is called when callExtension is used from SQF:
        //"Arma2NET" callExtension "<Arma2NetMySQL> [arguments]"
        public override string Invoke(string args, int maxResultSize)
        {
            //see AsyncTestAddin.cs in Arma2NET for an async example
            if (args != null)
            {
                //if we haven't setup the database connection and such yet, this will do it
                Startup.StartupConnection();

                IList<object> arguments;
                if (Format.TrySqfAsCollection(args, out arguments) && arguments.Count == 2 && arguments[0] != null && arguments[1] != null)
                {
                    string database = arguments[0] as string;
                    string sql_command = arguments[1] as string;

                    Logger.addMessage(Logger.LogType.Info, "Received - Database: " + database + " SQL Query: " + sql_command);

                    if (SQL.dbs.SQLProviderExists(database))
                    {
                        IEnumerable<string[][]> returned = SQL.dbs.getSQLProvider(database).RunCommand(sql_command, maxResultSize);
                        //the following is needed because we need to return something even if there is nothing to return
                        //for example, an SQL DELETE call will go off and return ""
                        //however, because on the SQF side, we check for this in a while loop so we know the database process has completed, we can
                        //just return an empty array
                        if (returned.ToString() == "")
                        {
                            //Logger.addMessage(Logger.LogType.Info, "Returning: []");
                            return Format.ObjectAsSqf("[]");
                        }
                        //Logger.addMessage(Logger.LogType.Info, "Returning: " + Format.ObjectAsSqf(returned));
                        return Format.ObjectAsSqf(returned);
                    }
                    else
                    {
                        Logger.addMessage(Logger.LogType.Warning, "The database: " + database + " is not loaded in through the Databases.config file.");
                    }
                }
                else
                {
                    Logger.addMessage(Logger.LogType.Error, "The number and/or format of the arguments passed in to Arma2NETMySQLCommandAsync doesn't match.");
                }
            }
            return Format.ObjectAsSqf("Error");
        }
    }
}
