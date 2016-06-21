using System.Collections.Generic;
using System.Linq;
using FluentAssert;
using NUnit.Framework;

namespace Scratch.RomanNumerals
{
	/// <summary>
	/// inspired by: http://www.sandimetz.com/blog/2016/6/9/make-everything-the-same
	/// </summary>
	public static class Roman
	{
		private static readonly IDictionary<int, string> Numerals = new Dictionary<int, string>
		{
			{1, "I"},
			{4, "IV"},
			{5, "V"},
			{9, "IX"},
			{10, "X"},
			{40, "XL"},
			{50, "L"},
			{90, "XC"},
			{100, "C"},
			{400, "CD"},
			{500, "D"},
			{900, "CM"},
			{1000, "M"}
		};

		public static int FromRoman(this string roman)
		{
			var value = 0;
			foreach (var kvp in Numerals.OrderByDescending(x => x.Key))
			{
				while (roman.StartsWith(kvp.Value))
				{
					value += kvp.Key;
					roman = roman.Substring(kvp.Value.Length);
				}
			}
			return value;
		}

		public static string ToRoman(this int i)
		{
			var result = "";
			foreach (var kvp in Numerals.OrderByDescending(x => x.Key))
			{
				while (i >= kvp.Key)
				{
					result += kvp.Value;
					i -= kvp.Key;
				}
			}
			return result;
		}
	}

	public class RomanTests
	{
		[TestFixture]
		public class When_asked_to_convert_and_integer_value_to_roman_numerals
		{
			[Test]
			public void Given_1_should_get_I()
			{
				1.ToRoman().ShouldBeEqualTo("I");
			}

			[Test]
			public void Given_2_should_get_II()
			{
				2.ToRoman().ShouldBeEqualTo("II");
			}

			[Test]
			public void Given_4_should_get_IV()
			{
				4.ToRoman().ShouldBeEqualTo("IV");
			}

			[Test]
			public void Given_5_should_get_V()
			{
				5.ToRoman().ShouldBeEqualTo("V");
			}

			[Test]
			public void Given_9_should_get_IX()
			{
				9.ToRoman().ShouldBeEqualTo("IX");
			}

			[Test]
			public void Given_10_should_get_X()
			{
				10.ToRoman().ShouldBeEqualTo("X");
			}

			[Test]
			public void Given_40_should_get_XL()
			{
				40.ToRoman().ShouldBeEqualTo("XL");
			}

			[Test]
			public void Given_50_should_get_L()
			{
				50.ToRoman().ShouldBeEqualTo("L");
			}

			[Test]
			public void Given_90_should_get_XC()
			{
				90.ToRoman().ShouldBeEqualTo("XC");
			}

			[Test]
			public void Given_100_should_get_C()
			{
				100.ToRoman().ShouldBeEqualTo("C");
			}

			[Test]
			public void Given_400_should_get_CD()
			{
				400.ToRoman().ShouldBeEqualTo("CD");
			}

			[Test]
			public void Given_500_should_get_D()
			{
				500.ToRoman().ShouldBeEqualTo("D");
			}

			[Test]
			public void Given_900_should_get_CM()
			{
				900.ToRoman().ShouldBeEqualTo("CM");
			}

			[Test]
			public void Given_1000_should_get_M()
			{
				1000.ToRoman().ShouldBeEqualTo("M");
			}
		}

		[TestFixture]
		public class When_asked_to_convert_roman_numerals_to_an_integer
		{
			[Test]
			public void Given_I_should_get_1()
			{
				"I".FromRoman().ShouldBeEqualTo(1);
			}

			[Test]
			public void Given_II_should_get_2()
			{
				"II".FromRoman().ShouldBeEqualTo(2);
			}

			[Test]
			public void Given_IV_should_get_4()
			{
				"IV".FromRoman().ShouldBeEqualTo(4);
			}
		}

		[TestFixture]
		public class When_asked_to_round_trip_integer_to_roman_numerals_to_integer
		{
			[Test]
			public void Should_be_able_to_convert_all_integers_between_1_and_10000()
			{
				for (var input = 1; input <= 10000; input++)
				{
					var result = input.ToRoman();
					var roundTripped = result.FromRoman();
					input.ShouldBeEqualTo(roundTripped);
				}
			}
		}
	}
}