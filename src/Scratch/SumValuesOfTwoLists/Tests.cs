//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************

using System.Collections.Generic;

using FluentAssert;

using NUnit.Framework;

namespace Scratch.SumValuesOfTwoLists
{
    /// <summary>
    /// http://stackoverflow.com/questions/1190657/add-two-lists-of-different-length-in-c/1241495#1241495
    /// </summary>
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Given_2_lists_of_equal_length()
        {
            var a = new List<double>
                {
                    3,
                    4,
                    5
                };
            var b = new List<double>
                {
                    1,
                    2,
                    3
                };

            var result = Numeric.Sum(a, b);
            result.Count.ShouldBeEqualTo(a.Count);
            result[0].ShouldBeEqualTo(a[0] + b[0]);
            result[1].ShouldBeEqualTo(a[1] + b[1]);
            result[2].ShouldBeEqualTo(a[2] + b[2]);
        }

        [Test]
        public void Given_2_lists_where_first_is_longer()
        {
            var a = new List<double>
                {
                    1,
                    2,
                    3,
                    4,
                    5
                };
            var b = new List<double>
                {
                    1,
                    2,
                    3
                };

            var result = Numeric.Sum(a, b);
            result.Count.ShouldBeEqualTo(a.Count);
            result[0].ShouldBeEqualTo(a[0] + b[0]);
            result[1].ShouldBeEqualTo(a[1] + b[1]);
            result[2].ShouldBeEqualTo(a[2] + b[2]);
            result[3].ShouldBeEqualTo(a[3]);
            result[4].ShouldBeEqualTo(a[4]);
        }

        [Test]
        public void Given_2_lists_where_second_is_longer()
        {
            var a = new List<double>
                {
                    1,
                    2,
                    3
                };
            var b = new List<double>
                {
                    1,
                    2,
                    3,
                    4,
                    5
                };

            var result = Numeric.Sum(a, b);
            result.Count.ShouldBeEqualTo(b.Count);
            result[0].ShouldBeEqualTo(a[0] + b[0]);
            result[1].ShouldBeEqualTo(a[1] + b[1]);
            result[2].ShouldBeEqualTo(a[2] + b[2]);
            result[3].ShouldBeEqualTo(b[3]);
            result[4].ShouldBeEqualTo(b[4]);
        }
    }
}