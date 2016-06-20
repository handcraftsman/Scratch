using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Scratch.SequentialLinq
{
	// http://stackoverflow.com/questions/30937901/merge-two-or-more-t-in-listt-based-on-condition
	public static class IEnumerableExtensions
	{
		public static IEnumerable<IList<T>> MakeSets<T>(this IEnumerable<T> items, Func<T, T, bool> areInSameGroup)
		{
			var result = new List<T>();
			foreach (var item in items)
			{
				if (!result.Any() || areInSameGroup(result[result.Count - 1], item))
				{
					result.Add(item);
					continue;
				}
				yield return result;
				result = new List<T> {item};
			}
			if (result.Any())
			{
				yield return result;
			}
		}
	}

	[TestFixture]
	public class SplitByBreakInSequence
	{
		public class FactoryOrder
		{
			public FactoryOrder(string text, int orderNo)
			{
				Text = text;
				OrderNo = orderNo;
			}

			public string Text { get; set; }
			public int OrderNo { get; set; }
		}

		private static List<FactoryOrder> GetItems()
		{
			var items = new List<FactoryOrder>(new[]
			{
				new FactoryOrder("Apple", 20),
				new FactoryOrder("Orange", 21),
				new FactoryOrder("WaterMelon", 42),
				new FactoryOrder("JackFruit", 51),
				new FactoryOrder("Grapes", 71),
				new FactoryOrder("mango", 72),
				new FactoryOrder("Cherry", 73)
			});
			return items.OrderBy(x => x.Text).ToList(); // not ordered by OrderNo
		}

		[Test]
		public void Test()
		{
			var groupId = 0;
			var previous = Int32.MinValue;
			var grouped = GetItems()
				.OrderBy(x => x.OrderNo)
				.Select(x =>
				{
					var group = x.OrderNo != previous + 1 ? (groupId = x.OrderNo) : groupId;
					previous = x.OrderNo;
					return new
					{
						GroupId = group,
						Item = x
					};
				})
				.GroupBy(x => x.GroupId)
				.Select(x => new FactoryOrder(String.Join(" ", x.Select(y => y.Item.Text).ToArray()), x.Key))
				.ToArray();

			foreach (var item in grouped)
			{
				Console.WriteLine(item.Text + "\t" + item.OrderNo);
			}
		}

		[Test]
		public void Test2()
		{
			var grouped = GetItems()
				.OrderBy(x => x.OrderNo)
				.MakeSets((prev, next) => next.OrderNo == prev.OrderNo + 1)
				.Select(x => new FactoryOrder(
					String.Join(" ", x.Select(y => y.Text).ToArray()),
					x.First().OrderNo))
				.ToList();

			foreach (var item in grouped)
			{
				Console.WriteLine(item.Text + "\t" + item.OrderNo);
			}
		}
	}
}