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

using NUnit.Framework;

using FluentAssert;

namespace Scratch.GroupBySequenceDirection
{
	public static class TExtensions
	{
		public static IEnumerable<T> Generate<T>(this T initial, Func<T, T> next)
		{
			var current = initial;
			while (true)
			{
				yield return current;
				current = next(current);
			}
		}
	}

	[TestFixture]
	public class Tests
	{
		public class ExampleClass
		{
			public decimal TheValue { get; set; }
		}

		[Test]
		public void Test()
		{
			var input = new[] { 0, 1, 2, 3, 1, 1, 4, 6, 7, 0, 1, 0, 2, 3, 5, 7, 6, 5, 4, 3, 2, 1 }
				.Select(x=>new ExampleClass{TheValue = x})
				.ToList();

			var grouped = SplitSequences(input).ToList();
			grouped.Count.ShouldBeEqualTo(5);
			grouped[0].Count().ShouldBeEqualTo(4);
			grouped[1].Count().ShouldBeEqualTo(5);
			grouped[2].Count().ShouldBeEqualTo(2);
			grouped[3].Count().ShouldBeEqualTo(5);
			grouped[4].Count().ShouldBeEqualTo(6);
			foreach (var group in grouped)
			{
				Console.WriteLine(String.Join(",",group.Select(x=>x.TheValue.ToString()).ToArray()));
			}

		}

		private IEnumerable<IEnumerable<ExampleClass>> SplitSequences(IList<ExampleClass> input)
		{
			var directions = input
				.Select((x, i) => i == 0 ? 0 : Math.Sign(input[i].TheValue.CompareTo(input[i - 1].TheValue)))
				.ToList();

			var result = new List<ExampleClass>();
			int? direction = null;
			for(int i = 0; i < input.Count; i++)
			{
				if (directions[i] == 0)
				{
					result.Add(input[i]);
				}
				else
				{
					if (!direction.HasValue)
					{
						direction = directions[i];
					}
					if (directions[i] == direction.Value)
					{
						result.Add(input[i]);
					}
					else
					{
						yield return result;
						direction = null;
						result = new List<ExampleClass>
							{
								input[i]
							};
					}
				}
			}

			if (result.Count > 0)
			{
				yield return result;
			}
		}
	}
}