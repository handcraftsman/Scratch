//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************

using FluentAssert;

using NUnit.Framework;

namespace Scratch.ListPermutation
{
	public class NumericTests
	{
		/// <summary>
		///     factorial values from
		///     http://en.wikipedia.org/wiki/Factorial
		/// </summary>
		[TestFixture]
		public class When_asked_to_calculate_factorial
		{
			[Test]
			public void Given_0_should_return_1()
			{
				const int input = 0;
				const int expect = 1;
				int result = Numeric.Factorial(input);
				result.ShouldBeEqualTo(expect);
			}

			[Test]
			public void Given_10_should_return_3628800()
			{
				const int input = 10;
				const int expect = 3628800;
				int result = Numeric.Factorial(input);
				result.ShouldBeEqualTo(expect);
			}

			/// <summary>
			///     using google calculator
			///     http://www.google.com/search?sourceid=chrome&q=11+factorial
			/// </summary>
			[Test]
			public void Given_11_should_return_39916800()
			{
				const int input = 11;
				const int expect = 39916800;
				int result = Numeric.Factorial(input);
				result.ShouldBeEqualTo(expect);
			}

			/// <summary>
			///     using google calculator
			///     http://www.google.com/search?sourceid=chrome&q=12+factorial
			/// </summary>
			[Test]
			public void Given_12_should_return_479001600()
			{
				const int input = 12;
				const int expect = 479001600;
				int result = Numeric.Factorial(input);
				result.ShouldBeEqualTo(expect);
			}

			[Test]
			public void Given_1_should_return_1()
			{
				const int input = 1;
				const int expect = 1;
				int result = Numeric.Factorial(input);
				result.ShouldBeEqualTo(expect);
			}

			[Test]
			public void Given_2_should_return_2()
			{
				const int input = 2;
				const int expect = 2;
				int result = Numeric.Factorial(input);
				result.ShouldBeEqualTo(expect);
			}

			[Test]
			public void Given_3_should_return_6()
			{
				const int input = 3;
				const int expect = 6;
				int result = Numeric.Factorial(input);
				result.ShouldBeEqualTo(expect);
			}

			[Test]
			public void Given_4_should_return_24()
			{
				const int input = 4;
				const int expect = 24;
				int result = Numeric.Factorial(input);
				result.ShouldBeEqualTo(expect);
			}

			[Test]
			public void Given_5_should_return_120()
			{
				const int input = 5;
				const int expect = 120;
				int result = Numeric.Factorial(input);
				result.ShouldBeEqualTo(expect);
			}

			[Test]
			public void Given_6_should_return_720()
			{
				const int input = 6;
				const int expect = 720;
				int result = Numeric.Factorial(input);
				result.ShouldBeEqualTo(expect);
			}

			[Test]
			public void Given_7_should_return_5040()
			{
				const int input = 7;
				const int expect = 5040;
				int result = Numeric.Factorial(input);
				result.ShouldBeEqualTo(expect);
			}

			[Test]
			public void Given_8_should_return_40320()
			{
				const int input = 8;
				const int expect = 40320;
				int result = Numeric.Factorial(input);
				result.ShouldBeEqualTo(expect);
			}

			[Test]
			public void Given_9_should_return_362880()
			{
				const int input = 9;
				const int expect = 362880;
				int result = Numeric.Factorial(input);
				result.ShouldBeEqualTo(expect);
			}
		}
	}
}