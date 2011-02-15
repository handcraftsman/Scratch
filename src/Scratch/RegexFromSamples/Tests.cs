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
using System.Text.RegularExpressions;

using NUnit.Framework;

using Scratch.GeneticAlgorithm;

namespace Scratch.RegexFromSamples
{
	/// <summary>
	///     http://stackoverflow.com/questions/4880402/how-to-auto-generate-regex-from-given-list-of-strings
	/// </summary>
	[TestFixture]
	public class Tests
	{
		[Test]
		public void Given_Sample_A()
		{
			var target = new[] { "00", "01", "10" };
			var dontMatch = new[] { "11" };

			GenerateRegex(target, dontMatch, 6);
		}

		[Test]
		public void Given_Sample_B()
		{
			var target = new[] { "00", "01", "11" };
			var dontMatch = new[] { "10" };

			GenerateRegex(target, dontMatch, 4);
		}

		[Test]
		public void Given_Sample_C()
		{
			var target = new[] { "0", "00", "0000", "00000" };
			var dontMatch = new[] { "000", "000000" };

			GenerateRegex(target, dontMatch, 9);
		}

		private static void GenerateRegex(IEnumerable<string> target, IEnumerable<string> dontMatch, int expectedLength)
		{
			string distinctSymbols = new String(target.SelectMany(x => x).Distinct().ToArray());
			string genes = distinctSymbols + "?*()+";

			Func<string, uint> calcFitness = str =>
				{
					if (!IsValidRegex(str))
					{
						return Int32.MaxValue;
					}
					var regex = new Regex("^" + str + "$");
					uint fitness = target.Aggregate<string, uint>(0, (current, t) => current + (regex.IsMatch(t) ? 0U : 1));
					uint nonFitness = dontMatch.Aggregate<string, uint>(0, (current, t) => current + (regex.IsMatch(t) ? 10U : 0));
					return fitness + nonFitness;
				};

			int targetGeneLength = 1;
			for (;;)
			{
				string best = new GeneticSolver(50+10*targetGeneLength).GetBestGenetically(targetGeneLength, genes, calcFitness, true);
				if (calcFitness(best) != 0)
				{
					Console.WriteLine("-- not solved with regex of length " + targetGeneLength);
					targetGeneLength++;
                    if (targetGeneLength > expectedLength)
                    {
                        Assert.Fail("failed to find a solution within the expected length");
                    }
					continue;
				}
				Console.WriteLine("solved with: " + best);
				break;
			}
		}

		private static bool HasBalancedParentheses(string str)
		{
			int depth = 0;
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i] == ')')
				{
					depth--;
					if (depth < 0)
					{
						return false;
					}
				}
				if (str[i] == '(')
				{
					depth++;
				}
			}
			return (depth == 0);
		}

		private static bool IsValidRegex(string str)
		{
			if (!HasBalancedParentheses(str))
			{
				return false;
			}
			if (str.All(x => "?*+()".Contains(x)))
			{
				return false;
			}
			if (")?*+".Any(x => str.First() == x))
			{
				return false;
			}
			if (str.Last() == ')')
			{
				return false;
			}
			if (str.Contains("()"))
			{
				return false;
			}
			if (str.Contains("(*") || str.Contains("(+") || str.Contains("(?"))
			{
				return false;
			}
			if (str.Contains("?*") || str.Contains("?+") || str.Contains("??") || str.Contains("*?"))
			{
				return false;
			}
			if (str.Contains("++") || str.Contains("**") || str.Contains("+*") || str.Contains("*+"))
			{
				return false;
			}
			try
			{
				new Regex("^" + str + "$");
			}
			catch (Exception)
			{
				Console.WriteLine("-- bad: " + str);
				return false;
			}
			return true;
		}
	}
}