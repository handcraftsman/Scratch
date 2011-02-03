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
using Scratch.ListPermutation;

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

			GenerateRegex(target, dontMatch);
		}

		[Test]
		public void Given_Sample_B()
		{
			var target = new[] { "00", "01", "11" };
			var dontMatch = new[] { "10" };

			GenerateRegex(target, dontMatch);
		}

		private static void GenerateRegex(IEnumerable<string> target, IEnumerable<string> dontMatch)
		{
			string distinctSymbols = new String(target.SelectMany(x => x).Distinct().ToArray());
			string genes = distinctSymbols + "?*()+";

			Func<string, uint> calcFitness = str =>
				{
					if (str.Count(x => x == '(') != str.Count(x => x == ')'))
					{
						return Int32.MaxValue;
					}
					if ("?*+".Any(x => str[0] == x))
					{
						return Int32.MaxValue;
					}
					if ("?*+?*+".ToArray().Permute(2)
						.Any(permutation => str.IndexOf(new string(permutation.ToArray())) != -1))
					{
						return Int32.MaxValue;
					}
					Regex regex;
					try
					{
						regex = new Regex("^" + str + "$");
					}
					catch (Exception)
					{
						return Int32.MaxValue;
					}
					uint fitness = target.Aggregate<string, uint>(0, (current, t) => current + (regex.IsMatch(t) ? 0U : 1));
					uint nonFitness = dontMatch.Aggregate<string, uint>(0, (current, t) => current + (regex.IsMatch(t) ? 10U : 0));
					return fitness + nonFitness;
				};

			for (int targetGeneLength = distinctSymbols.Length; targetGeneLength < genes.Length * 2; targetGeneLength++)
			{
				string best = new GeneticSolver(50).GetBestGenetically(targetGeneLength, genes, calcFitness, true);
				if (calcFitness(best) != 0)
				{
					Console.WriteLine("-- not solved with regex of length " + targetGeneLength);
					continue;
				}
				Console.WriteLine("solved with: " + best);
				break;
			}
		}
	}
}