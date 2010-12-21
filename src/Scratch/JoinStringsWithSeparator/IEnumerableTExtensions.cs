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
using System.Text;

namespace Scratch.JoinStringsWithSeparator
{
    /// <summary>
    /// http://stackoverflow.com/questions/853137/adding-a-separator-to-a-list-of-items-for-display/853812#853812
    /// </summary>
    public static class IEnumerableTExtensions
    {
        public static string Join<T>(this IEnumerable<T> items, string delimiter)
        {
            var result = new StringBuilder();
            if (items != null && items.Any())
            {
                delimiter = delimiter ?? "";
                foreach (var item in items)
                {
                    result.Append(item);
                    result.Append(delimiter);
                }
                result.Length = result.Length - delimiter.Length;
            }
            return result.ToString();
        }
    }
}