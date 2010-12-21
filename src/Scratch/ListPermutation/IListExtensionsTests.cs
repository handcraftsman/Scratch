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

namespace Scratch.ListPermutation
{
    /// <summary>
    /// http://handcraftsman.wordpress.com/2010/11/11/generating-all-permutations-of-a-sequence-in-csharp/
    /// </summary>
    public class IListExtensionsTests
    {
        [TestFixture]
        public class When_asked_to_generate_all_permutations_of_a_specific_length_from_a_list_of_characters
        {
            private Exception _exception;
            private int _expectedPermutationCount;
            private IList<char> _input;
            private int _length;
            private IEnumerable<IList<char>> _result;

            [Test]
            public void Given_a_list_with_3_items_and_length_2()
            {
                Test.Verify(
                    with_a_list_of_3_items,
                    and_a_requested_length_of_2,
                    when_asked_to_generate_all_permutations_of_the_requested_length_from_the_list,
                    should_return_a_list_having_the_expected_number_of_permutations,
                    all_permutations_should_be_unique,
                    each_permutation_should_have_disinct_elements,
                    each_permutation_should_have_the_requested_length
                    );
            }

            [Test]
            public void Given_a_list_with_4_items_and_length_equal_to_the_number_of_items_in_the_list()
            {
                Test.Verify(
                    with_a_list_of_4_items,
                    and_a_requested_length_equal_to_the_number_of_items_in_the_list,
                    when_asked_to_generate_all_permutations_of_the_requested_length_from_the_list,
                    should_return_a_list_having_the_expected_number_of_permutations,
                    all_permutations_should_be_unique,
                    each_permutation_should_have_disinct_elements,
                    each_permutation_should_have_the_requested_length
                    );
            }

            [Test]
            public void Given_a_non_empty_list_and_a_negative_length()
            {
                Test.Verify(
                    with_a_non_empty_list,
                    and_a_negative_requested_length,
                    when_asked_to_generate_all_permutations_of_the_requested_length_from_the_list,
                    should_return_an_empty_list_of_permutations
                    );
            }

            [Test]
            public void Given_a_non_empty_list_and_length_1()
            {
                Test.Verify(
                    with_a_non_empty_list,
                    and_a_requested_length_of_1,
                    when_asked_to_generate_all_permutations_of_the_requested_length_from_the_list,
                    should_return_a_list_having_the_expected_number_of_permutations,
                    each_input_should_appear_in_the_result_list_only_once
                    );
            }

            [Test]
            public void Given_a_non_empty_list_and_length_greater_than_the_number_of_items_in_the_list()
            {
                Test.Verify(
                    with_a_non_empty_list,
                    and_a_requested_length_greater_than_the_number_of_items_in_the_list,
                    when_asked_to_generate_all_permutations_of_the_requested_length_from_the_list,
                    should_throw_an_ArgumentOutOfRangeException
                    );
            }

            [Test]
            public void Given_a_non_empty_list_and_length_zero()
            {
                Test.Verify(
                    with_a_non_empty_list,
                    and_a_requested_length_of_zero,
                    when_asked_to_generate_all_permutations_of_the_requested_length_from_the_list,
                    should_return_an_empty_list_of_permutations
                    );
            }

            [Test]
            public void Given_a_null_list()
            {
                Test.Verify(
                    with_a_null_list,
                    and_a_requested_length_of_2,
                    when_asked_to_generate_all_permutations_of_the_requested_length_from_the_list,
                    should_return_an_empty_list_of_permutations
                    );
            }

            [Test]
            public void Given_an_empty_list()
            {
                Test.Verify(
                    with_an_empty_list,
                    and_a_requested_length_of_2,
                    when_asked_to_generate_all_permutations_of_the_requested_length_from_the_list,
                    should_return_an_empty_list_of_permutations
                    );
            }

            private void SetExpectedPermutationCount()
            {
                if (_input == null ||
                    _input.Count == 0 ||
                    _input.Count < _length)
                {
                    _expectedPermutationCount = 0;
                }
                else
                {
                    _expectedPermutationCount
                        = _input.Count.Factorial()
                          / (_input.Count - _length).Factorial();
                }
            }

            private void all_permutations_should_be_unique()
            {
                _result.Count().ShouldBeEqualTo(_expectedPermutationCount);
                _result
                    .Select(x => String.Join("|", x.Select(y => y.ToString()).ToArray()))
                    .Distinct()
                    .Count().ShouldBeEqualTo(_expectedPermutationCount);
            }

