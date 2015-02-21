using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using FluentAssert;

using NUnit.Framework;

namespace Scratch.Rectangle
{
	[TestFixture]
	public class Demo
	{
		// http://stackoverflow.com/questions/14524191/algorithm-to-find-the-biggest-rectangle-area-in-a-given-boolean-array

		[Test]
		public void Given_empty_should_return_empty()
		{
			var input = new int[,] { };
			GetRect(input).ShouldBeEmpty();
		}

		[Test]
		public void Given__1__should_return__0_0__0_0()
		{
			var input = new[,] { {1} };
			var points = GetRect(input);
			points.Count.ShouldBeEqualTo(2);

			points.First().ShouldBeEqualTo(points.Last());
			points.First().ShouldBeEqualTo(new Point(0, 0));
		}

		[Test]
		public void Given_any_not_1_or_0__should_return_empty()
		{
			var input = new[,] 
							{ 
								{0, 1, 2, 3, 4}, 
								{-5, -6, -7, Int32.MaxValue, Int32.MinValue} 
							};
			var points = GetRect(input);
			points.Count.ShouldBeEqualTo(0);
		}

		[Test]
		public void Given_only_zeros_should_return_empty()
		{
			var input = new[,]
				            {
					            { 0, 0 },
					            { 0, 0 }
				            };
			GetRect(input).ShouldBeEmpty();
		}

		[Test]
		public void Given_0_1__0_0__should_return___0_1__0_1()
		{
			var input = new[,]
				            {
					            { 0, 1 },
					            { 0, 0 }
				            };
			var points = GetRect(input);
			points.Count.ShouldBeEqualTo(2);
			points.First().ShouldBeEqualTo(points.Last());
			points.First().ShouldBeEqualTo(new Point(0, 1));
		}


		[Test]
		public void Given_0_1__1_0__should_return__0_1__0_1__1_0__1_0()
		{
			var input = new[,]
				            {
					            { 0, 1 },
					            { 1, 0 }
				            };
			var points = GetRect(input);
			points.Count.ShouldBeEqualTo(4);
			points[0].ShouldBeEqualTo(new Point(0, 1));
			points[1].ShouldBeEqualTo(new Point(0, 1));
			points[2].ShouldBeEqualTo(new Point(1, 0));
			points[3].ShouldBeEqualTo(new Point(1, 0));
		}

		[Test]
		public void Given_0_1__0_1__should_return__0_1__1_1()
		{
			var input = new[,]
				            {
					            { 0, 1 },
					            { 0, 1 }
				            };
			var points = GetRect(input);
			points.Count.ShouldBeEqualTo(2);
			points.First().ShouldBeEqualTo(new Point(0, 1));
			points.Last().ShouldBeEqualTo(new Point(1, 1));
		}

		[Test]
		public void Given_0_0__1_1__should_return__1_0__1_1()
		{
			var input = new[,]
				            {
					            { 0, 0 },
					            { 1, 1 }
				            };
			var points = GetRect(input);
			points.Count.ShouldBeEqualTo(2);
			points.First().ShouldBeEqualTo(new Point(1, 0));
			points.Last().ShouldBeEqualTo(new Point(1, 1));
		}

		[Test]
		public void Given_0_1__1_1__should_return__0_1__1_1__1_0__1_1()
		{
			var input = new[,]
				            {
					            { 0, 1 },
					            { 1, 1 }
				            };
			var points = GetRect(input);
			points.Count.ShouldBeEqualTo(2);
			points[0].ShouldBeEqualTo(new Point(0, 1));
			points[1].ShouldBeEqualTo(new Point(1, 1));
			points[2].ShouldBeEqualTo(new Point(1, 0));
			points[3].ShouldBeEqualTo(new Point(1, 1));
		}

		[Test]
		public void Given_1_1__1_1__should_return__0_0__1_1()
		{
			var input = new[,]
				            {
					            { 1, 1 },
					            { 1, 1 }
				            };
			var points = GetRect(input);
			points.Count.ShouldBeEqualTo(2);
			points[0].ShouldBeEqualTo(new Point(0, 0));
			points[1].ShouldBeEqualTo(new Point(1, 1));
		}


