using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace Scratch.StringDifferences
{
	[TestFixture]
	public class Demo
	{
		[Test]
		public void Try_123_to_324()
		{
			const string current = "123";
			const string target = "324";
			var commands = GetChangeCommands(current, target);
			Execute(current, target, commands);
		}

		[Test]
		public void Try_hello_to_world()
		{
			const string current = "hello";
			const string target = "world";
			var commands = GetChangeCommands(current, target);
			Execute(current, target, commands);
		}

		[Test]
		public void Try_something_to_sightseeing()
		{
			const string current = "something";
			const string target = "sightseeing";
			var commands = GetChangeCommands(current, target);
			Execute(current, target, commands);
		}

		[Test]
		public void Try_something_to_smith()
		{
			const string current = "something";
			const string target = "smith";
			var commands = GetChangeCommands(current, target);
			Execute(current, target, commands);
		}

		private static void Execute(string current, string target, IEnumerable<ICommand> commands)
		{
			Console.WriteLine("converting".PadRight(19) + current + " to " + target);
			foreach (var command in commands)
			{
				Console.Write(command.ToString().PadRight(15));
				Console.Write(" => ");
				current = command.Change(current);
				Console.WriteLine(current);
			}
		}

		private static IEnumerable<ICommand> GetChangeCommands(string source, string target)
		{
			var unmatchedSourceTokens = GetUnmatchedTokenIndexes(source, target);
			var unmatchedTargetTokens = GetUnmatchedTokenIndexes(target, source);

			var commands = new List<ICommand>();

			foreach (var tokenIndexList in unmatchedSourceTokens)
			{
				var sourceToken = tokenIndexList.Key;
				var sourceStringSourceTokenIndexes = unmatchedSourceTokens[sourceToken];

				foreach (var sourceLoopIndex in tokenIndexList.Value.ToList())
				{
					var sourceIndex = sourceLoopIndex;
					bool swapped;
					do
					{
						swapped = false;
						if (sourceIndex >= target.Length)
						{
							continue;
						}
						var targetToken = target[sourceIndex];
						if (targetToken == sourceToken)
						{
							sourceStringSourceTokenIndexes.Remove(sourceIndex);
							unmatchedTargetTokens[targetToken].Remove(sourceIndex);
							continue;
						}
						List<int> sourceStringTargetTokenIndexes;
						if (!unmatchedSourceTokens.TryGetValue(targetToken, out sourceStringTargetTokenIndexes) ||
						    !sourceStringTargetTokenIndexes.Any())
						{
							continue;
						}
						var targetIndex = sourceStringTargetTokenIndexes.First();
						commands.Add(new SwapCommand(sourceIndex, targetIndex));
						sourceStringTargetTokenIndexes.RemoveAt(0);
						sourceStringSourceTokenIndexes.Remove(sourceIndex);
						sourceStringSourceTokenIndexes.Add(targetIndex);
						unmatchedTargetTokens[targetToken].Remove(sourceIndex);
						swapped = true;
						sourceIndex = targetIndex;
					} while (swapped);
				}
			}

			var removalCommands = unmatchedSourceTokens
				.SelectMany(x => x.Value)
				.Select(x => new RemoveCommand(x))
				.Cast<ICommand>()
				.OrderByDescending(x => x.Index)
				.ToList();

			commands.AddRange(removalCommands);

			var insertCommands = unmatchedTargetTokens
				.SelectMany(x => x.Value.Select(y => new InsertCommand(y, x.Key)))
				.Cast<ICommand>()
				.OrderBy(x => x.Index)
				.ToList();

			commands.AddRange(insertCommands);

			return commands;
		}

		private static IDictionary<char, List<int>> GetUnmatchedTokenIndexes(string source, string target)
		{
			var targetTokenIndexes = target.Select((x, i) => new
				                                                 {
					                                                 Token = x,
					                                                 Index = i
				                                                 })
			                               .ToLookup(x => x.Token, x => x.Index)
			                               .ToDictionary(x => x.Key, x => x.ToList());

			var distinctSourceTokenIndexes = new Dictionary<char, List<int>>();
			foreach (var tokenInfo in source.Select((x, i) => new
				                                                  {
					                                                  Token = x,
					                                                  Index = i
				                                                  }))
			{
				List<int> indexes;
				if (!targetTokenIndexes.TryGetValue(tokenInfo.Token, out indexes) ||
				    !indexes.Contains(tokenInfo.Index))
				{
					if (!distinctSourceTokenIndexes.TryGetValue(tokenInfo.Token, out indexes))
					{
						indexes = new List<int>();
						distinctSourceTokenIndexes.Add(tokenInfo.Token, indexes);
					}
					indexes.Add(tokenInfo.Index);
				}
			}
			return distinctSourceTokenIndexes;
		}
	}

	internal class InsertCommand : ICommand
	{
		private readonly char _token;

		public InsertCommand(int index, char token)
		{
			Index = index;
			_token = token;
		}

		public int Index { get; private set; }

		public string Change(string input)
		{
			var chars = input.ToList();
			chars.Insert(Index, _token);
			return new string(chars.ToArray());
		}

		public override string ToString()
		{
			return string.Format("[\"add\", {0}, '{1}']", Index, _token);
		}
	}

	internal class RemoveCommand : ICommand
	{
		public RemoveCommand(int index)
		{
			Index = index;
		}

		public int Index { get; private set; }

		public string Change(string input)
		{
			var chars = input.ToList();
			chars.RemoveAt(Index);
			return new string(chars.ToArray());
		}

		public override string ToString()
		{
			return string.Format("[\"remove\", {0}]", Index);
		}
	}

	internal class SwapCommand : ICommand
	{
		private readonly int _targetIndex;

		public SwapCommand(int sourceIndex, int targetIndex)
		{
			Index = sourceIndex;
			_targetIndex = targetIndex;
		}

		public int Index { get; private set; }

		public string Change(string input)
		{
			var chars = input.ToArray();
			var temp = chars[Index];
			chars[Index] = chars[_targetIndex];
			chars[_targetIndex] = temp;
			return new string(chars);
		}

		public override string ToString()
		{
			return string.Format("[\"swap\", {0}, {1}]", Index, _targetIndex);
		}
	}

	internal interface ICommand
	{
		int Index { get; }
		string Change(string input);
	}
}