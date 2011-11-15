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
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

using NUnit.Framework;

namespace Scratch.FillGrid
{
	public class LandType
	{
		private static readonly List<LandType> _list = new List<LandType>();

		public static readonly LandType AlluvialPlain = new LandType(Color.LightGray);
		public static readonly LandType CoastalPlain = new LandType(Color.PaleGoldenrod);
		public static readonly LandType CultivatedLand = new LandType(Color.IndianRed);
		public static readonly LandType FloodPlain = new LandType(Color.Lavender);
		public static readonly LandType Forest = new LandType(Color.ForestGreen);
		public static readonly LandType ForestSwamp = new LandType(Color.BurlyWood);
		public static readonly LandType GrassLand = new LandType(Color.DarkSeaGreen);
		public static readonly LandType NeedleForest = new LandType(Color.DarkGreen);
		public static readonly LandType Plateau = new LandType(Color.RosyBrown);
		public static readonly LandType Sand = new LandType(Color.SandyBrown);
		public static readonly LandType Savannah = new LandType(Color.SpringGreen);
		public static readonly LandType SemiArid = new LandType(Color.PapayaWhip);
		public static readonly LandType Swamp = new LandType(Color.CadetBlue);
		public static readonly LandType TidalMarsh = new LandType(Color.CornflowerBlue);
		// .. etc, for more land form names see: 
		// http://makingmaps.net/2008/04/03/map-symbols-landforms-terrain/

		private LandType(Color color)
		{
			_list.Add(this);
			Color = color;
		}

		public Color Color { get; set; }

		public static IEnumerable<LandType> GetAll()
		{
			return _list;
		}
	}

	public static class IEnumerableExtensions
	{
		private static readonly Random _random = new Random();

		public static T PickRandom<T>(this IEnumerable<T> items, int expectedMax)
		{
			if (expectedMax <= 0)
			{
				throw new ArgumentOutOfRangeException("expectedMax", "Should be a positive integer at least as large as the collection.");
			}
			var list = new List<T>();
//			expectedMax = expectedMax > Int32.MaxValue - 100 ? Int32.MaxValue : expectedMax;
			int max = expectedMax;
			foreach (var item in items)
			{
				if (_random.Next(max) == 0)
				{
					return item;
				}
				list.Add(item);
//				if (max == 10)
//				{
//					max = expectedMax;
//				}
			}
			if (list.Count > 0)
			{
				return list[_random.Next(list.Count)];
			}
			return default(T);
		}
	}

	[TestFixture]
	public class Demo
	{
		public static int Height = 1000;
		public static int Width = 1000;
		public static int XOffsetEast = 1;
		public static int XOffsetWest = -1;

		public static int YOffsetNorth = -1;
		public static int YOffsetSouth = 1;

		[Test]
		public void EnumerableRandomNumberSelection()
		{
			var source = Enumerable.Range(0, 8).ToList();
			var randomSequence = Enumerable.Range(0, 100000).Select(x=> source.PickRandom(8)).ToArray();
//			var randomSequence = Enumerable.Range(0, 200).SelectMany(x=>
//				  Enumerable.Range(1, 500).OrderBy(n => n * n * (new Random()).Next()))
//				  .ToArray();
			RandomSequenceGeneratorTest(randomSequence);

		}

