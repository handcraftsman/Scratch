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

namespace Scratch.ListsHaveSameContents
{
    /// <summary>
    /// http://stackoverflow.com/questions/2913287/comparing-arrays-using-linq-in-c/2913406#2913406
    /// </summary>
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Given_different_lengths()
        {
            string[] a = { "a", "b", "c" };
            string[] b = { "b", "c" };

            bool containSame = a.HasSameContentsAs(b);
            containSame.ShouldBeFalse();
        }

        [Test]
        public void Given_duplicated_items_have_different_counts()
        {
            string[] a = { "a", "b", "b", "b", "c" };
            string[] b = { "a", "b", "c", "b", "c" };

            bool containSame = a.HasSameContentsAs(b);
            containSame.ShouldBeFalse();
        }

        [Test]
        public void Given_duplicated_items_have_same_count()
        {
            string[] a = { "a", "b", "b", "c" };
            string[] b = { "a", "b", "c", "b" };

            bool containSame = a.HasSameContentsAs(b);
            containSame.ShouldBeTrue();
        }

        [Test]
        public void Given_same_length_but_contents_in_different_order()
        {
            string[] a = { "a", "b", "c" };
            string[] b = { "c", "a", "b" };

            bool containSame = a.HasSameContentsAs(b);
            containSame.ShouldBeTrue();
        }
    }
}