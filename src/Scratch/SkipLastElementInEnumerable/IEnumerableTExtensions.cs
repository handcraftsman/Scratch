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

namespace Scratch.SkipLastElementInEnumerable
{
    /// <summary>
    /// http://stackoverflow.com/questions/969091/c-skiplast-implementation/971447#971447
    /// </summary>
    public static class IEnumerableTExtensions
    {
        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source)
        {
            if (!source.Any())
            {
                yield break;
            }
            var items = new Queue<T>();
            items.Enqueue(source.First());
            foreach (var item in source.Skip(1))
            {
                yield return items.Dequeue();
                items.Enqueue(item);
            }
        }
    }
}