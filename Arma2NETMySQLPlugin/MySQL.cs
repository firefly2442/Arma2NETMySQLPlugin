using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Threading;

namespace Arma2NETMySQLPlugin
{
    public class MySQL : SQL
    {
        private MySqlConnection connection;
        private object getCommandSync = new object();

        public MySQL()
        {
            // Default constructor for the derived class
        }

        public override void OpenConnection(string connectionString)
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
                    Logger.addMessage(Logger.LogType.Info, "Unable to open connection to MySQL database, trying again in 10 seconds." + ex.ToString());
                    Thread.Sleep(10000);
                }
            }
        }

        public override void CloseConnection()
        {
            if (connection != null)
            {
                while (connection.State == System.Data.ConnectionState.Executing || connection.State == System.Data.ConnectionState.Fetching);

                if (connection.State != System.Data.ConnectionState.Closed)
                {
                    connection.Close();
                    connection = null;
                }
            }
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

        public override IEnumerable<string[][]> RunProcedure(string procedure, string[] parameters, int maxResultSize)
        {
            //Logger.addMessage(Logger.LogType.Info, "Started RunProcedure");
            if (connection != null && connection.State == System.Data.ConnectionState.Open && procedure != null)
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

                yield return RunOnDatabase(command, maxResultSize);
            }
            //Logger.addMessage(Logger.LogType.Info, "yield breaking in RunProcedure");
            yield break;
        }

        public override IEnumerable<string[][]> RunCommand(string mysql_command, int maxResultSize)
        {
            //Logger.addMessage(Logger.LogType.Info, "Started RunCommand");
            if (connection != null && connection.State == System.Data.ConnectionState.Open && mysql_command != null)
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = mysql_command;
                yield return RunOnDatabase(command, maxResultSize);
            }
            //Logger.addMessage(Logger.LogType.Info, "yield breaking in RunProcedure");
            yield break;
        }

        private string[][] RunOnDatabase(MySqlCommand command, int maxResultSize)
        {
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

            using (reader)
            {
                if (mysql_error == false)
                {
                    List<List<string>> to_return = new List<List<string>>();
                    int max_array_rows = 0;
                    while (reader.Read())
                    {
                        List<string> inner_data = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string value = reader.GetString(i);
                            inner_data.Add(value);
                            //Logger.addMessage(Logger.LogType.Info, "Row value is: " + value);
                        }
                        to_return.Add(inner_data);
                        max_array_rows++;
                    }
                    reader.Close();

                    //convert into the array which we'll have to pass back
                    string[][] string_array = new string[max_array_rows][];
                    for (int i = 0; i < max_array_rows; i++)
                    {
                        string_array[i] = to_return[i].ToArray();
                    }

                    if (validLength(string_array, maxResultSize) == false)
                    {
                        return new string[][] { new[] { "TooLong" } };
                    }
                    else
                    {
                        return string_array;
                    }
                }
            }
            //Logger.addMessage(Logger.LogType.Info, "Returning error from RunProcedure");
            return new string[][] { new[] { "Error" } };
        }
    }
}
