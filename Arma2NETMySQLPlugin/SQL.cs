using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arma2Net.AddInProxy;

namespace Arma2NETMySQLPlugin
{
    public abstract class SQL
    {
        public static Databases dbs = null;

        public SQL()
        {
            //constructor
        }

        public abstract void OpenConnection(string connectionString);
        public abstract void CloseConnection();

        public abstract IEnumerable<string[][]> RunProcedure(string procedure, string[] parameters, int maxResultSize);
        public abstract IEnumerable<string[][]> RunCommand(string sql_command, int maxResultSize);


        protected Boolean validLength(string[][] check, int max_length)
        {
            /*
             * callExtension is the method that is used by Arma2NET to pass information between itself and Arma2
             * callExtension has a size limit for the max amount of data that can be passed:
             * http://community.bistudio.com/wiki/Extensions#A_few_technical_considerations
             * The limit is 16 Kilobytes (for Arma 2 beta 97299)
             * One character = one byte
             * The Wiki notes that this size limit could change through future patches.
             * https://dev-heaven.net/issues/25915
             * 
             * Arma2NET has a long output addin method that does the following:
             * "From version 1.5, Arma2NET supports plugins requiring the maximum result size as an argument to the Run method.
             * You can use this to ensure that a plugin won't return a result that is too long for Arma 2 to handle."
             * 
             * In Arma2NET, 16383 characters is the limit
             * The last character is reserved for null
             * 
             */
            string formatted = Format.ObjectAsSqf(check);
            int size = Encoding.UTF8.GetByteCount(formatted.ToCharArray());
            //Logger.addMessage(Logger.LogType.Info, "Size length: " + size);
            if (size > max_length) {
                return false;
            } else {
                return true;
            }
        }
    }
}
