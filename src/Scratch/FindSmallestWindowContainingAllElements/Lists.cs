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

namespace Scratch.FindSmallestWindowContainingAllElements
{
    /// <summary>
    /// http://stackoverflow.com/questions/3744601/find-the-smallest-window-of-input-array-that-contains-all-the-elements-of-query-a/3746817#3746817
    /// </summary>
    public static class Lists
    {
        public static int[] SmallestWindow(int[] inputArray, int[] queryArray)
        {
            var indexed = queryArray
                .SelectMany(x => inputArray
                                     .Select((y, i) => new
                                         {
                                             Value = y,
                                             Index = i
                                         })
                                     .Where(y => y.Value == x))
                .OrderBy(x => x.Index)
                .ToList();

            var segments = indexed
                .Select(x =>
                            {
                                var unique = new HashSet<int>();
                                return new
                                    {
                                        Item = x,
                                        Followers = indexed
                                            .Where(y => y.Index >= x.Index)
                                            .TakeWhile(y => unique.Count != queryArray.Length)
                                            .Select(y =>
                                                        {
                                                            unique.Add(y.Value);
                                                            return y;
                                                        })
                                            .ToList(),
                                        IsComplete = unique.Count == queryArray.Length
                                    };
                            })
                .Where(x => x.IsComplete);

            var queryIndexed = segments
                .Select(x => x.Followers.Select(y => new
                    {
                        QIndex = Array.IndexOf(queryArray, y.Value),
                        y.Index,
                        y.Value
                    }).ToArray());

            var queryOrdered = queryIndexed
                .Where(item =>
                           {
                               var qindex = item.Select(x => x.QIndex).ToList();
                               bool changed;
                               do
                               {
                                   changed = false;
                                   for (int i = 1; i < qindex.Count; i++)
                                   {
                                       if (qindex[i] <= qindex[i - 1])
                                       {
                                           qindex.RemoveAt(i);
                                           changed = true;
                                       }
                                   }
                               } while (changed);
                               return qindex.Count == queryArray.Length;
                           });
            var result = queryOrdered
                .Select(x => new[]
                    {
                        x.First().Index,
                        x.Last().Index
                    })
                .OrderBy(x => x[1] - x[0]);

            var best = result.FirstOrDefault();
            return best;
        }
    }
}