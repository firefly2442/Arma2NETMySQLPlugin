/*
 * Copyright 2012 Arma2NET Developers
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;


namespace Arma2NETMySQLPlugin
{
    /// <summary>
    /// Utility functions for representing .NET objects as SQF code and vice versa.
    /// </summary>
    public static class Format
    {
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

        public static bool TrySqfAsBoolean(string sqf, out bool result)
        {
            return bool.TryParse(sqf, out result);
        }

        public static bool TrySqfAsDouble(string sqf, out double result)
        {
            return double.TryParse(sqf, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        }

        public static bool TrySqfAsString(string sqf, out string result)
        {
            if (SqfIsNull(sqf) || SqfIsNil(sqf))
            {
                result = null;
                return false;
            }
            if (sqf.Length >= 2 && ((sqf.First() == '\'' && sqf.Last() == '\'') || (sqf.First() == '"' && sqf.Last() == '"')))
            {
                result = sqf.Substring(1, sqf.Length - 2).Replace("\"\"", "\"").Replace("''", "'");
                return true;
            }
            result = sqf.Replace("\"\"", "\"").Replace("''", "'");
            return true;
        }

        public static bool SqfIsNull(string sqf)
        {
            return string.IsNullOrWhiteSpace(sqf) || sqf.Equals(ObjNull, StringComparison.OrdinalIgnoreCase) || sqf == "<NULL-object>";
        }

        public static bool SqfIsNil(string sqf)
        {
            return string.IsNullOrWhiteSpace(sqf) || sqf.Equals(Nil, StringComparison.OrdinalIgnoreCase);
        }

        public static object SqfAsObject(string sqf)
        {
            if (SqfIsNull(sqf) || SqfIsNil(sqf))
            {
                return null;
            }
            IList<object> collection;
            if (TrySqfAsCollection(sqf, out collection))
            {
                return collection;
            }
            bool bl;
            if (TrySqfAsBoolean(sqf, out bl))
            {
                return bl;
            }
            double dbl;
            if (TrySqfAsDouble(sqf, out dbl))
            {
                return dbl;
            }
            string str;
            if (TrySqfAsString(sqf, out str))
            {
                return str;
            }
            return sqf;
        }

        public static bool TrySqfAsCollection(string sqf, out IList<object> result)
        {
            return TrySqfAsCollection(sqf, out result, true);
        }

        public static bool TrySqfAsCollection(string sqf, out IList<object> result, bool recurse)
        {
            if (SqfIsNull(sqf) || SqfIsNil(sqf) || sqf[0] != '[' || sqf[sqf.Length - 1] != ']')
            {
                result = null;
                return false;
            }
            var collection = SqfAsCollectionImpl(sqf);
            result = new ReadOnlyCollection<object>((recurse ? collection.Select(SqfAsObject) : collection).ToList());
            return true;
        }

        static IEnumerable<string> SqfAsCollectionImpl(string sqf)
        {
            var buffer = new StringBuilder();
            var inner = sqf.Substring(1, sqf.Length - 2);
            char? quoteChar = null;
            foreach (var c in inner)
            {
                if (!quoteChar.HasValue)
                {
                    if (c == '[')
                    {
                        quoteChar = ']';
                    }
                    if (c == '\'' || c == '"')
                    {
                        quoteChar = c;
                    }
                    if (c == ',')
                    {
                        yield return buffer.ToString().Trim();
                        buffer.Clear();
                        continue;
                    }
                }
                else if (c == quoteChar)
                {
                    quoteChar = null;
                }
                buffer.Append(c);
            }
            yield return buffer.ToString().Trim();
        }

        public static Tuple<string, string> ParseFunction(string function)
        {
            if (function == null)
            {
                throw new ArgumentNullException("function");
            }
            IList<object> collection;
            string functionName;
            string functionArgs = null;
            if (TrySqfAsCollection(function, out collection, false))
            {
                if (!TrySqfAsString((string)collection[0], out functionName))
                {
                    throw new FormatException(string.Format("Unable to parse the function name: {0}", function));
                }
                if (collection.Count > 1)
                {
                    functionArgs = (string)collection[1];
                }
            }
            else
            {
                var splitFunction = function.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                if (splitFunction.Length == 0)
                {
                    throw new FormatException(string.Format("Unable to parse the function name: {0}", function));
                }
                functionName = splitFunction[0];
                if (splitFunction.Length > 1)
                {
                    functionArgs = splitFunction[1];
                }
            }
            return Tuple.Create(functionName, functionArgs);
        }
    }
}
