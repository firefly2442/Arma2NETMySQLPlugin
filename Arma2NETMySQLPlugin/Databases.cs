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

        public Databases()
        {
            //Constructor

            //Load the database names, ip, port, usernames, passwords, and so on from file
            string line;
            var databasesFileLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Arma2MySQL/Databases.txt");
            if (!File.Exists(databasesFileLocation))
            {
                Logger.addMessage(Logger.LogType.Error, "Unable to find the Databases.txt file, are you sure it's there?");
            }

            StreamReader sr = File.OpenText(databasesFileLocation);
            line = sr.ReadLine();
            while (line != null)
            {
                //Make sure it's not a comment
                if (!line.StartsWith("#"))
                {
                    //Separate out the information
                    string[] split = line.Split(',');
                    if (split.Length == 6)
                    {
                        split[0] = split[0].ToLower();
                        Logger.addMessage(Logger.LogType.Info, "Type: " + split[0] + " Database: " + split[1] + " IPAddress: " + split[2] + " Port: " + split[3] + " Username: " + split[4] + " Password: NotShownForSecurityReasons");
                        DatabaseObject temp = new DatabaseObject(new string[6] {split[0], split[1], split[2], split[3], split[4], split[5]});
                        databaseList.Add(temp);
                    }
                    else if (split.Length == 2)
                    {
                        split[0] = split[0].ToLower();
                        Logger.addMessage(Logger.LogType.Info, "Type: " + split[0] + " Database: " + split[1]);
                        DatabaseObject temp = new DatabaseObject(new string[2] {split[0], split[1]});
                        databaseList.Add(temp);
                    }
                    else if (line.Contains(","))
                    {
                        Logger.addMessage(Logger.LogType.Error, "Unable to parse line: " + line + " in Databases.txt file.");
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
