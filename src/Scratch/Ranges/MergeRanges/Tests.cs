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

namespace Scratch.Ranges.MergeRanges
{
	/// <summary>
	///     http://stackoverflow.com/questions/4593583/algorithm-to-find-longest-non-overlapping-sequences
	///     Pairs are start and length
	/// </summary>
	[TestFixture]
	public class Tests
	{
		[Test]
		public void Given_non_overlapping_ranges()
		{
			var input = new[]
				{
					new Pair<int, int>(0, 5),
					new Pair<int, int>(5, 7)
				};
			var result = Merge(input);
			result.Count.ShouldBeEqualTo(2);
			result.First().ShouldBeSameInstanceAs(input[0]);
			result.Last().ShouldBeSameInstanceAs(input[1]);
		}

		[Test]
		public void Given_the_PhilH_sample()
		{
			var input = new[]
				{
					new Pair<int, int>(0, 5),
					new Pair<int, int>(0, 7),
					new Pair<int, int>(5, 7),
					new Pair<int, int>(7, 2),
					new Pair<int, int>(9, 3),
					new Pair<int, int>(12, 3)
				};
			var result = Merge(input);
			result.Count.ShouldBeEqualTo(3);
			result.First().ShouldBeSameInstanceAs(input[1]);
			result[1].ShouldBeSameInstanceAs(input[2]);
			result.Last().ShouldBeSameInstanceAs(input[5]);
		}

		[Test]
		public void Given_the_problem_sample()
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
			var result = Merge(input);
			result.Count.ShouldBeEqualTo(2);
			result.First().ShouldBeSameInstanceAs(input[0]);
			result.Last().ShouldBeSameInstanceAs(input[4]);
		}

		[Test]
		public void Given_three_ranges_where_first_and_third_are_adjacent_and_middle_starts_in_first_and_ends_in_second()
		{
			var input = new[]
				{
					new Pair<int, int>(0, 5),
					new Pair<int, int>(3, 5),
					new Pair<int, int>(5, 7)
				};
			var result = Merge(input);
			result.Count.ShouldBeEqualTo(2);
			result.First().ShouldBeSameInstanceAs(input[0]);
			result.Last().ShouldBeSameInstanceAs(input[2]);
		}

		[Test]
		public void Given_two_ranges_that_start_at_same_point_and_second_ends_after_first()
		{
			var input = new[]
				{
					new Pair<int, int>(0, 5),
					new Pair<int, int>(0, 6)
				};
			var result = Merge(input);
			result.Count.ShouldBeEqualTo(1);
			result.First().ShouldBeSameInstanceAs(input[1]);
		}

		[Test]
		public void Given_two_ranges_that_start_at_same_point_and_second_ends_before_first()
		{
			var input = new[]
				{
					new Pair<int, int>(0, 5),
					new Pair<int, int>(0, 1)
				};
			var result = Merge(input);
			result.Count.ShouldBeEqualTo(1);
			result.First().ShouldBeSameInstanceAs(input[0]);
		}

		[Test]
		public void Given_two_ranges_where_second_starts_after_first_and_ends_after_first()
		{
			var input = new[]
				{
					new Pair<int, int>(0, 5),
					new Pair<int, int>(1, 5)
				};
			var result = Merge(input);
			result.Count.ShouldBeEqualTo(2);
			result.First().ShouldBeSameInstanceAs(input[0]);
			result.Last().ShouldBeSameInstanceAs(input[1]);
		}

		[Test]
		public void Given_two_ranges_where_second_starts_after_first_and_ends_before_first()
		{
			var input = new[]
				{
					new Pair<int, int>(0, 5),
					new Pair<int, int>(1, 2)
				};
			var result = Merge(input);
			result.Count.ShouldBeEqualTo(1);
			result.First().ShouldBeSameInstanceAs(input[0]);
		}

		/// <summary>
		///     assumes pairs are sorted by the start element
		/// </summary>
		/// <param name = "input"></param>
		/// <returns></returns>
		public static IList<Pair<int, int>> Merge(Pair<int, int>[] input)
		{
			var result = new List<Pair<int, int>>
				{
					input[0]
				};
			for (int i = 1; i < input.Length; i++)
			{
				var tuple = input[i];
				var last = result.Last();
				if (tuple.First >= last.First + last.Second)
				{
					result.Add(tuple);
					continue;
				}

				if (tuple.First + tuple.Second <= last.First + last.Second)
				{
					continue;
				}

				if (tuple.First == last.First)
				{
					if (tuple.First + tuple.Second > last.First + last.Second)
					{
						result[result.Count - 1] = tuple;
						continue;
					}
				}

				if (result.Count > 1)
				{
					var penultimate = result[result.Count - 2];
					if (penultimate.First + penultimate.Second >= tuple.First)
					{
						result[result.Count - 1] = tuple;
						continue;
					}
				}

				result.Add(tuple);
			}
			return result;
		}
	}
}