using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

namespace Scratch.Parse
{
	[TestFixture]
	public class TabDelimitedSerialized
	{
		// http://stackoverflow.com/questions/15179379/parse-tab-delimited-file-with-more-than-one-table

		[Test]
		public void Test()
		{
			const string data = @"%T\tperson
%F\tid\tname\taddress\tcity
%R\t1\tBob\t999 Main St\tBurbank
%R\t2\tSara\t829 South st\tPasadena
%T\thouses
%F\tid\tpersonid\thousetype\tColor
%R\t25\t1\tHouse\tRed
%R\t26\t2\tcondo\tGreen";

			var reader = new StringReader(data.Replace("\\t","\t"));

			var rows = Parse(reader);
			foreach (var row in rows)
			{
				foreach (var entry in row)
				{
					Console.Write(entry.Key);
					Console.Write('\t');
					Console.Write('=');
					Console.Write('\t');
					Console.Write(entry.Value);
					Console.WriteLine();
				}
				Console.WriteLine();
			}
		}

		public IEnumerable<Dictionary<string, string>> Parse(TextReader reader)
		{
			var state = new State { Handle = ExpectTableTitle };
			return GenerateFrom(reader)
				.Select(line => state.Handle(line.Split('\t'), state))
				.Where(returnIt => returnIt)
				.Select(returnIt => state.Row);
		}

		private bool ExpectTableTitle(string[] lineParts, State state)
		{
			if (lineParts[0] == "%T")
			{
				state.TableTitle = lineParts[1];
				state.Handle = ExpectFieldNames;
			}
			else
			{
				Console.WriteLine("Expected %T but found '"+lineParts[0]+"'");
			}
			return false;
		}

		private bool ExpectFieldNames(string[] lineParts, State state)
		{
			if (lineParts[0] == "%F")
			{
				state.FieldNames = lineParts.Skip(1).ToArray();
				state.Handle = ExpectRowOrTableTitle;
			}
			else
			{
				Console.WriteLine("Expected %F but found '" + lineParts[0] + "'");
			}
			return false;
		}

		private bool ExpectRowOrTableTitle(string[] lineParts, State state)
		{
			if (lineParts[0] == "%R")
			{

				state.Row = lineParts.Skip(1)
					.Select((x, i) => new { Value = x, Index = i })
					.ToDictionary(x => state.FieldNames[x.Index], x => x.Value);
				state.Row.Add("_tableTitle",state.TableTitle);
				return true;
			}
			return ExpectTableTitle(lineParts, state);
		}

		public class State
		{
			public string TableTitle;
			public string[] FieldNames;
			public Dictionary<string, string> Row;
			public Func<string[], State, bool> Handle;
		}

		private static IEnumerable<string> GenerateFrom(TextReader reader)
		{
			string line;
			while ((line = reader.ReadLine()) != null)
			{
				yield return line;
			}
		}
	}

}