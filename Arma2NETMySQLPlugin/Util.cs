using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Globalization;


namespace Arma2NETMySQLPlugin
{
    class Util
    {
        public static String stripSquareBrackets(String input)
        {
            if (input.StartsWith("[") && input.EndsWith("]"))
            {
                input = input.Substring(1, input.Length - 2);
            }
            return input;
        }

        public static String stripQuotes(String input)
        {
            if ((input.StartsWith("\"") && input.EndsWith("\"")) || (input.StartsWith("\'") && input.EndsWith("\'")))
            {
                input = input.Substring(1, input.Length - 2);
            }
            return input;
        }

        //From the old Format.cs code from Arma2NET
        //Licensed under the Apache License v2
        // -->
        public static string ObjNull { get { return "objNull"; } }
        public static string Nil { get { return "nil"; } }

        public static string ObjectAsSqf(double obj)
        {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        public static string ObjectAsSqf(bool obj)
        {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        public static string ObjectAsSqf(string obj)
        {
            if (obj == null)
            {
                return ObjNull;
            }
            return '"' + obj.Replace("\"", "\"\"").Replace("'", "''") + '"';
        }

        public static string ObjectAsSqf(char obj)
        {
            return ObjectAsSqf(obj.ToString(CultureInfo.InvariantCulture));
        }

        public static string ObjectAsSqf(IEnumerable<object> obj)
        {
            if (obj == null)
            {
                return ObjNull;
            }
            return '[' + string.Join(", ", obj.Select(ObjectAsSqf)) + ']';
        }

        public static string ObjectAsSqf<T>(IEnumerable<T> obj)
        {
            if (obj == null)
            {
                return ObjNull;
            }
            return ObjectAsSqf(obj.Cast<object>());
        }

        public static string ObjectAsSqf(object obj)
        {
            if (obj == null)
            {
                return ObjNull;
            }
            if (obj is char || obj is string)
            {
                return ObjectAsSqf(obj.ToString());
            }
            var enumObj = obj as IEnumerable<object>;
            if (enumObj != null)
            {
                return ObjectAsSqf(enumObj);
            }
            return obj.ToString();
        }
        // <--
    }
}