		[Test]
		public void Given_1_1_1__should_return__0_0__0_2()
		{
			var input = new[,]
				            {
					            { 1, 1, 1 }
				            };
			var points = GetRect(input);
			points.Count.ShouldBeEqualTo(2);
			points.First().ShouldBeEqualTo(new Point(0, 0));
			points.Last().ShouldBeEqualTo(new Point(0, 2));
		}

		[Test]
		public void Given_1_0_1_1__should_return__0_2__0_3()
		{
			var input = new[,]
				            {
					            { 1, 0, 1, 1 }
				            };
			var points = GetRect(input);
			points.Count.ShouldBeEqualTo(2);
			points.First().ShouldBeEqualTo(new Point(0, 2));
			points.Last().ShouldBeEqualTo(new Point(0, 3));
		}

		[Test]
		public void Given_1_1_0_1__should_return__0_0__0_1()
		{
			var input = new[,]
				            {
					            { 1, 1, 0, 1 }
				            };
			var points = GetRect(input);
			points.Count.ShouldBeEqualTo(2);
			points.First().ShouldBeEqualTo(new Point(0, 0));
			points.Last().ShouldBeEqualTo(new Point(0, 1));
		}

		[Test]
		public void Given_1_0__1_0__1_1__should_return__0_0__2_0()
		{
			var input = new[,]
				            {
					            { 1, 0 },
					            { 1, 0 },
					            { 1, 1 }
				            };
			var points = GetRect(input);
			points.Count.ShouldBeEqualTo(2);
			points.First().ShouldBeEqualTo(new Point(0, 0));
			points.Last().ShouldBeEqualTo(new Point(2, 0));
		}

		[Test]
		public void Given_1_0__1_1__1_1__should_return__1_0__2_1()
		{
			var input = new[,]
				            {
					            { 1, 0 },
					            { 1, 1 },
					            { 1, 1 }
				            };
			var points = GetRect(input);
			points.Count.ShouldBeEqualTo(2);
			points.First().ShouldBeEqualTo(new Point(1, 0));
			points.Last().ShouldBeEqualTo(new Point(2, 1));
		}

		[Test]
		public void Given_1_0_0__1_0_0__1_1_1__should_return__0_0__0_2__2_0__2_2()
		{
			var input = new[,]
				            {
					            { 1, 0, 0 },
					            { 1, 0, 0 },
					            { 1, 1, 1 }
				            };
			var points = GetRect(input);
			points.Count.ShouldBeEqualTo(2);
			points[0].ShouldBeEqualTo(new Point(0, 0));
			points[1].ShouldBeEqualTo(new Point(2, 0));
			points[2].ShouldBeEqualTo(new Point(2, 0));
			points[3].ShouldBeEqualTo(new Point(2, 2));
		}

		[Test]
		public void Given_1_0_0__1_1_0__1_1_1__should_return__1_0__2_1()
		{
			var input = new[,]
				            {
					            { 1, 0, 0 },
					            { 1, 1, 0 },
					            { 1, 1, 1 }
				            };
			var points = GetRect(input);
			points.Count.ShouldBeEqualTo(2);
			points[0].ShouldBeEqualTo(new Point(1, 0));
			points[1].ShouldBeEqualTo(new Point(2, 1));
		}

		[Test]
		public void Given_0_1_1__1_1_1__1_1_0__should_return__0_1__1_2__1_0__2_1()
		{
			var input = new[,]
				            {
					            { 0, 1, 1 },
					            { 1, 1, 1 },
					            { 1, 1, 0 }
				            };
			var points = GetRect(input);
			points.Count.ShouldBeEqualTo(2);
			points[0].ShouldBeEqualTo(new Point(0, 1));
			points[1].ShouldBeEqualTo(new Point(1, 2));
			points[2].ShouldBeEqualTo(new Point(1, 0));
			points[3].ShouldBeEqualTo(new Point(2, 1));
		}

		[Test]
		public void Given_1_1_1__1_1_1__1_1_1__should_return__0_0__2_2()
		{
			var input = new[,]
				            {
					            { 1, 1, 1 },
					            { 1, 1, 1 },
					            { 1, 1, 1 }
				            };
			var points = GetRect(input);
			points.Count.ShouldBeEqualTo(2);
			points[0].ShouldBeEqualTo(new Point(0, 0));
			points[1].ShouldBeEqualTo(new Point(2, 2));
		}

