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

namespace Scratch.SumValuesOfTwoLists
{
    /// <summary>
    /// http://stackoverflow.com/questions/1190657/add-two-lists-of-different-length-in-c/1241495#1241495
    /// </summary>
    public static class Numeric
    {
        public static IList<double> Sum(IList<double> a, IList<double> b)
        {
            var doubles = Enumerable.Range(0, Math.Max(a.Count, b.Count))
                .Select(x => (a.Count > x ? a[x] : 0) + (b.Count > x ? b[x] : 0))
                .ToList();
            return doubles;
        }
    }
}