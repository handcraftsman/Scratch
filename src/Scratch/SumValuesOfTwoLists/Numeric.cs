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