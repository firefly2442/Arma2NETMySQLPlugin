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
using System.IO; //for reading file

namespace Arma2NETMySQLPlugin
{
    public class Databases
    {
        private List<DatabaseObject> databaseList = new List<DatabaseObject>();
        private List<Aliases> aliasList = new List<Aliases>();

        public Databases()
        {
            //Constructor

            //Load the database names, ip, port, usernames, passwords, and so on from file
            string line;
            string databasesFileLocation = null;
            //check the Arma2 root directory first
            if (File.Exists("Databases.config")) {
                databasesFileLocation = Path.GetFullPath("Databases.config");
            } else {
                Logger.addMessage(Logger.LogType.Warning, "Unable to find the Databases.config file here: " + Path.GetFullPath("Databases.config"));
                databasesFileLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Arma2NETMySQL/Databases.config");
                if (!File.Exists(databasesFileLocation))
                {
                    Logger.addMessage(Logger.LogType.Error, "Unable to find the Databases.config here: " + databasesFileLocation);
                }
            }
            Logger.addMessage(Logger.LogType.Info, "Databases.config file loading in from: " + databasesFileLocation);

            StreamReader sr = File.OpenText(databasesFileLocation);
            line = sr.ReadLine();
            while (line != null)
            {
                //Make sure it's not a comment
                if (!line.StartsWith("#"))
                {
                    //Separate out the information
                    string[] split = line.Split(',');
                    if (split.Length == 6) //mysql
                    {
                        //check to make sure the "root" user is not being used
                        if (split[4].ToLower() == "root") {
                            Logger.addMessage(Logger.LogType.Error, "Using 'root' as a user is not allowed for security purposes.");
                        } else {
                            split[0] = split[0].ToLower();
                            Logger.addMessage(Logger.LogType.Info, "Type: " + split[0] + " Database: " + split[1] + " IPAddress: " + split[2] + " Port: " + split[3] + " Username: " + split[4] + " Password: NotShownForSecurityReasons");
                            DatabaseObject temp = new DatabaseObject(new string[6] { split[0], split[1], split[2], split[3], split[4], split[5] });
                            databaseList.Add(temp);
                        }
                    }
                    else if (split.Length == 2) //sqlite
                    {
                        split[0] = split[0].ToLower();
                        Logger.addMessage(Logger.LogType.Info, "Type: " + split[0] + " Database: " + split[1]);
                        DatabaseObject temp = new DatabaseObject(new string[2] {split[0], split[1]});
                        databaseList.Add(temp);
                    }
                    else if (split.Length == 3) //alias
                    {
                        Logger.addMessage(Logger.LogType.Info, "Adding database alias from: " + split[1] + " to: " + split[2]);
                        Aliases aliasObj = new Aliases(split[1], split[2]);
                        aliasList.Add(aliasObj);
                    }
                    else if (line.Contains(","))
                    {
                        Logger.addMessage(Logger.LogType.Error, "Unable to parse line: " + line + " in Databases.config file.");
                    }
                }
                line = sr.ReadLine();
            }
            sr.Close();
        }

        public int getSize()
        {
            return databaseList.Count();
        }

        public void shutdown()
        {
            for (int i = 0; i < databaseList.Count(); i++)
            {
                databaseList[i].sql_connection.CloseConnection();
            }
        }

        public bool SQLProviderExists(string dbname)
        {
            for (int i = 0; i < aliasList.Count(); i++)
            {
                if (aliasList[i].from == dbname)
                {
                    return true;
                }
            }
            for (int i = 0; i < databaseList.Count(); i++)
            {
                if (databaseList[i].databasename == dbname)
                {
                    return true;
                }
            }
            return false;
        }

        public SQL getSQLProvider(string dbname)
        {
            //check the aliases first
            for (int i = 0; i < aliasList.Count(); i++)
            {
                if (aliasList[i].from == dbname)
                {
                    dbname = aliasList[i].to;
                    break;
                }
            }
            for (int i = 0; i < databaseList.Count(); i++)
            {
                if (databaseList[i].databasename == dbname)
                {
                    return databaseList[i].sql_connection;
                }
            }
            return null;
        }
    }
}
