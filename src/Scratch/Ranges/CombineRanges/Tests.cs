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

using FluentAssert;

using NUnit.Framework;

namespace Scratch.Ranges.CombineRanges
{
    /// <summary>
    /// http://stackoverflow.com/questions/4147935/converting-sets-of-integers-into-ranges-using-c
    /// </summary>
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Should_combine_ordered_data_0_1_2_3_4_7_8_9_11_into_3_sets__0_4__7_9__11_11()
        {
            var input = new[] { 0, 1, 2, 3, 4, 7, 8, 9, 11 };
            var result = CombineRanges(input).OrderBy(x => x.Key).ToList();
            result.Count.ShouldBeEqualTo(3, "incorrect number of ranges: " + result.Count);
            result.First().ShouldBeEqualTo(new KeyValuePair<int, int>(0, 4));
            result.Skip(1).First().ShouldBeEqualTo(new KeyValuePair<int, int>(7, 9));
            result.Last().ShouldBeEqualTo(new KeyValuePair<int, int>(11, 11));
        }

        [Test]
        public void Should_combine_ordered_data_11_9_8_7_4_3_2_1_0_into_3_sets__0_4__7_9__11_11()
        {
            var input = new[] { 11, 9, 8, 7, 4, 3, 2, 1, 0 };
            var result = CombineRanges(input).OrderBy(x => x.Key).ToList();
            result.Count.ShouldBeEqualTo(3, "incorrect number of ranges: " + result.Count);
            result.First().ShouldBeEqualTo(new KeyValuePair<int, int>(0, 4));
            result.Skip(1).First().ShouldBeEqualTo(new KeyValuePair<int, int>(7, 9));
            result.Last().ShouldBeEqualTo(new KeyValuePair<int, int>(11, 11));
        }

        public IEnumerable<KeyValuePair<int, int>> CombineRanges(IEnumerable<int> input)
        {
            var ranges = input.ToDictionary(i => i, i => i);
            input
                .Where(x => ranges.ContainsKey(x - 1))
                .OrderBy(x => x)
                .ToList()
                .ForEach(x => ranges[x] = ranges[x - 1]);

            return ranges
                .GroupBy(x => x.Value)
                .Select(x => new KeyValuePair<int, int>(x.Key, x.Max(y => y.Key)));
        }
    }
}