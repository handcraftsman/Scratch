//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************

using FluentAssert;

using NUnit.Framework;

namespace Scratch.FindOutOfOrderListItems
{
    /// <summary>
    /// http://stackoverflow.com/questions/998073/compare-two-lists-or-arrays-of-arbitrary-length-in-c-order-is-important/998907#998907
    /// </summary>
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Given_inputs_with_1_pair_of_items_out_of_order()
        {
            string[] expected = { "a", "c", "b", "d", "f", "e" };
            string[] actual = { "a", "d", "e", "f", "h" };
            var index = actual.FindOutOfOrderItems(expected);
            index.ShouldBeEqualTo(2);
        }

        [Test]
        public void Given_inputs_with_all_items_in_order()
        {
            string[] expected = { "a", "c", "b", "d", "f", "e" };
            string[] actual = { "a", "d", "f", "h" };
            var indexes = actual.FindOutOfOrderItems(expected);
            indexes.ShouldBeNull();
        }
    }
}