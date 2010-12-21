//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************
using System.Collections.Generic;
using System.Linq;

namespace Scratch.ListsHaveSameContents
{
    /// <summary>
    /// http://stackoverflow.com/questions/2913287/comparing-arrays-using-linq-in-c/2913406#2913406
    /// </summary>
    public static class ICollectionTExtensions
    {
        public static bool HasSameContentsAs<T>(this ICollection<T> source,
                                                ICollection<T> other)
        {
            if (source.Count != other.Count)
            {
                return false;
            }
            var s = source
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count());
            var o = other
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count());
            int count;
            return s.Count == o.Count &&
                   s.All(x => o.TryGetValue(x.Key, out count) &&
                              count == x.Value);
        }
    }
}