            private void and_a_negative_requested_length()
            {
                _length = -3;
            }

            private void and_a_requested_length_equal_to_the_number_of_items_in_the_list()
            {
                _length = _input.Count;
            }

            private void and_a_requested_length_greater_than_the_number_of_items_in_the_list()
            {
                _length = 1 + _input.Count;
            }

            private void and_a_requested_length_of_1()
            {
                _length = 1;
            }

            private void and_a_requested_length_of_2()
            {
                _length = 2;
            }

            private void and_a_requested_length_of_zero()
            {
                _length = 0;
            }

            private void each_input_should_appear_in_the_result_list_only_once()
            {
                foreach (char value in _input)
                {
                    _result.Count(x => x.Contains(value)).ShouldBeEqualTo(1);
                }
            }

            private void each_permutation_should_have_disinct_elements()
            {
                foreach (var value in _result)
                {
                    value.Distinct().Count().ShouldBeEqualTo(_length);
                }
            }

            private void each_permutation_should_have_the_requested_length()
            {
                foreach (var value in _result)
                {
                    value.Count.ShouldBeEqualTo(_length);
                }
            }

            private void should_return_a_list_having_the_expected_number_of_permutations()
            {
                _result.Count().ShouldBeEqualTo(_expectedPermutationCount);
            }

            private void should_return_an_empty_list_of_permutations()
            {
                _result.Count().ShouldBeEqualTo(0);
            }

            private void should_throw_an_ArgumentOutOfRangeException()
            {
                _exception.ShouldNotBeNull();
                _exception.GetType().ShouldBeEqualTo(typeof(ArgumentOutOfRangeException));
            }

            private void when_asked_to_generate_all_permutations_of_the_requested_length_from_the_list()
            {
                SetExpectedPermutationCount();

                try
                {
                    _result = _input.Permute(_length).ToList();
                }
                catch (Exception e)
                {
                    _exception = e;
                }
            }

            private void with_a_list_of_3_items()
            {
                _input = new List<char>
                    {
                        'A',
                        'B',
                        'C'
                    };
            }

            private void with_a_list_of_4_items()
            {
                _input = new List<char>
                    {
                        'A',
                        'B',
                        'C',
                        'D'
                    };
            }

            private void with_a_non_empty_list()
            {
                with_a_list_of_3_items();
            }

            private void with_a_null_list()
            {
                _input = null;
            }

            private void with_an_empty_list()
            {
                _input = new List<char>();
            }
        }

        [TestFixture]
        public class When_asked_to_generate_all_permutations_of_a_specific_length_from_a_list_of_integers
        {
            private Exception _exception;
            private int _expectedPermutationCount;
            private IList<int> _input;
            private int _length;
            private IEnumerable<IList<int>> _result;

            [Test]
            public void Given_a_list_with_3_items_and_length_2()
            {
                Test.Verify(
                    with_a_list_of_3_items,
                    and_a_requested_length_of_2,
                    when_asked_to_generate_all_permutations_of_the_requested_length_from_the_list,
                    should_return_a_list_having_the_expected_number_of_permutations,
                    all_permutations_should_be_unique,
                    each_permutation_should_have_disinct_elements,
                    each_permutation_should_have_the_requested_length
                    );
            }

            [Test]
            public void Given_a_list_with_4_items_and_length_equal_to_the_number_of_items_in_the_list()
            {
                Test.Verify(
                    with_a_list_of_4_items,
                    and_a_requested_length_equal_to_the_number_of_items_in_the_list,
                    when_asked_to_generate_all_permutations_of_the_requested_length_from_the_list,
                    should_return_a_list_having_the_expected_number_of_permutations,
                    all_permutations_should_be_unique,
                    each_permutation_should_have_disinct_elements,
                    each_permutation_should_have_the_requested_length
                    );
            }

            [Test]
            public void Given_a_non_empty_list_and_a_negative_length()
            {
                Test.Verify(
                    with_a_non_empty_list,
                    and_a_negative_requested_length,
                    when_asked_to_generate_all_permutations_of_the_requested_length_from_the_list,
                    should_return_an_empty_list_of_permutations
                    );
            }

            [Test]
            public void Given_a_non_empty_list_and_length_1()
            {
                Test.Verify(
                    with_a_non_empty_list,
                    and_a_requested_length_of_1,
                    when_asked_to_generate_all_permutations_of_the_requested_length_from_the_list,
                    should_return_a_list_having_the_expected_number_of_permutations,
                    each_input_should_appear_in_the_result_list_only_once
                    );
            }

