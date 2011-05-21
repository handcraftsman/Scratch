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

using FluentAssert;

using NUnit.Framework;

namespace Scratch.SequentialLinq
{
	/// <summary>
	///     http://stackoverflow.com/questions/5908606/how-to-use-linq-to-compare-values-of-sequential-neighbors-inside-a-list
	/// </summary>
	[TestFixture]
	public class Tests
	{
		[Test]
		public void Should_convert_all_low_probability_colors()
		{
			var colors = new List<ColorResult>
				{
					new ColorResult(1, "Unknown", 5f),
					new ColorResult(2, "Blue", 80f),
					new ColorResult(3, "Blue", 80f),
					new ColorResult(4, "Green", 40f),
					new ColorResult(5, "Blue", 80f),
					new ColorResult(6, "Blue", 80f),
					new ColorResult(7, "Red", 20f),
					new ColorResult(8, "Blue", 80f),
					new ColorResult(9, "Green", 5f)
				};

			ConvertLowProbabilityColors(colors);

			foreach (var colorResult in colors)
			{
				Console.WriteLine(colorResult.Index + " " + colorResult.Color);
			}

			colors[0].Color.ShouldBeEqualTo("Blue");
			colors[1].Color.ShouldBeEqualTo("Blue");
			colors[2].Color.ShouldBeEqualTo("Blue");
			colors[3].Color.ShouldBeEqualTo("Blue");
			colors[4].Color.ShouldBeEqualTo("Blue");
			colors[5].Color.ShouldBeEqualTo("Blue");
			colors[6].Color.ShouldBeEqualTo("Red");
			colors[7].Color.ShouldBeEqualTo("Blue");
			colors[8].Color.ShouldBeEqualTo("Green");
		}

		[Test]
		public void Should_convert_leading_low_probability_colors()
		{
			var colors = new List<ColorResult>
				{
					new ColorResult(1, "Unknown", 5f),
					new ColorResult(2, "Blue", 80f),
					new ColorResult(3, "Blue", 80f),
					new ColorResult(4, "Green", 40f),
					new ColorResult(5, "Blue", 80f),
					new ColorResult(6, "Blue", 80f),
					new ColorResult(7, "Red", 20f),
					new ColorResult(8, "Blue", 80f),
					new ColorResult(9, "Green", 5f)
				};

			ConvertLeadingLowProbabilityColors(colors);

			foreach (var colorResult in colors)
			{
				Console.WriteLine(colorResult.Index + " " + colorResult.Color);
			}

			colors[0].Color.ShouldBeEqualTo("Blue");
			colors[1].Color.ShouldBeEqualTo("Blue");
			colors[2].Color.ShouldBeEqualTo("Blue");
			colors[3].Color.ShouldBeEqualTo("Green");
			colors[4].Color.ShouldBeEqualTo("Blue");
			colors[5].Color.ShouldBeEqualTo("Blue");
			colors[6].Color.ShouldBeEqualTo("Red");
			colors[7].Color.ShouldBeEqualTo("Blue");
			colors[8].Color.ShouldBeEqualTo("Green");
		}

		[Test]
		public void Should_convert_surrounded_low_probability_colors()
		{
			var colors = new List<ColorResult>
				{
					new ColorResult(1, "Unknown", 5f),
					new ColorResult(2, "Blue", 80f),
					new ColorResult(3, "Blue", 80f),
					new ColorResult(4, "Green", 40f),
					new ColorResult(5, "Blue", 80f),
					new ColorResult(6, "Blue", 80f),
					new ColorResult(7, "Red", 20f),
					new ColorResult(8, "Blue", 80f),
					new ColorResult(9, "Green", 5f)
				};

			ConvertSurroundedLowProbabilityColors(colors);

			foreach (var colorResult in colors)
			{
				Console.WriteLine(colorResult.Index + " " + colorResult.Color);
			}

			colors[0].Color.ShouldBeEqualTo("Unknown");
			colors[1].Color.ShouldBeEqualTo("Blue");
			colors[2].Color.ShouldBeEqualTo("Blue");
			colors[3].Color.ShouldBeEqualTo("Blue");
			colors[4].Color.ShouldBeEqualTo("Blue");
			colors[5].Color.ShouldBeEqualTo("Blue");
			colors[6].Color.ShouldBeEqualTo("Red");
			colors[7].Color.ShouldBeEqualTo("Blue");
			colors[8].Color.ShouldBeEqualTo("Green");
		}

