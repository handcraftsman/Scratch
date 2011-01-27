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

namespace Scratch.ListPermutation
{
	/// <summary>
	///     http://handcraftsman.wordpress.com/2010/11/11/generating-all-permutations-of-a-sequence-in-csharp/
	/// </summary>
	public static class Numeric
	{
		public static int Factorial(int n)
		{
			if (n < 0)
			{
				throw new ArgumentOutOfRangeException("n", "input must be greater than or equal to 0");
			}
			if (n == 0)
			{
				return 1;
			}
			int result = (int)Gamma(n + 1) + 1;
			return result;
		}

		/// <summary>
		/// ``Gergo Nemes’s approximation to Stirling’s approximation to the Gamma Function''
		///     http://chaosinmotion.com/blog/?p=622
		/// references: http://en.wikipedia.org/wiki/Stirling%27s_approximation#A_version_suitable_for_calculators
		/// </summary>
		/// <param name = "z"></param>
		/// <returns></returns>
		public static double Gamma(double z)
		{
			double tmp1 = Math.Sqrt(2 * Math.PI / z);
			double tmp2 = z + 1.0 / (12 * z - 1.0 / (10 * z));
			tmp2 = Math.Pow(tmp2 / Math.E, z);
			return tmp1 * tmp2;
		}
	}
}