		[Test]
		public void Given_0_1_1_1__1_1_1_1__1_1_1_1__should_return__0_1__2_3()
		{
			var input = new[,]
				            {
					            { 0, 1, 1, 1 },
					            { 1, 1, 1, 1 },
					            { 1, 1, 1, 1 }
				            };
			var points = GetRect(input);
			points.Count.ShouldBeEqualTo(2);
			points[0].ShouldBeEqualTo(new Point(0, 1));
			points[1].ShouldBeEqualTo(new Point(2, 3));
		}

		[Test]
		public void Given_0_0_1_1_1__0_1_1_1_1__1_1_1_1_1__1_1_0_1_1__1_1_1_1_1__should_return__0_3__4_5()
		{
			var input = new[,]
				            {
					            { 0, 0, 1, 1, 1 },
					            { 0, 1, 1, 1, 1 },
					            { 1, 1, 1, 1, 1 },
					            { 1, 1, 0, 1, 1 },
					            { 1, 1, 1, 1, 1 }
				            };
			var points = GetRect(input);
			points.Count.ShouldBeEqualTo(2);
			points[0].ShouldBeEqualTo(new Point(0, 3));
			points[1].ShouldBeEqualTo(new Point(4, 5));
		}




		[Test,Ignore]
		public void SelfContained()
		{
			var data = new[,] { { 0, 0, 1, 1, 1 }, { 0, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 } };
			var numrows = data.GetLength(0);
			var numcols = data.GetLength(1);
			var indexes = from y in Enumerable.Range(-1, 2 + numrows)
			              from x in Enumerable.Range(-1, 2 + numcols)
			              select new
				                     {
					                     Row = y,
					                     Column = x
				                     };
			const int leftEdge = -1;
			const int bottomEdge = -1;
			var rightEdge = numcols;
			var topEdge = numrows;
			Func<int, int, bool> isEdge = (row, column) => column == leftEdge || row == bottomEdge || column == rightEdge || row == topEdge;
			Func<int, int, bool> hasAdjacentColumn = (row, column) => !isEdge(row, column) && (column > 0 && data[row, column - 1] == 1 || column < numcols && data[row, column + 1] == 1);
			Func<int, int, bool> hasAdjacentRow = (row, column) => !isEdge(row, column) && (row > 0 && data[row - 1, column] == 1 || row < numrows && data[row + 1, column] == 1);
			var potentialCorners = indexes
				.Where(x => isEdge(x.Row, x.Column) || data[x.Row, x.Column] == 0)
				.Select(x => new
					             {
						             IsEdge = isEdge(x.Row, x.Column),
						             HasAdjacentColumn = hasAdjacentColumn(x.Row, x.Column),
						             HasAdjacentRow = hasAdjacentRow(x.Row, x.Column),
						             x.Row,
						             x.Column
					             })
				.Where(x => x.IsEdge || x.HasAdjacentColumn || x.HasAdjacentRow)
				.ToList();

			var xPairs = potentialCorners
				.Where(x => x.HasAdjacentColumn || x.IsEdge)
				.GroupBy(x => x.Column)
				.Where(x => x.Key > leftEdge && x.Key < rightEdge)
				.SelectMany(g =>
					{
						var sorted = g.OrderBy(x => x.Row).ToArray();
						return sorted.Where(x => x.Row < topEdge)
						             .Select(x =>
							             {
								             var first = sorted.First(y => y.Row > x.Row);
								             return new
									                    {
										                    Column = g.Key,
										                    BottomEdge = x.Row,
										                    TopEdge = first.Row,
										                    HasAdjacentRow = data[x.Row + 1, g.Key] == 1
									                    };
							             })
						             .Where(x => x.HasAdjacentRow)
						             .Where(x => x.TopEdge - x.BottomEdge > 0);
					})
				.ToList();

			var yPairs = potentialCorners
				.Where(x => x.HasAdjacentRow || x.IsEdge)
				.GroupBy(x => x.Row)
				.Where(x => x.Key > bottomEdge && x.Key < topEdge)
				.SelectMany(g =>
					{
						var sorted = g.OrderBy(x => x.Column).ToArray();
						return sorted.Where(x => x.Column < rightEdge)
						             .Select(x =>
							             {
								             var first = sorted.First(y => y.Column > x.Column);
								             return new
									                    {
										                    Row = g.Key,
										                    LeftEdge = x.Column,
										                    RightEdge = first.Column,
										                    HasAdjacentColumn = data[g.Key, x.Column + 1] == 1
									                    };
							             })
						             .Where(x => x.HasAdjacentColumn)
						             .Where(x => x.RightEdge - x.LeftEdge > 0);
					})
				.ToList();

			var rectangles = xPairs
				.Select(x =>
					{
						var yPairsInBounds = yPairs.Where(y => y.Row > x.BottomEdge && y.Row < x.TopEdge).ToList();
						var before = yPairsInBounds.OrderByDescending(y => y.LeftEdge).First(y => y.LeftEdge < x.Column);
						var after = yPairsInBounds.OrderBy(y => y.RightEdge).First(y => y.RightEdge > x.Column);
						return new
							       {
								       x.TopEdge,
								       x.BottomEdge,
								       before.LeftEdge,
								       after.RightEdge,
							       };
					})
				.Select(x => new
					             {
						             TopLeft = new Point(x.LeftEdge, x.TopEdge),
						             BottomRight = new Point(x.RightEdge, x.BottomEdge)
					             })
				.ToList();

			foreach (var potentialCorner in potentialCorners.Where(x => !x.IsEdge).OrderBy(x => x.Row).ThenBy(x => x.Column))
			{
				Console.WriteLine(potentialCorner.Row + "," + potentialCorner.Column);
			}

			Func<Point, Point, int> getArea = (tl, br) => (tl.X + 1 - (br.X + 1)) * (br.Y - 1 - (tl.Y + 1));
			foreach (var rectangle in rectangles.OrderByDescending(x => getArea(x.TopLeft, x.BottomRight)))
			{
				Console.WriteLine("(" + rectangle.TopLeft + ") to (" + rectangle.BottomRight + ") -- area " + getArea(rectangle.TopLeft, rectangle.BottomRight));
			}
		}

