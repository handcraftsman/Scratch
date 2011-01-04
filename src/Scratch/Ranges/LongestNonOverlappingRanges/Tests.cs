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

using Scratch.Ranges.RangeEnumeration;

namespace Scratch.Ranges.LongestNonOverlappingRanges
{
	/// <summary>
	///     http://stackoverflow.com/questions/4593583/algorithm-to-find-longest-non-overlapping-sequences
	///     Pairs are start and length
	[TestFixture]
	public class Tests
	{
		[Test]
		public void Given_the_first_problem_sample()
		{
			var input = new[]
				{
					new Pair<int, int>(0, 5),
					new Pair<int, int>(0, 1),
					new Pair<int, int>(1, 9),
					new Pair<int, int>(5, 5),
					new Pair<int, int>(5, 7),
					new Pair<int, int>(10, 1)
				};
			var result = FindLongestNonOverlappingRangeSet(input);
			result.Count.ShouldBeEqualTo(2);
			result.First().ShouldBeSameInstanceAs(input[0]);
			result.Last().ShouldBeSameInstanceAs(input[4]);
		}

		[Test]
		public void Given_the_second_problem_sample()
		{
			var input = new[]
				{
					new Pair<int, int>(0, 1),
					new Pair<int, int>(1, 7),
					new Pair<int, int>(3, 20),
					new Pair<int, int>(8, 5)
				};
			var result = FindLongestNonOverlappingRangeSet(input);
			result.Count.ShouldBeEqualTo(1);
			result.First().ShouldBeSameInstanceAs(input[2]);
		}

		/// <summary>
		/// create a hashtable of start->list of tuples that start there
		/// put all tuples in a queue of tupleSets
		/// set the longestTupleSet to the first tuple
		/// while the queue is not empty
		/// 	take tupleSet from the queue
		/// 	if any tuples start where the tupleSet ends
		/// 		foreach tuple that starts where the tupleSet ends
		/// 			enqueue new tupleSet of tupleSet + tuple
		/// 		continue
		/// 
		/// 	if tupleSet is longer than longestTupleSet
		/// 		replace longestTupleSet with tupleSet
		/// 
		/// return longestTupleSet
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		private static IList<Pair<int, int>> FindLongestNonOverlappingRangeSet(IList<Pair<int, int>> input)
		{
			var rangeStarts = new Dictionary<int, List<Pair<int, int>>>();
			var adjacentTuples = new Queue<List<Pair<int, int>>>();
			foreach (var tuple in input)
			{
				adjacentTuples.Enqueue(new List<Pair<int, int>>
					{
						tuple
					});
				int start = tuple.First;
				List<Pair<int, int>> sameStart;
				if (!rangeStarts.TryGetValue(start, out sameStart))
				{
					sameStart = new List<Pair<int, int>>();
					rangeStarts.Add(start, sameStart);
				}
				sameStart.Add(tuple);
			}

			var longest = new List<Pair<int, int>>
				{
					input[0]
				};
			int longestLength = input[0].Second - input[0].First;

			while (adjacentTuples.Count > 0)
			{
				var tupleSet = adjacentTuples.Dequeue();
				var last = tupleSet.Last();
				List<Pair<int, int>> sameStart;
				int end = last.First + last.Second;
				if (rangeStarts.TryGetValue(end, out sameStart))
				{
					foreach (var nextTuple in sameStart)
					{
						adjacentTuples.Enqueue(tupleSet.Concat(new[] { nextTuple }).ToList());
					}
					continue;
				}
				int length = end - tupleSet.First().First;
				if (length > longestLength)
				{
					longestLength = length;
					longest = tupleSet;
				}
			}

			return longest;
		}
	}
}