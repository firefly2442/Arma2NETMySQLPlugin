using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arma2NETMySQLPlugin
{
    class DatabaseObject
    {
        public string databasename;
        public string ipaddress;
        public string username;
        public string password;
        public string port;

        public MySQL mysql_connection;

        public DatabaseObject(string name, string ip, string p, string user, string pass)
        {
            //Constructor
            databasename = name;
            ipaddress = ip;
            port = p;
            username = user;
            password = pass;

            mysql_connection = new MySQL();
            mysql_connection.OpenConnection(getFormattedConnectionString());
        }

        private string getFormattedConnectionString()
        {
            return "Server = " + ipaddress + "; Port = " + port + "; Database = " + databasename + "; Uid = " + username + "; Pwd = " + password + ";";
        }
    }
}
