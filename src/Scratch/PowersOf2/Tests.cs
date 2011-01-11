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

using NUnit.Framework;

namespace Scratch.PowersOf2
{
	public class Tests
	{
		/// <summary>
		///     http://stackoverflow.com/questions/53161/find-the-highest-order-bit-in-c
		/// </summary>
		[TestFixture]
		public class Power_of_2_less_than_or_equal_to_input
		{
			[Test]
			public void Using_Log()
			{
				for (int i = 0; i < 64; i++)
				{
					Console.WriteLine(i + "\t" + (i == 0 ? i : (1 << (int)Math.Log(i, 2))));
				}
			}
		}

		/// <summary>
		///     http://stackoverflow.com/questions/53161/find-the-highest-order-bit-in-c
		/// </summary>
		[TestFixture]
		public class Power_of_2_less_than_the_input
		{
			[Test]
			public void Inline()
			{
				for (int i = -2; i < 10; i++)
				{
					int nextLowerPowerOf2 = i <= 0
					                        	? 0
					                        	: ((i & (~i + 1)) == i)
					                        	  	? i >> 1
					                        	  	: (1 << (int)Math.Log(i, 2));
					Console.WriteLine(i + "\t" + nextLowerPowerOf2);
				}
			}

			[Test]
			public void Using_methods()
			{
				for (int i = -2; i < 10; i++)
				{
					int nextLowerPowerOf2 = IsPowerOfTwo(i)
					                        	? i >> 1
					                        	: GetPowerOfTwoLessThanOrEqualTo(i);
					Console.WriteLine(i + "\t" + nextLowerPowerOf2);
				}
			}

			private static int GetPowerOfTwoLessThanOrEqualTo(int x)
			{
				return (x <= 0 ? 0 : (1 << (int)Math.Log(x, 2)));
			}

			private static bool IsPowerOfTwo(int x)
			{
				return (((x & (~x + 1)) == x) && (x > 0));
			}
		}
	}
}