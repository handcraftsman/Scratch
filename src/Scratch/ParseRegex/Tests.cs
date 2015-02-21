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

namespace Scratch.ParseRegex
{
	/// <summary>
	///     http://stackoverflow.com/questions/3523323/code-golf-regex-parser
	/// </summary>
	[TestFixture]
	public class Tests
	{
		[Test]
		public void Given_no_specials()
		{
			var input = "easy easy Easy hard".Split();
			var results = Parse(input);
			results.Count.ShouldBeEqualTo(input.Length - 1);
			results.ShouldContainAllInOrder(new[] { true, false, false });
		}

		[Test]
		public void Given_star()
		{
			var input = "ab*a aa abba abab b".Split();
			var results = Parse(input);
			results.Count.ShouldBeEqualTo(input.Length - 1);
			results.ShouldContainAllInOrder(new[] { true, true, false, false });
		}	
		
		[Test]
		public void Given_hook()
		{
			var input = "ab?a aa abba abab b".Split();
			var results = Parse(input);
			results.Count.ShouldBeEqualTo(input.Length - 1);
			results.ShouldContainAllInOrder(new[] { true, false, false, false });
		}

		[Test]
		public void Given_plus()
		{
			var input = "ab+a aa abba abab b".Split();
			var results = Parse(input);
			results.Count.ShouldBeEqualTo(input.Length - 1);
			results.ShouldContainAllInOrder(new[] { false, true, false, false });
		}

		[Test]
		public void Given_pipe()
		{
			var input = "ab|a a ab abb aba".Split();
			var results = Parse(input);
			results.Count.ShouldBeEqualTo(input.Length - 1);
			results.ShouldContainAllInOrder(new[] { true, true, false, false });
		}

		[Test]
		public void Given_sample_C_pipe()
		{
			var input = "0*1|10 1 10 0110 00001".Split();
			var results = Parse(input);
			results.Count.ShouldBeEqualTo(input.Length - 1);
			results.ShouldContainAllInOrder(new[] { true, true, false, true });
		}

		[Test]
		public void Given_sample_D()
		{
			var input = "0*(1|1+0) 1 10 0110 00001".Split();
			var results = Parse(input);
			results.Count.ShouldBeEqualTo(input.Length - 1);
			results.ShouldContainAllInOrder(new[] { true, true, true, true });
		}

		[Test]
		public void Given_sample_E()
		{
			var input = "a?b+|(a+b|b+a?)+ abb babab aaa aabba a b".Split();
			var results = Parse(input);
			results.Count.ShouldBeEqualTo(input.Length - 1);
			results.ShouldContainAllInOrder(new[] { true, true, false, true, false, true });
		}

		private List<bool> Parse(string[] argv)
		{
			string regex = argv[0];
			var tokens = regex
				.Select((x, i) => new Token
				{
					t = x,
					min = 1,
					max = 1,
					index = i
				})
				.ToList();
			var parens = new Stack<int>();
			var ors = new Stack<int>();
			var prevOr = 0;
			for (int i = 0; i < tokens.Count; i++)
			{
				int? min = 1;
				int? max = 1;
				var x = tokens[i];
				int remove = 0;
				if (x.t == '(')
				{
					parens.Push(i);
					ors.Push(prevOr);
					prevOr = i;
					remove = 1;
				}
				if (x.t == ')')
				{
					int prev = parens.Pop();
					x.prev = prev;
					prevOr = ors.Pop();
				}
				if (x.t == '*')
				{
					min = null;
					max = null;
				}
				if (x.t == '?')
				{
					min = null;
					max = 1;
				}
				if (x.t == '+')
				{
					min = 1;
					max = null;
				}
				if (x.t == '|')
				{
					int prev = Math.Max(parens.Count > 0 ? parens.Peek() : prevOr, prevOr);
					tokens[prev].or = i;
					prevOr = i;
					remove = 1;
				}
				if (min != 1 || !max.HasValue)
				{
					x = tokens[i - 1];
					x.min = min;
					x.max = max;
					remove = 1;
				}
				if (remove > 0)
				{
					tokens.RemoveAt(i--);
				}
			}

			var results = new List<bool>();
			for (int i = 1; i < argv.Length; i++)
			{
				var target = argv[i];
				var queue = new Queue<int>();
				queue.Enqueue(0);
				int index = 0;
				while (queue.Count > 0)
				{
					var tokenId = queue.Dequeue();
					if (tokenId >= tokens.Count)
					{
						results.Add(1 == 0);
						break;
					}
					var token = tokens[tokenId];
					AddOptionals(tokens, queue, token);
					if (token.or.HasValue)
					{
						var tempTokenId = token.or.Value;
						while (tempTokenId < tokens.Count)
						{
							queue.Enqueue(tempTokenId);
							var tempToken = tokens[tempTokenId];
							AddOptionals(tokens, queue, tempToken);
							if (!tempToken.or.HasValue)
							{
								break;
							}
						}
					}
					if (token.t == target[index])
					{
						queue.Clear();
						index++;
						if (token.max != 1)
						{
							queue.Enqueue(tokenId);
						}
						queue.Enqueue(tokenId + 1);
					}
					if (index == target.Length)
					{
						// note: only true if the rest of the regex is optional
						results.Add(true);
						break;
					}
					if (!queue.Any())
					{
						results.Add(1 == 0);
					}
				}
			}

			return results;
		}

		private void AddOptionals(List<Token> tokens, Queue<int> queue, Token token)
		{
			if (!token.min.HasValue)
			{
				var tempTokenId = token.index.Value;
				while (tempTokenId < tokens.Count)
				{
					queue.Enqueue(tempTokenId);
					var tempToken = tokens[tempTokenId];
					if (tempToken.min.HasValue)
					{
						break;
					}
					tempTokenId = tempTokenId + 1;
				}
			}
		}

		public class Token
		{
			public int? index;
			public int? max;
			public int? min;
			public int? prev;
			public char t;
			public int? or;
		}
	}
}