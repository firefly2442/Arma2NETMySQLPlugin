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


namespace Arma2NETMySQLPlugin
{
    class DatabaseObject
    {
        public string type;
        public string databasename;
        public string ipaddress;
        public string username;
        public string password;
        public string port;

        public SQL sql_connection;

        public DatabaseObject(params string[] values)
        {
            //Constructor
            type = values[0];
            databasename = values[1];
            if (values.Length == 6) {
                ipaddress = values[2];
                port = values[3];
                username = values[4];
                password = values[5];
            }

            if (type == "mysql")
            {
                sql_connection = new MySQL();
                sql_connection.OpenConnection(getMySQLFormattedConnectionString());
            }
            else if (type == "sqlite")
            {
                sql_connection = new SQLite();
                sql_connection.OpenConnection(databasename);
            }
            else
            {
                Logger.addMessage(Logger.LogType.Error, "Database type does not match one of the supported types.");
            }
        }

        private string getMySQLFormattedConnectionString()
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
