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
using System.Linq;

namespace Scratch.StringSearching
{
    /// <summary>
    /// http://stackoverflow.com/questions/3925063/help-fix-my-kmp-search-algorithm/3936998#3936998
    /// </summary>
    public static class KMPSearch
    {
        public static int[] BuildTable<T>(IList<T> word) where T : IComparable
        {
            var table = Enumerable.Repeat(0, word.Count).ToList();

            table[0] = -1;
            table[1] = 0;
            int current = 0;
            for (int pos = 2; pos < word.Count; pos++)
            {
                if (word[pos - 1].CompareTo(word[current]) == 0)
                {
                    current++;
                    table[pos] = current;
                }
                else if (current > 0)
                {
                    current = table[current];
                }
                else
                {
                    table[pos] = 0;
                }
            }
            return table.ToArray();
        }

        public static int IndexOf<T>(IList<T> word, IList<T> textToBeSearched, int start) where T : IComparable
        {
            // http://en.wikipedia.org/wiki/Knuth%E2%80%93Morris%E2%80%93Pratt_algorithm

            var table = BuildTable(word);
            int i = 0;
            while ((start + i) < textToBeSearched.Count)
            {
                if (word[i].CompareTo(textToBeSearched[start + i]) == 0)
                {
                    if (i++ == word.Count - 1)
                    {
                        return start;
                    }
                }
                else
                {
                    start = start + i - table[i];
                    i = table[i] > -1 ? table[i] : 0;
                }
            }
            return -1;
        }
    }
}