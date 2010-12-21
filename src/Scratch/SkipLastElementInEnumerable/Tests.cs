//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************

using System.Linq;

using FluentAssert;

using NUnit.Framework;

namespace Scratch.SkipLastElementInEnumerable
{
    /// <summary>
    /// http://stackoverflow.com/questions/969091/c-skiplast-implementation/971447#971447
    /// </summary>
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Given_input_with_0_items()
        {
            var input = new int[] { };
            var result = input.SkipLast().ToList();
            result.Count.ShouldBeEqualTo(0);
        }

        [Test]
        public void Given_input_with_1_item()
        {
            var input = new[] { 5 };
            var result = input.SkipLast().ToList();
            result.Count.ShouldBeEqualTo(0);
        }

        [Test]
        public void Given_input_with_2_items()
        {
            var input = new[] { 5, 7 };
            var result = input.SkipLast().ToList();
            result.Count.ShouldBeEqualTo(1);
            result[0].ShouldBeEqualTo(input[0]);
        }

        [Test]
        public void Given_input_with_3_items()
        {
            var input = new[] { 5, 7, 9 };
            var result = input.SkipLast().ToList();
            result.Count.ShouldBeEqualTo(2);
            result[0].ShouldBeEqualTo(input[0]);
            result[1].ShouldBeEqualTo(input[1]);
        }
    }
}