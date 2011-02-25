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

using Scratch.Ranges.RangeEnumeration;

namespace Scratch.WhenDsl
{
	public static class IntExtensions
	{
		private static Action<Pair<int, string>> AppendBuzzToResult
		{
			get { return y => y.Second += "Buzz"; }
		}
		private static Func<Pair<int, string>, bool> NumberIsDivisibleBy3
		{
			get { return y => y.First % 3 == 0; }
		}

		private static Func<Pair<int, string>, bool> NumberIsDivisibleBy3Or5
		{
			get { return y => NumberIsDivisibleBy3(y) || NumberIsDivisibleBy5(y); }
		}

		private static Func<Pair<int, string>, bool> NumberIsDivisibleBy5
		{
			get { return y => y.First % 5 == 0; }
		}

		private static Action<Pair<int, string>> SetResultToFizz
		{
			get { return y => y.Second = "Fizz"; }
		}

		private static Action<Pair<int, string>> SetResultToTheNumber
		{
			get { return y => y.Second = y.First.ToString(); }
		}

		public static IEnumerable<Pair<int, string>> GetFizzBuzz(this IEnumerable<int> values)
		{
			return values
				.Select(x => new Pair<int, string>(x, "")
				             	.When(NumberIsDivisibleBy3,
				             	      SetResultToFizz)
				             	.When(NumberIsDivisibleBy5,
				             	      AppendBuzzToResult)
				             	.Unless(NumberIsDivisibleBy3Or5,
				             	        SetResultToTheNumber));
		}
	}
}