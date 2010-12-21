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

namespace Scratch.PrimeFactors
{
    /// <summary>
    /// http://stackoverflow.com/questions/3596324/how-to-factor-a-number-functionally/3622884#3622884
    /// </summary>
    public static class Numeric
    {
        public static IEnumerable<int> Factorize(int input)
        {
            int first = Primes()
                .TakeWhile(x => x <= Math.Sqrt(input))
                .FirstOrDefault(x => input % x == 0);
            return first == 0
                       ? new[] { input }
                       : new[] { first }.Concat(Factorize(input / first));
        }

        public static IEnumerable<int> Primes()
        {
            var ints = Enumerable.Range(2, Int32.MaxValue - 1);
            return ints.Where(x => !ints
                                        .TakeWhile(y => y <= Math.Sqrt(x))
                                        .Any(y => x % y == 0));
        }
    }
}