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

namespace Scratch.FindSmallestWindowContainingAllElements
{
    /// <summary>
    /// http://stackoverflow.com/questions/3744601/find-the-smallest-window-of-input-array-that-contains-all-the-elements-of-query-a/3746817#3746817
    /// </summary>
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Given_an_input_with_overlapping_matching_windows()
        {
            var inputArray = new[] { 2, 1, 5, 6, 8, 1, 8, 6, 2, 9, 2, 9, 1, 2 };
            var queryArray = new[] { 6, 1, 2 };

            var result = Lists.SmallestWindow(inputArray, queryArray);

            result.ShouldNotBeNull();
            result[0].ShouldBeEqualTo(3);
            result[1].ShouldBeEqualTo(8);
            Console.WriteLine("Smallest window is indexes " + result[0] + " to " + result[1]);
        }
    }
}