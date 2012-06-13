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
            //With new (6.5.4) versions of the MySQL Connector, it was throwing an error when running the stored procedures:
            //Unable to retrieve stored procedure metadata for routine.  Either grant  SELECT privilege to mysql.proc for this user or use "check parameters=false" with  your connection string.
            //Therefore, check parameters is now false
            //http://lists.mysql.com/commits/136669
            /*  "We have made the Use Procedure Bodies flag obsolete and introduced the Check Parameters option.  They server mainly the
                same purpose.  The Check Parameters option is true by default.  Setting to false tells
                Connector/Net to not fetch any routine or parameter metadata and to simply trust what the user has specified.  This can greatly
                increase performance but it also puts significant pressure on the application developer to give the parameters in the right
                order."
             */
            return "Server = " + ipaddress + "; Port = " + port + "; Database = " + databasename + "; Uid = " + username + "; Pwd = " + password + ";CheckParameters=false";
        }
    }
}
