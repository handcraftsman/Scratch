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
using System.Linq;

using FluentAssert;

using NUnit.Framework;

namespace Scratch.PrimeFactors
{
    /// <summary>
    /// http://stackoverflow.com/questions/3596324/how-to-factor-a-number-functionally/3622884#3622884
    /// </summary>
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Should_factor_825_as_0_1_2_0_1()
        {
            var factors = Numeric.Factorize(825)
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count());

            var outputs = PrimeNumbers.Numeric.Primes()
                .TakeWhile(x => x <= factors.Max(y => y.Key))
                .Select(x => factors.ContainsKey(x) ? factors[x] : 0)
                .Select(x => x.ToString())
                .ToArray();

            string buffer = String.Join(", ", outputs);
            buffer.ShouldBeEqualTo("0, 1, 2, 0, 1");
        }
    }
}