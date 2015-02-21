using System;

namespace Scratch.Exercism.csharp.leap
{
	public static class Year
	{
		public static bool IsLeap(int year)
		{
			if (year < 0)
			{
				throw new ArgumentException("Year must be greater than or equal to zero.", "year");
			}

			if (!year.IsDivisibleBy(4))
			{
				return false; // leap years are divisible by 4
			}

			if (!year.IsDivisibleBy(100))
			{
				return true; // leap years are not divisible by 100
			}

			return year.IsDivisibleBy(400); // unless divisible by 400
		}
	}

	public static class IntExtensions
	{
		public static bool IsDivisibleBy(this int numerator, int denominator)
		{
			return numerator % denominator == 0;
		}
	}
}