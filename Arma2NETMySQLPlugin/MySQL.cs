using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MySql.Data.MySqlClient;

namespace Arma2NETMySQLPlugin
{
    class MySQL
    {
        MySqlConnection connection;

        private object getCommandSync = new object();

        public MySQL()
        {
            //constructor
        }

        public void OpenConnection(string connectionString)
        {
            // if there is no connection
            if (connection == null)
            {
                connection = new MySqlConnection(connectionString);
            }
            // if there is a connection and it is a different connection string
            else if (connection.ConnectionString != connectionString)
            {
                // close old connection and create with the new connection string
                CloseConnection();
                connection = new MySqlConnection(connectionString);
            }

            while (connection.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    Logger.addMessage(Logger.LogType.Info, "Unable to open connection to database, trying again in 10 seconds." + ex.ToString());
                    Thread.Sleep(10000);
                }
            }
        }

        public void CloseConnection()
        {
            if (connection != null)
            {
                while (connection.State == System.Data.ConnectionState.Executing || connection.State == System.Data.ConnectionState.Fetching) ;

                if (connection.State != System.Data.ConnectionState.Closed)
                {
                    connection.Close();
                    connection = null;
                }
            }
        }

        public IEnumerable<string[]> RunProcedure(string procedure, string[] parameters)
        {
            //Logger.addMessage(Logger.LogType.Info, "Started RunProcedure");
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                MySqlCommand command = GetCommand(procedure);

                if (parameters != null) // could have a procedure with no inputs
                {
                    Logger.addMessage(Logger.LogType.Info, "Parsing parameters...");
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        //separate out parameter name and value
                        string[] split = parameters[i].Split('=');

                        if (split.Length != 2)
                            Logger.addMessage(Logger.LogType.Warning, "Couldn't parse procedure, split didn't work.");

                        //Logger.addMessage(Logger.LogType.Info, "Adding parameter key:value " + split[0].ToString() + ":" + split[1].ToString());
                        command.Parameters.AddWithValue(split[0], (object)split[1]);
                    }
                }

                MySqlDataReader reader = null;

                Boolean mysql_error = false;
                try
                {
                    //Logger.addMessage(Logger.LogType.Info, "Executing mysql command.");
                    reader = command.ExecuteReader();
                }
                catch (Exception sql_ex)
                {
                    //Catch any MySQL errors (bad procedure name, missing/malformed parameters, etc.) and just return false
                    //Can't use yield in a try/catch so we'll return later...
                    mysql_error = true;
                    Logger.addMessage(Logger.LogType.Warning, "MySQL error. " + sql_ex.ToString());
                }

                if (mysql_error == false)
                {
                    while (reader.Read())
                    {
                        string[] row = new string[reader.FieldCount];
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[i] = reader.GetString(i);
                            //Logger.addMessage(Logger.LogType.Info, "Row value is: " + row[i].ToString());
                        }
                        yield return row;
                    }
                    reader.Close();
                }
                else
                {
                    //Logger.addMessage(Logger.LogType.Info, "Returning false from RunProcedure");
                    yield return new string[] { "false" };
                }
            }
            //Logger.addMessage(Logger.LogType.Info, "yield breaking in RunProcedure");
            yield break;
        }

        private MySqlCommand GetCommand(string procedureName)
        {
            MySqlCommand cmd = null;
            // only one request at a time.
            lock (getCommandSync)
            {
                // create the command
                cmd = new MySqlCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection = connection;
                cmd.CommandText = procedureName;
                cmd.Prepare();
            }
            return cmd;
        }
    }
}
