using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssert;
using NUnit.Framework;

namespace Scratch.CreditCard
{
	[TestFixture]
	public class Tests
	{
		private static void Verify(string input, string expected)
		{
			var result = Sanitize(input);
			result.ShouldBeEqualTo(expected);
		}

		private static string Sanitize(string input)
		{
			const int maxCardLength = 16;
			var detectors = Enumerable.Range(0, maxCardLength).Select(x => new CreditCardNumberDetector(x)).ToArray();
			var characters = input.ToCharArray();

			for (int i = 0; i < input.Length; i++)
			{
				char character = input[i];
				for (int j = 0; j < maxCardLength; j++)
				{
					var cardDetector = detectors[j];
					if (cardDetector.IsCreditCard())
					{
						var digitIndexes = cardDetector.GetDigitIndexes().ToList();
						foreach (int digitIndex in digitIndexes)
						{
							characters[digitIndex] = 'X';
						}
					}
					if (cardDetector.NumberOfDigits >= 16)
					{
						cardDetector.Reset();
					}

					cardDetector.Add(character, i);
				}
			}

			foreach (var cardDetector in detectors)
			{
				if (cardDetector.IsCreditCard())
				{
					var digitIndexes = cardDetector.GetDigitIndexes().ToList();
					foreach (int digitIndex in digitIndexes)
					{
						characters[digitIndex] = 'X';
					}
				}
			}

			string result = new String(characters);
			return result;
		}

		[Test]
		public void Given_valid_16_in_longer_41111111111111111_should_produce_XXXXXXXXXXXXXXXX1()
		{
			Verify("41111111111111111", "XXXXXXXXXXXXXXXX1");
		}
	
		[Test]
		public void Given_valid_16_in_4111111111111111_should_produce_XXXXXXXXXXXXXXXX()
		{
			Verify("4111111111111111", "XXXXXXXXXXXXXXXX");
		}

		[Test]
		public void Given_valid_16_in_a4111111111111111_should_produce_aXXXXXXXXXXXXXXXX()
		{
			Verify("a4111111111111111", "aXXXXXXXXXXXXXXXX");
		}

		[Test]
		public void Given_valid_16_in_14111111111111111_should_produce_1XXXXXXXXXXXXXXXX()
		{
			Verify("14111111111111111", "1XXXXXXXXXXXXXXXX");
		}	
		
		[Test]
		public void Given_valid_16_in_154111111111111111_should_produce_15XXXXXXXXXXXXXXXX()
		{
			Verify("154111111111111111", "15XXXXXXXXXXXXXXXX");
		}

		[Test]
		public void Given_valid_16_in_ab4111111111111111_should_produce_abXXXXXXXXXXXXXXXX()
		{
			Verify("ab4111111111111111", "abXXXXXXXXXXXXXXXX");
		}

		[Test]
		public void Given_valid_16_in_abc4111111111111111_should_produce_abcXXXXXXXXXXXXXXXX()
		{
			Verify("abc4111111111111111", "abcXXXXXXXXXXXXXXXX");
		}

		[Test]
		public void Given_valid_16_in_abcd4111111111111111_should_produce_abcdXXXXXXXXXXXXXXXX()
		{
			Verify("abcd4111111111111111", "abcdXXXXXXXXXXXXXXXX");
		}

		[Test]
		public void Given_valid_15_in_411111111111126_should_produce_XXXXXXXXXXXXXXX()
		{
			Verify("411111111111126", "XXXXXXXXXXXXXXX");
		}		
		
		[Test]
		public void Given_valid_14_in_41111111111114_should_produce_XXXXXXXXXXXXXX()
		{
			Verify("41111111111114", "XXXXXXXXXXXXXX");
		}		
		
		[Test]
		public void Given_overlapping_valid_14_and_16_in_4111111111111400_should_produce_XXXXXXXXXXXXXXXX()
		{
			Verify("4111111111111400", "XXXXXXXXXXXXXXXX");
		}			
		
		[Test]
		public void Given_13_in_41111111111111_should_produce_41111111111111()
		{
			Verify("41111111111111", "41111111111111");
		}		
		
		[Test]
		public void Given_12_in_4111111111111_should_produce_4111111111111()
		{
			Verify("4111111111111", "4111111111111");
		}

