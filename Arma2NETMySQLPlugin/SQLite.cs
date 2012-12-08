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
using System.Data.SQLite;
using System.Data;
using System.IO;

namespace Arma2NETMySQLPlugin
{
    public class SQLite : SQL
    {
        private SQLiteConnection sqlite_connection;
        private string sqliteDatabaseLocation = null;

        public SQLite()
        {
            // Default constructor for the derived class

            //check to see if the sqlite folder exists, if not create it
            //check the Arma2 root directory first
            if (System.IO.Directory.Exists("sqlite"))
            {
                sqliteDatabaseLocation = Path.GetFullPath("sqlite/");
            }
            else
            {
                sqliteDatabaseLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Arma2NETMySQL/sqlite/");
                if (!System.IO.Directory.Exists(sqliteDatabaseLocation))
                {
                    System.IO.Directory.CreateDirectory(sqliteDatabaseLocation);
                }
            }
        }

        public override void OpenConnection(string databasename)
        {
            try
            {
                // if there is no connection
                if (sqlite_connection == null)
                {
                    Logger.addMessage(Logger.LogType.Info, "SQLite folder location: " + sqliteDatabaseLocation);
                    sqlite_connection = new SQLiteConnection("Data Source=" + sqliteDatabaseLocation + databasename + ".sqlite");
                }

                sqlite_connection.Open();
            }
            catch (Exception ex)
            {
                Logger.addMessage(Logger.LogType.Error, "Unable to open connection to SQLite database." + ex.ToString());
            }
        }

        public override void CloseConnection()
        {
            if (sqlite_connection != null)
            {
                try
                {
                    sqlite_connection.Close();
                    sqlite_connection = null;
                }
                catch (Exception ex)
                {
                    Logger.addMessage(Logger.LogType.Error, "Unable to close SQLite connection." + ex.ToString());
                }
            }
        }

        public override IEnumerable<string[][]> RunProcedure(string procedure, string[] parameters, int maxResultSize)
        {
            Logger.addMessage(Logger.LogType.Error, "SQLite cannot run stored procedures.");
            return null;
        }

        public override IEnumerable<string[][]> RunCommand(string sql_command, int maxResultSize)
        {
            //SQLite example code:
            //http://www.dreamincode.net/forums/topic/157830-using-sqlite-with-c%23/

            DataTable dt = new DataTable();
            try
            {
                SQLiteCommand mycommand = new SQLiteCommand(sqlite_connection);
                mycommand.CommandText = sql_command;
                SQLiteDataReader reader = mycommand.ExecuteReader();
                dt.Load(reader);
                reader.Close();
            }
            catch (Exception ex)
            {
                Logger.addMessage(Logger.LogType.Error, "Unable to run SQLite command." + ex.ToString());
            }

            //convert datatable to array
            string[][] string_array = new string[dt.Rows.Count][];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                string_array[i] = row.ItemArray.Select(j => j.ToString()).ToArray();
            }


            if (validLength(string_array, maxResultSize) == false)
            {
                yield return new string[][] { new[] { "TooLong" } };
            }
            else
            {
                yield return string_array;
            }
        }
    }
}
