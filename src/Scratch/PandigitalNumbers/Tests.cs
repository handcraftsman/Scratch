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

namespace Scratch.PandigitalNumbers
{
    /// <summary>
    /// http://stackoverflow.com/questions/2484892/fastest-algorithm-to-check-if-a-number-is-pandigital/3742448#3742448
    /// </summary>
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Given_range()
        {
            int pans = 0;
            for (int i = 123456789; i <= 123987654; i++)
            {
                if (Numeric.IsPandigital(i))
                {
                    pans++;
                }
            }

            pans.ShouldBeEqualTo(720);
        }
    }
}