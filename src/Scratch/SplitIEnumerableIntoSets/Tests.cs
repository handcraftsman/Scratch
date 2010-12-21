//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************

using System.Linq;

using FluentAssert;

using NUnit.Framework;

namespace Scratch.SplitIEnumerableIntoSets
{
    /// <summary>
    /// http://stackoverflow.com/questions/2222292/optimize-this-slow-linq-to-objects-query/2374520#2374520
    /// </summary>
    [TestFixture]
    public class Tests
    {
        /// <summary>
        /// http://stackoverflow.com/questions/4461367/linq-to-objects-return-pairs-of-numbers-from-list-of-numbers/4471596#4471596
        /// </summary>
        [Test]
        public void Should_fill_an_incomplete_set_if_requested()
        {
            var nums = new[] { 1, 2, 3, 4, 5, 6, 7 };
            const int numberInSet = 2;
            const int fillValue = 9;
            var pairs = nums.InSetsOf(numberInSet, true, fillValue).ToArray();
            var last = pairs.Last();
            last.Count.ShouldBeEqualTo(numberInSet);
            last[0].ShouldBeEqualTo(nums.Last());
            last[1].ShouldBeEqualTo(fillValue);
        }

        [Test]
        public void Should_return_the_correct_number_of_sets_if_the_input_contains_a_multiple_of_the_setSize()
        {
            var input = "abcdefghij".Select(x => x.ToString()).ToList();
            var result = input.InSetsOf(5);
            result.Count().ShouldBeEqualTo(2);
            result.First().Count.ShouldBeEqualTo(5);
            result.Last().Count.ShouldBeEqualTo(5);
        }

        [Test]
        public void Should_separate_the_input_into_sets_of_size_requested()
        {
            var input = "abcdefghijklm".Select(x => x.ToString()).ToList();
            var result = input.InSetsOf(5);
            result.Count().ShouldBeEqualTo(3);
            result.First().Count.ShouldBeEqualTo(5);
            result.Last().Count.ShouldBeEqualTo(3);
        }
    }
}