            [Test]
            public void Given_a_non_empty_list_and_length_greater_than_the_number_of_items_in_the_list()
            {
                Test.Verify(
                    with_a_non_empty_list,
                    and_a_requested_length_greater_than_the_number_of_items_in_the_list,
                    when_asked_to_generate_all_permutations_of_the_requested_length_from_the_list,
                    should_throw_an_ArgumentOutOfRangeException
                    );
            }

            [Test]
            public void Given_a_non_empty_list_and_length_zero()
            {
                Test.Verify(
                    with_a_non_empty_list,
                    and_a_requested_length_of_zero,
                    when_asked_to_generate_all_permutations_of_the_requested_length_from_the_list,
                    should_return_an_empty_list_of_permutations
                    );
            }

            [Test]
            public void Given_a_null_list()
            {
                Test.Verify(
                    with_a_null_list,
                    and_a_requested_length_of_2,
                    when_asked_to_generate_all_permutations_of_the_requested_length_from_the_list,
                    should_return_an_empty_list_of_permutations
                    );
            }

            [Test]
            public void Given_an_empty_list()
            {
                Test.Verify(
                    with_an_empty_list,
                    and_a_requested_length_of_2,
                    when_asked_to_generate_all_permutations_of_the_requested_length_from_the_list,
                    should_return_an_empty_list_of_permutations
                    );
            }

            private void SetExpectedPermutationCount()
            {
                if (_input == null ||
                    _input.Count == 0 ||
                    _input.Count < _length)
                {
                    _expectedPermutationCount = 0;
                }
                else
                {
                    _expectedPermutationCount
                        = _input.Count.Factorial()
                          / (_input.Count - _length).Factorial();
                }
            }

            private void all_permutations_should_be_unique()
            {
                _result.Count().ShouldBeEqualTo(_expectedPermutationCount);
                _result
                    .Select(x => String.Join("|", x.Select(y => y.ToString()).ToArray()))
                    .Distinct()
                    .Count()
                    .ShouldBeEqualTo(_expectedPermutationCount);
            }

            private void and_a_negative_requested_length()
            {
                _length = -3;
            }

            private void and_a_requested_length_equal_to_the_number_of_items_in_the_list()
            {
                _length = _input.Count;
            }

            private void and_a_requested_length_greater_than_the_number_of_items_in_the_list()
            {
                _length = 1 + _input.Count;
            }

            private void and_a_requested_length_of_1()
            {
                _length = 1;
            }

            private void and_a_requested_length_of_2()
            {
                _length = 2;
            }

            private void and_a_requested_length_of_zero()
            {
                _length = 0;
            }

            private void each_input_should_appear_in_the_result_list_only_once()
            {
                foreach (int value in _input)
                {
                    _result.Count(x => x.Contains(value)).ShouldBeEqualTo(1);
                }
            }

            private void each_permutation_should_have_disinct_elements()
            {
                foreach (var value in _result)
                {
                    value.Distinct().Count().ShouldBeEqualTo(_length);
                }
            }

            private void each_permutation_should_have_the_requested_length()
            {
                foreach (var value in _result)
                {
                    value.Count.ShouldBeEqualTo(_length);
                }
            }

            private void should_return_a_list_having_the_expected_number_of_permutations()
            {
                _result.Count().ShouldBeEqualTo(_expectedPermutationCount);
            }

            private void should_return_an_empty_list_of_permutations()
            {
                _result.Count().ShouldBeEqualTo(0);
            }

            private void should_throw_an_ArgumentOutOfRangeException()
            {
                _exception.ShouldNotBeNull();
                _exception.GetType().ShouldBeEqualTo(typeof(ArgumentOutOfRangeException));
            }

            private void when_asked_to_generate_all_permutations_of_the_requested_length_from_the_list()
            {
                SetExpectedPermutationCount();

                try
                {
                    _result = _input.Permute(_length).ToList();
                }
                catch (Exception e)
                {
                    _exception = e;
                }
            }

            private void with_a_list_of_3_items()
            {
                _input = new List<int>
                    {
                        1,
                        2,
                        3
                    };
            }

            private void with_a_list_of_4_items()
            {
                _input = Enumerable.Range(1, 4).ToList();
            }

            private void with_a_non_empty_list()
            {
                with_a_list_of_3_items();
            }

            private void with_a_null_list()
            {
                _input = null;
            }

            private void with_an_empty_list()
            {
                _input = new List<int>();
            }
        }
    }
}