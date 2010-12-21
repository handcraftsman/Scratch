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

namespace Scratch.ShuffleIEnumerable
{
    /// <summary>
    /// http://stackoverflow.com/questions/3645644/whats-your-favorite-linq-to-objects-operator-which-is-not-built-in/3646859#3646859
    /// </summary>
    public static class IEnumerableExtensions
    {
        private static readonly Random _rand = new Random();

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            var items = source == null ? new T[] { } : source.ToArray();

            for (int i = 0; i < items.Length; i++)
            {
                int toReturn = _rand.Next(i, items.Length);
                yield return items[toReturn];
                items[toReturn] = items[i];
            }
        }
    }
}