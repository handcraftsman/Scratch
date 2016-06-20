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
	public class Demo
	{
		[Test]
		public void Given_Sample_A()
		{
			var target = new[] { "00", "01", "10" };
			var dontMatch = new[] { "11" };

			GenerateRegex(target, dontMatch, 6);
		}
	
		[Test]
		public void Should_only_match_alphabetical_once_ignoring_whitespace()
		{
			var target = new[] { "abc", "abcdefghijk", "abdfkmnpstvxz", "cxy", "cdklstxy", 
				"bfrtw", "a b c", "acg jko pr", "a z", "v  z",
			"a  b cdefg kl", "uv xyz", "ab de gh", "x yz", "abcdefghijklmnopqrstuvwxyz"};
			var dontMatch = new[] { "abbc", "abcb", "a bcdjkrza", "qwerty", "zyxcba", 
				"abcdfe", "ab c dfe", "a  z  a", "asdfg", "asd  f g", "poqwoieruytjhfg" };

			GenerateRegex(target, dontMatch, 30);
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

		[Test]
		public void Given_Sample_D()
		{
			// http://stackoverflow.com/questions/15807365/find-simplest-regular-expression-matching-all-given-strings?rq=1
			var target = new[] { "h_q1_a", "h_q1_b", "h_q1_c", "h_p2_a", "h_p2_b", "h_p2_c" };
			var dontMatch = new string[] { };

			GenerateRegex(target, dontMatch, 16);
		}

		private static void GenerateRegex(IEnumerable<string> target, IEnumerable<string> dontMatch, int expectedLength)
		{
			string distinctSymbols = new String(target.SelectMany(x => x).Distinct().ToArray());
			string genes = distinctSymbols + "?*()[^]+";

			Func<string, FitnessResult> calcFitness = str =>
				{
					if (!IsValidRegex(str))
					{
					    return new FitnessResult
					        {
					            Value = Int32.MaxValue
					        };
					}
					var regex = new Regex("^" + str + "$");
					uint fitness = target.Aggregate<string, uint>(0, (current, t) => current + (regex.IsMatch(t) ? 0U : 1));
					uint nonFitness = dontMatch.Aggregate<string, uint>(0, (current, t) => current + (regex.IsMatch(t) ? 10U : 0));
				    return new FitnessResult
				        {
				            Value = fitness + nonFitness
				        };
				};

			int targetGeneLength = 1;
			for (;;)
			{
			    var best = new GeneticSolver(50 + 10 * targetGeneLength).GetBestGenetically(targetGeneLength, genes, calcFitness);
				if (calcFitness(best.GetStringGenes()).Value != 0)
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