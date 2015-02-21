using System;
using System.Linq;

using NUnit.Framework;

namespace Scratch.Sorting.QuickSortWithInsertionSort
{
	[TestFixture]
	public class Class1
	{
		[Test]
		public void Test()
		{
			var random = new Random();
			while(true)
			{
				for (var count = 2; count < 14; count++)
				{
					Console.WriteLine("count " + count);
					for (var round = 0; round < 100; round++)
					{
						var items = Enumerable.Range(0, count).Select(x => random.Next(0, 100)).ToArray();
						var copy = items.ToArray();
						Console.WriteLine("orig: " + String.Join(", ", items.Select(x => x.ToString())));
						AlgorithmC(copy, 0, copy.Length - 1);

						var sorted = items.OrderBy(x => x).ToArray();
						for (var i = 0; i < copy.Length; i++)
						{
							if (copy[i] != sorted[i])
							{
								Console.WriteLine("orig: " + String.Join(", ", items.Select(x => x.ToString())));
								Console.WriteLine("algC: " + String.Join(", ", copy.Select(x => x.ToString())));
								Console.WriteLine("sort: " + String.Join(", ", sorted.Select(x => x.ToString())));
								Console.WriteLine("differs at index " + i);
								Assert.Fail();
							}
						}
					}
				}
			}
		}	
		
		[Test]
		public void TestInsertionSort()
		{
			var random = new Random();
			while(true)
			{
				for (var count = 5; count < 17; count++)
				{
					bool breakRound = false;
					for (var round = 0; round < 10 && !breakRound; round++)
					{
						breakRound = false;
						var items = Enumerable.Range(0, count).Select(x => random.Next(0, 10)).ToArray();
						var copy = items.ToArray();
						InsertionSortA(copy, 0, copy.Length - 1);
						var sorted = items.OrderBy(x => x).ToArray();
						for (var i = 0; i < copy.Length; i++)
						{
							if (copy[i] != sorted[i])
							{
								Console.WriteLine("orig: " + String.Join(", ", items.Select(x => x.ToString())));
								Console.WriteLine("inss: " + String.Join(", ", copy.Select(x => x.ToString())));
								Console.WriteLine("sort: " + String.Join(", ", sorted.Select(x => x.ToString())));
								Console.WriteLine("differs at index " + i);
								breakRound = true;
								Assert.Fail();
							}
						}
					}
				
				}
			}
		}

		[Test]
		public void TestSpecific()
		{
			var items = new[] { 18, 82, 94, 30, 97, 59 };
			var copy = items.ToArray();
			AlgorithmC(copy, 0, items.Length -1);
			var sorted = items.OrderBy(x => x).ToArray();
			for (var i = 0; i < copy.Length; i++)
			{
				if (copy[i] != sorted[i])
				{
					Console.WriteLine("orig: " + String.Join(", ", items.Select(x => x.ToString())));
					Console.WriteLine("algC: " + String.Join(", ", copy.Select(x => x.ToString())));
					Console.WriteLine("sort: " + String.Join(", ", sorted.Select(x => x.ToString())));
					Console.WriteLine("differs at index " + i);
					Assert.Fail();
				}
			}
		}		
		
		[Test]
		public void TestInsertionSortSpecific()
		{
			var items = new[] { 9, 2, 6 };
			var copy = items.ToArray();
			InsertionSortA(copy, 0, 2);
			var sorted = items.OrderBy(x => x).ToArray();
			for (var i = 0; i < copy.Length; i++)
			{
				if (copy[i] != sorted[i])
				{
					Console.WriteLine("orig: " + String.Join(", ", items.Select(x => x.ToString())));
					Console.WriteLine("inss: " + String.Join(", ", copy.Select(x => x.ToString())));
					Console.WriteLine("sort: " + String.Join(", ", sorted.Select(x => x.ToString())));
					Console.WriteLine("differs at index " + i);
					Assert.Fail();
				}
			}
		}

		public static void AlgorithmC(int[] array, int start, int end)
		{
//			Console.WriteLine("starting with " + start + " " + end + ": " + String.Join(", ", array.Select(x => x.ToString())));

			if (start < end)
			{
				if ((end - start) < 16) // changed
				{
					InsertionSortA(array, start, end);
					return;
				}
				else
				{
//					Console.WriteLine("pivot "+array[start]);
					int left = start;
					int right = end;
					int pivot = array[start];
					while (left < right)
					{
						if (array[left] < pivot)
						{
							left++;
							continue;
						}
						if (array[right] > pivot)
						{
							right--;
							continue;
						}
						int tmp = array[left];
						array[left] = array[right];
						array[right] = tmp;
//						Console.WriteLine("swapped " + left + " and " + right + ": " + String.Join(", ", array.Select(x => x.ToString())));
						left++;
					}

					AlgorithmC(array, start, left - 1);
					AlgorithmC(array, left, end);
				}
			}
		}

		public static void InsertionSortA(int[] array, int start, int end)
		{
//			Console.WriteLine("insert before " + start + " and " + end + ": " + String.Join(", ", array.Select(x => x.ToString())));
			for (int x = start + 1; x <= end; x++)
			{
				int val = array[x];
				int j = x - 1;
				while (j >= 0 && val < array[j])
				{
					array[j + 1] = array[j];
					j--;
				}
				array[j + 1] = val;
			}
//			Console.WriteLine("insert after " + start + " and " + end + ": " + String.Join(", ", array.Select(x => x.ToString())));
		}
	}


}