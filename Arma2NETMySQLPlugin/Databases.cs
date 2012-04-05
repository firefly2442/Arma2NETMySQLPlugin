using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; //for reading file

namespace Arma2NETMySQLPlugin
{
    class Databases
    {
        private List<DatabaseObject> databaseList = new List<DatabaseObject>();

        public Databases()
        {
            //Constructor

            //Load the database names, ip, port, usernames, passwords, and so on from file
            string line;
            if (!File.Exists("Databases.txt"))
            {
                Logger.addMessage(Logger.LogType.Error, "Unable to find the Databases.txt file, are you sure it's there?");
            }

            StreamReader sr = File.OpenText("Databases.txt");
            line = sr.ReadLine();
            while (line != null)
            {
                //Make sure it's not a comment
                if (!line.StartsWith("#"))
                {
                    //Separate out the information
                    string[] split = line.Split(',');
                    if (split.Length == 5)
                    {
                        Logger.addMessage(Logger.LogType.Info, "Database: " + split[0] + " IPAddress: " + split[1] + " Port: " + split[2] + " Username: " + split[3] + " Password: NotShownForSecurityReasons");
                        DatabaseObject temp = new DatabaseObject(split[0], split[1], split[2], split[3], split[4]);                        
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
                databaseList[i].mysql_connection.CloseConnection();
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

        public MySQL getSQLProvider(string dbname)
        {
            for (int i = 0; i < databaseList.Count(); i++)
            {
                if (databaseList[i].databasename == dbname)
                {
                    return databaseList[i].mysql_connection;
                }
            }
            return null;
        }
    }
}
