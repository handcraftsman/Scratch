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

using FluentAssert;

using NUnit.Framework;

using Scratch.Ranges.FindRangeGaps;

namespace Scratch.FindRangeGaps
{
    public class Tests
    {
        /// <summary>
        /// http://stackoverflow.com/questions/2698143/i-need-to-add-ranges-and-if-a-range-is-missed-while-adding-i-should-display-a-mes/2762570#2762570
        /// http://handcraftsman.wordpress.com/2010/05/02/bdd-kata-find-the-gap-between-two-ranges/
        /// </summary>
        [TestFixture]
        public class When_asked_for_the_gap_between_two_ranges
        {
            private int _expectedEnd;
            private int _expectedStart;
            private Range _gap;
            private Range _originalRange1;
            private Range _originalRange2;
            private Range _range1;
            private Range _range2;

            [Test]
            public void Given_a_null_for_range1()
            {
                Test.Given(new RangeGapFinder())
                    .When(Asked_to_find_the_gap)
                    .With(A_null_range1)
                    .ShouldThrowException<ArgumentNullException>("range1 cannot be null")
                    .Verify();
            }

            [Test]
            public void Given_a_null_for_range2()
            {
                Test.Given(new RangeGapFinder())
                    .When(Asked_to_find_the_gap)
                    .With(A_null_range2)
                    .ShouldThrowException<ArgumentNullException>("range2 cannot be null")
                    .Verify();
            }

            [Test]
            public void Given_a_value_gap()
            {
                Test.Given(new RangeGapFinder())
                    .When(Asked_to_find_the_gap)
                    .With(A_value_gap)
                    .Should(Get_a_non_null_result)
                    .Should(Get_the_correct_range_start_value)
                    .Should(Get_the_correct_range_end_value)
                    .Verify();
            }

            [Test]
            public void Given_adjacent_ranges()
            {
                Test.Given(new RangeGapFinder())
                    .When(Asked_to_find_the_gap)
                    .With(Adjacent_ranges)
                    .Should(Get_a_null_result)
                    .Verify();
            }

            [Test]
            public void Given_first_range_End_value_equal_to_second_range_Start_value()
            {
                Test.Given(new RangeGapFinder())
                    .When(Asked_to_find_the_gap)
                    .With(First_range_End_value_same_as_second_range_Start_value)
                    .Should(Get_a_null_result)
                    .Verify();
            }

            [Test]
            public void Given_range1_End_value_lower_than_its_Start_value()
            {
                Test.Given(new RangeGapFinder())
                    .When(Asked_to_find_the_gap)
                    .With(Range1_end_value_lower_than_its_start_value)
                    .Should(Get_a_non_null_result)
                    .Should(Get_the_correct_range_start_value)
                    .Should(Get_the_correct_range_end_value)
                    .Should(Not_change_the_values_of_range1)
                    .Should(Not_change_the_values_of_range2)
                    .Verify();
            }

            [Test]
            public void Given_range2_End_value_lower_than_its_Start_value()
            {
                Test.Given(new RangeGapFinder())
                    .When(Asked_to_find_the_gap)
                    .With(Range2_end_value_lower_than_its_start_value)
                    .Should(Get_a_non_null_result)
                    .Should(Get_the_correct_range_start_value)
                    .Should(Get_the_correct_range_end_value)
                    .Should(Not_change_the_values_of_range1)
                    .Should(Not_change_the_values_of_range2)
                    .Verify();
            }

            [Test]
            public void Given_range2_Start_value_lower_than_range1_Start_value()
            {
                Test.Given(new RangeGapFinder())
                    .When(Asked_to_find_the_gap)
                    .With(Second_range_Start_lower_than_first_range_Start)
                    .Should(Get_a_non_null_result)
                    .Should(Get_the_correct_range_start_value)
                    .Should(Get_the_correct_range_end_value)
                    .Verify();
            }

            [Test]
            public void Given_second_range_starts_in_the_first_range()
            {
                Test.Given(new RangeGapFinder())
                    .When(Asked_to_find_the_gap)
                    .With(Second_range_starting_in_the_first_range)
                    .Should(Get_a_null_result)
                    .Verify();
            }

            private void A_null_range1()
            {
                _range1 = null;
                _range2 = new Range(1, 5);
            }

            private void A_null_range2()
            {
                _range1 = new Range(1, 5);
                _range2 = null;
            }

            public void A_value_gap()
            {
                _range1 = new Range(1, 5);
                _range2 = new Range(10, 15);
                _expectedStart = 6;
                _expectedEnd = 9;
            }

            private void Adjacent_ranges()
            {
                _range1 = new Range(1, 5);
                _range2 = new Range(6, 10);
            }

            private void Asked_to_find_the_gap(RangeGapFinder rangeGapFinder)
            {
                if (_range1 != null)
                {
                    _originalRange1 = new Range(_range1.Start, _range1.End);
                }
                if (_range2 != null)
                {
                    _originalRange2 = new Range(_range2.Start, _range2.End);
                }
                _gap = rangeGapFinder.FindGap(_range1, _range2);
            }

            private void First_range_End_value_same_as_second_range_Start_value()
            {
                _range1 = new Range(1, 5);
                _range2 = new Range(5, 9);
            }

            private void Get_a_non_null_result()
            {
                _gap.ShouldNotBeNull();
            }

            private void Get_a_null_result()
            {
                _gap.ShouldBeNull();
            }

            private void Get_the_correct_range_end_value()
            {
                _gap.End.ShouldBeEqualTo(_expectedEnd);
            }

            private void Get_the_correct_range_start_value()
            {
                _gap.Start.ShouldBeEqualTo(_expectedStart);
            }

            private void Not_change_the_values_of_range1()
            {
                _range1.Start.ShouldBeEqualTo(_originalRange1.Start);
                _range1.End.ShouldBeEqualTo(_originalRange1.End);
            }

            private void Not_change_the_values_of_range2()
            {
                _range2.Start.ShouldBeEqualTo(_originalRange2.Start);
                _range2.End.ShouldBeEqualTo(_originalRange2.End);
            }

            private void Range1_end_value_lower_than_its_start_value()
            {
                _range1 = new Range(5, 1);
                _range2 = new Range(10, 15);
                _expectedStart = 6;
                _expectedEnd = 9;
            }

            private void Range2_end_value_lower_than_its_start_value()
            {
                _range1 = new Range(1, 5);
                _range2 = new Range(15, 10);
                _expectedStart = 6;
                _expectedEnd = 9;
            }

            private void Second_range_Start_lower_than_first_range_Start()
            {
                _range1 = new Range(10, 15);
                _range2 = new Range(1, 5);
                _expectedStart = 6;
                _expectedEnd = 9;
            }

            private void Second_range_starting_in_the_first_range()
            {
                _range1 = new Range(1, 5);
                _range2 = new Range(2, 9);
            }
        }
    }
}