		[Test]
		public void Should_convert_trailing_low_probability_colors()
		{
			var colors = new List<ColorResult>
				{
					new ColorResult(1, "Unknown", 5f),
					new ColorResult(2, "Blue", 80f),
					new ColorResult(3, "Blue", 80f),
					new ColorResult(4, "Green", 40f),
					new ColorResult(5, "Blue", 80f),
					new ColorResult(6, "Blue", 80f),
					new ColorResult(7, "Red", 20f),
					new ColorResult(8, "Blue", 40f),
					new ColorResult(9, "Green", 5f)
				};

			ConvertTrailingLowProbabilityColors(colors);

			foreach (var colorResult in colors)
			{
				Console.WriteLine(colorResult.Index + " " + colorResult.Color);
			}

			colors[0].Color.ShouldBeEqualTo("Unknown");
			colors[1].Color.ShouldBeEqualTo("Blue");
			colors[2].Color.ShouldBeEqualTo("Blue");
			colors[3].Color.ShouldBeEqualTo("Green");
			colors[4].Color.ShouldBeEqualTo("Blue");
			colors[5].Color.ShouldBeEqualTo("Blue");
			colors[6].Color.ShouldBeEqualTo("Blue");
			colors[7].Color.ShouldBeEqualTo("Blue");
			colors[8].Color.ShouldBeEqualTo("Blue");
		}

		public class ColorResult
		{
			public string Color;
			public int Index;
			public string Name;
			public float Probability;

			public ColorResult(int index, string color, float probability)
			{
				Index = index;
				Color = color;
				Probability = probability;
			}

			public override string ToString()
			{
				return String.Format("{0}, {1}, {2}", Index, Name, Probability);
			}
		}

		private void ConvertLeadingLowProbabilityColors(IList<ColorResult> colors)
		{
			var leadingBelow60 = Enumerable
				.Range(0, colors.Count)
				.TakeWhile(index => colors[index].Probability < 60)
				.ToList();
			if (leadingBelow60.Count > 0 && leadingBelow60.Count < colors.Count - 2)
			{
				int lastIndex = leadingBelow60.Last();
				var firstNext = colors[lastIndex + 1];
				var secondNext = colors[lastIndex + 2];
				if (firstNext.Probability > 60 &&
				    secondNext.Probability > 60 &&
				    firstNext.Color == secondNext.Color)
				{
					leadingBelow60.ForEach(index => colors[index].Color = firstNext.Color);
				}
			}
		}

		public void ConvertLowProbabilityColors(IList<ColorResult> colors)
		{
			ConvertLeadingLowProbabilityColors(colors);
			ConvertSurroundedLowProbabilityColors(colors);
			ConvertTrailingLowProbabilityColors(colors);
		}

		private void ConvertSurroundedLowProbabilityColors(IList<ColorResult> colors)
		{
			var surrounding4Modification = new Surrounding4ModificationStrategy();
			foreach (int index in Enumerable
				.Range(0, colors.Count)
				.Where(index => surrounding4Modification.IsMatch(colors, index)))
			{
				surrounding4Modification.Update(colors, index);
			}
		}

		private void ConvertTrailingLowProbabilityColors(IList<ColorResult> colors)
		{
			var trailingBelow60 = Enumerable
				.Range(0, colors.Count)
				.Select(i => colors.Count - 1 - i)
				.TakeWhile(index => colors[index].Probability < 60)
				.ToList();
			if (trailingBelow60.Count > 0 && trailingBelow60.Count < colors.Count - 2)
			{
				int lastIndex = trailingBelow60.Last();
				var firstPrevious = colors[lastIndex - 1];
				var secondPrevious = colors[lastIndex - 2];
				if (firstPrevious.Probability > 60 &&
				    secondPrevious.Probability > 60 &&
				    firstPrevious.Color == secondPrevious.Color)
				{
					trailingBelow60.ForEach(index => colors[index].Color = firstPrevious.Color);
				}
			}
		}

		public class Surrounding4ModificationStrategy
		{
			public bool IsMatch(IList<ColorResult> input, int index)
			{
				if (index < 2)
				{
					return false;
				}
				if (index >= input.Count - 2)
				{
					return false;
				}
				if (input[index].Probability >= 60)
				{
					return false;
				}

				var secondPrevious = input[index - 2];
				if (secondPrevious.Probability < 60)
				{
					return false;
				}
				var firstPrevious = input[index - 1];
				if (firstPrevious.Probability < 60)
				{
					return false;
				}

				var firstNext = input[index + 1];
				if (firstNext.Probability < 60)
				{
					return false;
				}
				var secondNext = input[index + 2];
				if (secondNext.Probability < 60)
				{
					return false;
				}

				if (new[] { secondPrevious.Color, firstPrevious.Color, firstNext.Color, secondNext.Color }.Distinct().Count() > 1)
				{
					return false;
				}
				return true;
			}

			public void Update(IList<ColorResult> input, int index)
			{
				input[index].Color = input[index + 1].Color;
			}
		}
	}
}