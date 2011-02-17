//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Scratch.JoinStringsWithSeparator
{
    /// <summary>
    /// http://stackoverflow.com/questions/853137/adding-a-separator-to-a-list-of-items-for-display/853812#853812
    /// </summary>
    [TestFixture]
    public class Experiments
    {
        /// <summary>
        /// 00:00:00.0066368    True    StringJoin
        /// 00:00:28.4661134    True    Aggregate
        /// 00:00:54.5965833    True    CheckForEndInsideLoop_String
        /// 00:00:54.6720860    True    CheckForBeginningInsideLoop_String
        /// 00:00:27.6516146    True    RemoveFinalDelimiter_String
        /// 00:00:00.0074332    True    CheckForEndInsideLoop_StringBuilder
        /// 00:00:00.0093083    True    RemoveFinalDelimiter_StringBuilder
        /// </summary>
        [Test, Explicit]
        public void Time_variants()
        {
            const string item = "a";
            const int numberOfTimes = 100000;
            const string delimiter = ", ";
            var items = new List<string>(Enumerable.Repeat(item, numberOfTimes)).ToArray();
            string expected = String.Join(delimiter, items);

            Time(StringJoin, items, delimiter, expected);
            Time(Aggregate, items, delimiter, expected);
            Time(CheckForEndInsideLoop_String, items, delimiter, expected);
            Time(CheckForBeginningInsideLoop_String, items, delimiter, expected);
            Time(RemoveFinalDelimiter_String, items, delimiter, expected);
            Time(CheckForEndInsideLoop_StringBuilder, items, delimiter, expected);
            Time(RemoveFinalDelimiter_StringBuilder, items, delimiter, expected);
        }

        private static string Aggregate(string[] items, string delimiter)
        {
            return items.Aggregate((c, s) => c + delimiter + s);
        }

        private static string CheckForBeginningInsideLoop_String(string[] items, string delimiter)
        {
            string result = "";
            foreach (string s in items)
            {
                if (result.Length != 0)
                {
                    result += delimiter;
                }
                result += s;
            }
            return result;
        }

        private static string CheckForEndInsideLoop_String(string[] items, string delimiter)
        {
            string result = "";
            for (int i = 0; i < items.Length; i++)
            {
                result += items[i];
                if (i != items.Length - 1)
                {
                    result += delimiter;
                }
            }
            return result;
        }

        private static string CheckForEndInsideLoop_StringBuilder(string[] items, string delimiter)
        {
            var result = new StringBuilder();
            for (int i = 0; i < items.Length; i++)
            {
                result.Append(items[i]);
                if (i != items.Length - 1)
                {
                    result.Append(delimiter);
                }
            }
            return result.ToString();
        }

        private static string RemoveFinalDelimiter_String(string[] items, string delimiter)
        {
            string result = "";
            for (int i = 0; i < items.Length; i++)
            {
                result += items[i] + delimiter;
            }
            return result.Substring(0, result.Length - delimiter.Length);
        }

        private static string RemoveFinalDelimiter_StringBuilder(string[] items, string delimiter)
        {
            var result = new StringBuilder();
            for (int i = 0; i < items.Length; i++)
            {
                result.Append(items[i]);
                result.Append(delimiter);
            }
            result.Length = result.Length - delimiter.Length;
            return result.ToString();
        }

        private static string StringJoin(string[] items, string delimiter)
        {
            return String.Join(delimiter, items);
        }

        private static void Time(Func<string[], string, string> func, string[] items, string delimiter, string expected)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            string result = func(items, delimiter);
            stopwatch.Stop();
            bool isValid = result == expected;
            Console.WriteLine("{0}\t{1}\t{2}", stopwatch.Elapsed, isValid, func.Method.Name);
        }
    }
}