		public IList<Point> GetRect(int[,] input)
		{
//			var leftEdgeOnes = new List<Point>();
//			var rightEdgeOnes = new List<Point>();
//			var topEdgeOnes = new List<Point>();
//			var bottomEdgeOnes = new List<Point>();
//			var maxI = input.GetLength(0);
//			var maxJ = input.GetLength(1);
//			for (int i = 0; i < maxI; i++)
//			{
//				for (int j = 0; j < maxJ; j++)
//				{
//					if (input[i, j] == 1)
//					{
//						if (i == 0 || input[i - 1, j] == 0)
//						{
//							leftEdgeOnes.Add(new Point(i, j));
//						}
//						if (i == maxI - 1 || input[i + 1, j] == 0)
//						{
//							rightEdgeOnes.Add(new Point(i,j));
//						}
//						if (j == 0 || input[i,j - 1] == 0)
//						{
//							topEdgeOnes.Add(new Point(i,j));
//						}
//						if (j == maxJ -1 || input[i,j + 1] == 0)
//						{
//							bottomEdgeOnes.Add(new Point(i,j));
//						}
//					}
//				}
//			}
//
//			var allOnes = topEdgeOnes.Concat(bottomEdgeOnes).Concat(leftEdgeOnes).Concat(rightEdgeOnes).Distinct().ToList();



			var across = new List<List<Point>>{new List<Point>()};
			for (int i = 0; i < input.GetLength(0); i++)
			{
				for (int j = 0; j < input.GetLength(1); j++)
				{
					if (input[i, j] == 1)
					{
						var list = across.Last();
						list.Add(new Point(i, j));
					}
					else
					{
						across.Add(new List<Point>());
					}
				}
				across.Add(new List<Point>());
			}

			var down = new List<List<Point>> { new List<Point>() };
			for (int j = 0; j < input.GetLength(1); j++)
			{
				for (int i = 0; i < input.GetLength(0); i++)				
				{
					if (input[i, j] == 1)
					{
						var list = down.Last();
						list.Add(new Point(i, j));
					}
					else
					{
						down.Add(new List<Point>());
					}
				}
				down.Add(new List<Point>());
			}

			var longest = down.Concat(across).OrderByDescending(x => x.Count).First();
			if (longest.Count == 1)
			{

				longest.Add(longest[0]);
			}
			if (longest.Count > 2)
			{
				longest = new List<Point> { longest.First(), longest.Last() };
			}
			return longest;
		}
	}
}