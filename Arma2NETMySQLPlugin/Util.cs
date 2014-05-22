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
