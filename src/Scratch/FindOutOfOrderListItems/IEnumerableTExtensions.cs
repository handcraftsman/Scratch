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

namespace Scratch.FindOutOfOrderListItems
{
    /// <summary>
    /// http://stackoverflow.com/questions/998073/compare-two-lists-or-arrays-of-arbitrary-length-in-c-order-is-important/998907#998907
    /// </summary>
    public static class IEnumerableTExtensions
    {
        public static int? FindOutOfOrderItems<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            var indexedList1 = actual.Select((x, i) => new
                {
                    Index = i,
                    Item = x
                });

            var indexedList2 = expected.Select((x, i) => new
                {
                    Index = i,
                    Item = x
                });

            var intersectedWithIndexes = indexedList2
                .Join(indexedList1,
                      x => x.Item,
                      y => y.Item,
                      (x, y) => new
                          {
                              ExpectedIndex = x.Index,
                              ActualIndex = y.Index,
                              x.Item
                          })
                .Where(x => x.ActualIndex != x.ExpectedIndex)
                .ToArray();

            var outOfOrder = intersectedWithIndexes
                .Select((x, i) => new
                    {
                        Item = x,
                        index = i
                    })
                .Skip(1)
                .Where(x => x.Item.ActualIndex < intersectedWithIndexes[x.index - 1].ActualIndex ||
                            x.Item.ExpectedIndex < intersectedWithIndexes[x.index - 1].ExpectedIndex)
                .Select(x => new
                    {
                        ExpectedBefore = x.Item,
                        ExpectedAfter = intersectedWithIndexes[x.index - 1]
                    });

            var firstOutOfOrder = outOfOrder.FirstOrDefault();
            if (firstOutOfOrder == null)
            {
                return null;
            }
            return firstOutOfOrder.ExpectedBefore.ActualIndex;
        }
    }
}