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
using System.Diagnostics;
using System.Linq;

using FluentAssert;

using NUnit.Framework;

namespace Scratch.RangeEnumeration
{
    /// <summary>
    /// // http://stackoverflow.com/questions/4271060/can-someone-come-up-with-a-better-version-of-this-enumerator
    /// </summary>
    [TestFixture]
    public class Tests
    {
        private char[] _source;
        

        [SetUp]
        public void BeforeEachTest()
        {
            //         0123456789
            _source = "ABCDEFGHIJ".ToCharArray();
        }

        [Test]
        public void Given_contained_range_overlapping_another_range()
        {
            var ranges = new List<Pair<int, int>>
                {
                    new Pair<int, int>(1, 6),
                    new Pair<int, int>(2, 5),
                    new Pair<int, int>(4, 8),
                };
            string result = new string(WalkRanges(_source, ranges).ToArray());
            result.ShouldBeEqualTo("BCDEFGHI");
        }

        [Test]
        public void Given_empty_ranges()
        {
            List<Pair<int, int>> ranges = null;
            var result = WalkRanges(_source, ranges);
            result.ShouldContainAllInOrder(_source);
        }

        [Test]
        public void Given_non_overlapping_ranges()
        {
            var ranges = new List<Pair<int, int>>
                {
                    new Pair<int, int>(-5, 3),
                    new Pair<int, int>(7, 10)
                };
            string result = new string(WalkRanges(_source, ranges).ToArray());
            result.ShouldBeEqualTo("ABCDHIJ");
        }

        [Test]
        public void Given_null_ranges()
        {
            List<Pair<int, int>> ranges = null;
            var result = WalkRanges(_source, ranges);
            result.ShouldContainAllInOrder(_source);
        }

        [Test]
        public void Given_one_range_containing_another()
        {
            var ranges = new List<Pair<int, int>>
                {
                    new Pair<int, int>(1, 6),
                    new Pair<int, int>(2, 3)
                };
            string result = new string(WalkRanges(_source, ranges).ToArray());
            result.ShouldBeEqualTo("BCDEFG");
        }

        [Test]
        public void Given_overlapping_and_non_overlapping_ranges()
        {
            var ranges = new List<Pair<int, int>>
                {
                    new Pair<int, int>(1, 3),
                    new Pair<int, int>(2, 5),
                    new Pair<int, int>(8, 9),
                };
            string result = new string(WalkRanges(_source, ranges).ToArray());
            result.ShouldBeEqualTo("BCDEFIJ");
        }

        [Test]
        public void Given_overlapping_ranges()
        {
            var ranges = new List<Pair<int, int>>
                {
                    new Pair<int, int>(1, 6),
                    new Pair<int, int>(4, 8)
                };
            string result = new string(WalkRanges(_source, ranges).ToArray());
            result.ShouldBeEqualTo("BCDEFGHI");
        }

        [Test]
        public void Given_range_overlapping_end_of_source()
        {
            var ranges = new List<Pair<int, int>>
                {
                    new Pair<int, int>(6, 20)
                };
            string result = new string(WalkRanges(_source, ranges).ToArray());
            result.ShouldBeEqualTo("GHIJ");
        }

        [Test]
        public void Given_range_overlapping_start_of_source()
        {
            var ranges = new List<Pair<int, int>>
                {
                    new Pair<int, int>(-5, 4)
                };
            string result = new string(WalkRanges(_source, ranges).ToArray());
            result.ShouldBeEqualTo("ABCDE");
        }

        [Test]
        public void Given_range_starting_and_ending_inside_the_source()
        {
            var ranges = new List<Pair<int, int>>
                {
                    new Pair<int, int>(3, 6)
                };
            string result = new string(WalkRanges(_source, ranges).ToArray());
            result.ShouldBeEqualTo("DEFG");
        }

        public static IEnumerable<T> WalkRanges<T>(IEnumerable<T> source, List<Pair<int, int>> ranges)
        {
            Debug.Assert(ranges == null || ranges.Count > 0);
            return ranges == null || ranges.Count < 1 ? source : WalkRangesInternal(source, ranges);
        }

        private static IEnumerable<T> WalkRangesInternal<T>(IEnumerable<T> source, IList<Pair<int, int>> ranges)
        {
            var currentRange = ranges[0];
            int currentRangeIndex = 0;
            bool betweenRanges = currentRange.First > 0;

            foreach (var itemWithIndex in source.Select((x, index) => new
                {
                    x,
                    index
                }))
            {
                int currentItem = itemWithIndex.index;
                if (betweenRanges)
                {
                    if (currentItem == currentRange.First)
                    {
                        betweenRanges = false;
                    }
                    else
                    {
                        continue;
                    }
                }

                yield return itemWithIndex.x;
                while (currentItem >= currentRange.Second)
                {
                    if (currentRangeIndex == ranges.Count - 1)
                    {
                        yield break; // We just visited the last item in the ranges
                    }

                    currentRangeIndex = currentRangeIndex + 1;
                    currentRange = ranges[currentRangeIndex];
                    betweenRanges = currentItem < currentRange.First;
                }
            }
        }
    }

    public class Pair<T, T1>
    {
        public Pair(T first, T1 second)
        {
            First = first;
            Second = second;
        }

        public Pair()
        {
        }

        public T First { get; set; }
        public T1 Second { get; set; }
    }
}