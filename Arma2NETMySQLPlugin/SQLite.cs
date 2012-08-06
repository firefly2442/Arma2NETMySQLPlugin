using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;

namespace Arma2NETMySQLPlugin
{
    public class SQLite : SQL
    {
        private SQLiteConnection sqlite_connection;

        public SQLite()
        {
            // Default constructor for the derived class

            //check to see if the sqlite folder exists, if not create it
            if (!System.IO.Directory.Exists("sqlite"))
            {
                System.IO.Directory.CreateDirectory("sqlite");
            }
        }

        public override void OpenConnection(string databasename)
        {
            try
            {
                // if there is no connection
                if (sqlite_connection == null)
                {
                    sqlite_connection = new SQLiteConnection("Data Source=sqlite/" + databasename + ".sqlite");
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
                Logger.addMessage(Logger.LogType.Info, "Row value is: " + dt.Rows[i].ToString());
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string_array[i][j] = dt.Rows[i][j].ToString();
                }
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