		[Test]
		public void FillGrid()
		{
			var bitmap = new Bitmap(Width, Height);
			var defaultPixel = bitmap.GetPixel(0, 0);
			var hasOpenNeighbors = new HashSet<Land>();
			var random = new Random();
			var landTypes = LandType.GetAll().ToList();
			for (int i = 0; i < 300; i++)
			{
				int randX = random.Next(Width);
				int randY = random.Next(Height);
				var point = new Point(randX, randY);
				if (bitmap.GetPixel(randX, randY) == defaultPixel)
				{
					var landType = landTypes[random.Next(landTypes.Count)];
					var item = new Land
						{
							Location = point,
							LandType = landType
						};
					bitmap.SetPixel(randX, randY, landType.Color);
					hasOpenNeighbors.Add(item);
				}
			}

			while (hasOpenNeighbors.Any())
			{
				var toRemove = new List<Land>();
				var toAdd = new List<Land>();
				foreach (var item in hasOpenNeighbors)
				{
					var neighborLocation = GetNeighbors(item.Location)
						.Where(x => bitmap.GetPixel(x.X, x.Y) == defaultPixel)
						.PickRandom(7);
					if (neighborLocation == default(Point))
					{
						toRemove.Add(item);
						continue;
					}
					bitmap.SetPixel(neighborLocation.X, neighborLocation.Y, item.LandType.Color);
					toAdd.Add(new Land
						{
							Location = neighborLocation,
							LandType = item.LandType
						});
				}
				foreach (var land in toRemove)
				{
					hasOpenNeighbors.Remove(land);
				}
				foreach (var land in toAdd)
				{
					hasOpenNeighbors.Add(land);
				}
			}

			bitmap.Save("result.jpg");
		}

		public static Point CreatePoint(Point point, int xOffset, int yOffset)
		{
			return new Point(point.X + xOffset, point.Y + yOffset);
		}

		public static IEnumerable<Point> GetNeighbors(Point location)
		{
			return new Func<Point, Point>[]
				{
					GoNorth, GoNorthEast, GoEast, GoSouthEast,
					GoSouth, GoSouthWest, GoWest, GoNorthWest
				}
				.Select(direction => direction(location))
				.Where(IsOnTheBoard);
		}

		public static Point GoEast(Point point)
		{
			return CreatePoint(point, XOffsetEast, 0);
		}

		public static Point GoNorth(Point point)
		{
			return CreatePoint(point, 0, YOffsetNorth);
		}

		public static Point GoNorthEast(Point point)
		{
			return CreatePoint(point, XOffsetEast, YOffsetNorth);
		}

		public static Point GoNorthWest(Point point)
		{
			return CreatePoint(point, XOffsetWest, YOffsetNorth);
		}

		public static Point GoSouth(Point point)
		{
			return CreatePoint(point, 0, YOffsetSouth);
		}

		public static Point GoSouthEast(Point point)
		{
			return CreatePoint(point, XOffsetEast, YOffsetSouth);
		}

		public static Point GoSouthWest(Point point)
		{
			return CreatePoint(point, XOffsetWest, YOffsetSouth);
		}

		public static Point GoWest(Point point)
		{
			return CreatePoint(point, XOffsetWest, 0);
		}

		public static bool IsOnTheBoard(Point point)
		{
			return point.X >= 0 && point.X < Width && point.Y >= 0 && point.Y < Height;
		}

		public class Land
		{
			public LandType LandType { get; set; }
			public Point Location { get; set; }
		}

		private static void RandomSequenceGeneratorTest(IEnumerable<int> sequence)
		{
			var array = sequence.ToArray();
			int maxX = array.Max() + 1;
			var grouped = array.GroupBy(x => x).ToArray();
			int minY = grouped.Min(x => x.Count());
			int maxY = grouped.Max(x => x.Count()) + 1;

			/* create image */
			using (var bitmap = new Bitmap(maxX, maxY - minY))
			{
				using (var graphics = Graphics.FromImage(bitmap))
				{
					graphics.FillRectangle(Brushes.White, 0, 0, maxX, maxY);

					foreach(var set in grouped)
					{
						var y = maxY - set.Count();
						var height = set.Count() - minY;
						graphics.FillRectangle(Brushes.Black, set.Key, y, 1, height);
					}

//					int prev = array.First();
//					foreach (int current in array.Skip(1))
//					{
//						bitmap.SetPixel(prev, current, Color.Black);
//						prev = current;
//					}
				}
				bitmap.Save("test.png", ImageFormat.Png);
			}
		}
	}
}