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

namespace Scratch.WhenDsl
{
	public class IntExtensionsTests
	{
		[TestFixture]
		public class When_asked_to_GetFizzBuzz
		{
			private List<int> _inputs;
			private List<Pair<int, string>> _result;

			[SetUp]
			public void BeforeEachTest()
			{
				_inputs = new List<int>();
			}

			[Test]
			public void Given_numbers_divisible_by_3_and_5_should_get__FizzBuzz()
			{
				Test.Verify(
					with_numbers,
					that_are_divisible_by_3,
					that_are_divisible_by_5,
					when_asked_to_get_FizzBuzz,
					should_get_same_number_of_outputs_as_inputs,
					should_get__FizzBuzz__for_each
					);
			}

			[Test]
			public void Given_numbers_divisible_by_3_that_are_indivisible_5_should_get__Fizz()
			{
				Test.Verify(
					with_numbers,
					that_are_divisible_by_3,
					that_are_not_divisible_by_5,
					when_asked_to_get_FizzBuzz,
					should_get_same_number_of_outputs_as_inputs,
					should_get__Fizz__for_each
					);
			}

			[Test]
			public void Given_numbers_indivisible_by_3_and_5_should_get_the_number()
			{
				Test.Verify(
					with_numbers,
					that_are_not_divisible_by_3,
					that_are_not_divisible_by_5,
					when_asked_to_get_FizzBuzz,
					should_get_same_number_of_outputs_as_inputs,
					should_get_the_number_for_each
					);
			}

			[Test]
			public void Given_numbers_indivisible_by_3_that_are_divisible_5_should_get__Buzz()
			{
				Test.Verify(
					with_numbers,
					that_are_not_divisible_by_3,
					that_are_divisible_by_5,
					when_asked_to_get_FizzBuzz,
					should_get_same_number_of_outputs_as_inputs,
					should_get__Buzz__for_each
					);
			}

			private void should_get__Buzz__for_each()
			{
				_result.All(x => x.Second == "Buzz").ShouldBeTrue();
			}

			private void should_get__FizzBuzz__for_each()
			{
				_result.All(x => x.Second == "FizzBuzz").ShouldBeTrue();
			}

			private void should_get__Fizz__for_each()
			{
				_result.All(x => x.Second == "Fizz").ShouldBeTrue();
			}

			private void should_get_same_number_of_outputs_as_inputs()
			{
				_result.Count.ShouldBeEqualTo(_inputs.Count);
			}

			private void should_get_the_number_for_each()
			{
				_result.All(x => x.Second == x.First.ToString()).ShouldBeTrue();
			}

			private void that_are_divisible_by_3()
			{
				_inputs.RemoveAll(x => x % 3 != 0);
			}

			private void that_are_divisible_by_5()
			{
				_inputs.RemoveAll(x => x % 5 != 0);
			}

			private void that_are_not_divisible_by_3()
			{
				_inputs.RemoveAll(x => x % 3 == 0);
			}

			private void that_are_not_divisible_by_5()
			{
				_inputs.RemoveAll(x => x % 5 == 0);
			}

			private void when_asked_to_get_FizzBuzz()
			{
				_result = _inputs.GetFizzBuzz().ToList();
			}

			private void with_numbers()
			{
				_inputs.AddRange(Enumerable.Range(1, 100));
			}
		}
	}
}