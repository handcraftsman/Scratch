using System;
using System.Linq;

using FluentAssert;

using NUnit.Framework;

namespace Scratch.StringContainsAll
{
	[TestFixture]
	public class Tests
	{
		[Test]
		public void SLOW_from_WSOL__should_be_true()
		{
			"SLOW".IsStringMadeOf("SWOL").ShouldBeTrue();
			Repeat(() => "SLOW".IsStringMadeOf("SWOL"), 10000);
		}
		[Test]
		public void SLOW_from_WTOL__should_be_false()
		{
			"SLOW".IsStringMadeOf("WTOL").ShouldBeFalse();
			Repeat(() => "SLOW".IsStringMadeOf("WTOL"), 10000);
		}
		[Test]
		public void ASIA_from_XZYABSTRIB__should_be_false()
		{
			"ASIA".IsStringMadeOf("XZYABSTRIB").ShouldBeFalse();
			Repeat(() => "ASIA".IsStringMadeOf("XZYABSTRIB"), 10000);
		}		
		
		[Test]
		public void ASIA_from_XZYABSTRIAB__should_be_true()
		{
			"ASIA".IsStringMadeOf("XZYABSTRIAB").ShouldBeTrue();
			Repeat(() => "ASIA".IsStringMadeOf("XZYABSTRIAB"), 10000);
		}

		private void Repeat(Action action, int times)
		{
			for (int i = 0; i < times; i++)
			{
				action();
			}
		}
	}

	public static class StringExtensions
	{
		public static bool IsStringMadeOf(this string str, string from)
		{
			var strLookup = str.GroupBy(x=>x).ToDictionary(x=>x.Key,x=>x.Count());
			var fromLookup = from.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
			var result = strLookup.All(x => fromLookup.ContainsKey(x.Key) && fromLookup[x.Key] >= x.Value);

			return result;
		}
	}
}