		[Test]
		public void Given_valid_14_in_longer_41111111111114a11_should_produce_XXXXXXXXXXXXXXa11()
		{
			Verify("41111111111114a11", "XXXXXXXXXXXXXXa11");
		}

		[Test]
		public void Given_valid_16_in_4111_dash_1111_dash_1111_dash_1111_should_produce_XXXX_dash_XXXX_dash_XXXX_dash_XXXX()
		{
			Verify("4111-1111-1111-1111", "XXXX-XXXX-XXXX-XXXX");
		}

		[Test]
		public void Given_valid_16_in_4111_space_1111_space_1111_space_1111_should_produce_XXXX_space_XXXX_space_XXXX_space_XXXX()
		{
			Verify("4111 1111 1111 1111", "XXXX XXXX XXXX XXXX");
		}

		[Test]
		public void Given_back_to_back_16_should_X_both()
		{
			Verify("4111 1111 1111 1111 4111 1111 1111 1111", "XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX");
		}
		[Test]
		public void Given_varlid_14_followed_by_valid_16_should_X_both()
		{
			Verify("4111 1111 1111 14 4111 1111 1111 1111", "XXXX XXXX XXXX XX XXXX XXXX XXXX XXXX");
		}
	}

	public class CheckSumType
	{
		private static readonly Dictionary<int, int> ChecksumLookup =
			"0246812579".Select((x, i) => new {x = x - '0', i}).ToDictionary(x => x.i, x => x.x);

		public static CheckSumType Even = new CheckSumType(() => Odd, x => ChecksumLookup[x - '0']);
		public static CheckSumType Odd = new CheckSumType(() => Even, x => x - '0');

		private CheckSumType(Func<CheckSumType> getNext, Func<char, int> getCheckValue)
		{
			Next = getNext;
			GetCheckValue = getCheckValue;
		}

		public Func<CheckSumType> Next { get; private set; }
		public Func<char, int> GetCheckValue { get; private set; }
	}

	internal enum State
	{
		PossibleCard,
		NotACard,
		IsACard
	}

	public class CreditCardNumberDetector
	{
		private int _initialNumberOfDigits;
		private static readonly HashSet<char> _validCharacters = new HashSet<char>("0123456789- ");
		private readonly List<int> _indexes = new List<int>();
		private CheckSumType _type;
		private int _checkSumValue;
		private State _state;
		private readonly List<int> _matchingCardLengths = new List<int>();

		public CreditCardNumberDetector(int initialNumberOfDigits)
		{
			Reset();
			_initialNumberOfDigits = initialNumberOfDigits;
			NumberOfDigits = initialNumberOfDigits;
		}

		public int NumberOfDigits { get; private set; }

		public void Add(char character, int index)
		{
			if (_state != State.PossibleCard)
			{
				return;
			}
			if (!_validCharacters.Contains(character))
			{
				Reset();
				return;
			}
			if (!Char.IsNumber(character))
			{
				return;
			}
			if (NumberOfDigits >= 16)
			{
				if (NumberOfDigits > 16)
				{
					Reset();
				}
				else
				{
					NumberOfDigits++;
					return;
				}
			}
			NumberOfDigits++;
			if (_initialNumberOfDigits > 0)
			{
				return;
			}
			_checkSumValue += _type.GetCheckValue(character);
			_indexes.Add(index);
			_type = _type.Next();

			if (NumberOfDigits >= 14)
			{
				if (IsCreditCard())
				{
					_matchingCardLengths.Add(NumberOfDigits);
				}
			}
		}

		public bool IsCreditCard()
		{
			if (_initialNumberOfDigits > 0)
			{
				return false;
			}
			if (_state != State.PossibleCard && _state != State.IsACard)
			{
				return false;
			}
			if (NumberOfDigits < 14 || NumberOfDigits > 16)
			{
				return false;
			}
			return ((_checkSumValue%10) == 0);
		}

		public IEnumerable<int> GetDigitIndexes()
		{
			if (IsCreditCard())
			{
				return _indexes.Take(_matchingCardLengths.Last() - _initialNumberOfDigits);
			}
			return new int[]{};
		}

		public void Reset()
		{
			_indexes.Clear();
			_type = CheckSumType.Even;
			_state = State.PossibleCard;
			NumberOfDigits = 0;
			_matchingCardLengths.Clear();
			_initialNumberOfDigits = 0;
		}
	}
}