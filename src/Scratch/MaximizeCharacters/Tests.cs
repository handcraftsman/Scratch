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

using NUnit.Framework;

using Scratch.GeneticAlgorithm;

namespace Scratch.MaximizeCharacters
{
	/// <summary>
	///     http://stackoverflow.com/questions/4606984/maximum-number-of-characters-using-keystrokes-a-cttrla-ctrlc-and-ctrlv
	/// </summary>
	[TestFixture]
	public class Tests
	{
		[Test]
		public void Find_best_with_10_characters()
		{
			FindBest(10);
		}

		[Test]
		public void Find_best_with_15_characters()
		{
			FindBest(15);
		}

		[Test]
		public void Find_best_with_20_characters()
		{
			FindBest(20);
		}

		[Test]
		public void Find_best_with_24_characters()
		{
			FindBest(24);
		}

		[Test]
		public void Find_best_with_25_characters()
		{
			FindBest(25);
		}

		[Test]
		public void Find_best_with_26_characters()
		{
			FindBest(26);
		}

		[Test]
		public void Find_best_with_27_characters()
		{
			FindBest(27);
		}

		[Test]
		public void Find_best_with_28_characters()
		{
			FindBest(28);
		}

		[Test]
		public void Find_best_with_4_characters()
		{
			FindBest(4);
		}

		[Test]
		public void Find_best_with_5_characters()
		{
			FindBest(5);
		}

		[Test]
		public void Find_best_with_6_characters()
		{
			FindBest(6);
		}

		[Test]
		public void Find_best_with_7_characters()
		{
			FindBest(7);
		}

		[Test]
		public void Find_best_with_8_characters()
		{
			FindBest(8);
		}

		[Test]
		public void Find_best_with_9_characters()
		{
			FindBest(9);
		}

		private static void FindBest(int numberOfCharacters)
		{
			Func<string, uint> calcFitness = x =>
				{
					string result = Run(x);
					return (uint)(Int32.MaxValue - result.Length);
				};
			string best = new GeneticSolver(2500).GetBestGenetically(numberOfCharacters, "ASCP", calcFitness, true);
			string finalString = Run(best);
			Console.WriteLine(best + " generatates final string with length " + finalString.Length);
		}

		private static string Run(IEnumerable<char> commands)
		{
			string result = "";
			string selected = "";
			string copyBuffer = "";
			foreach (char command in commands)
			{
				switch (command)
				{
					case 'A':
						result += "A";
						break;
					case 'S':
						selected = result;
						break;
					case 'C':
						copyBuffer = selected;
						break;
					case 'P':
						result += copyBuffer;
						selected = "";
						break;
				}
			}
			return result;
		}
	}
}