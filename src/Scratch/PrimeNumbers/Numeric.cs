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

namespace Scratch.PrimeNumbers
{
    /// <summary>
    /// http://handcraftsman.wordpress.com/2010/09/02/ienumerable-of-prime-numbers-in-csharp/
    /// </summary>
    public static class Numeric
    {
        private static IEnumerable<int> PotentialPrimes()
        {
            yield return 2;
            yield return 3;
            int k = 1;
            while (k > 0)
            {
                yield return 6 * k - 1;
                yield return 6 * k + 1;
                k++;
            }
        }

        public static IEnumerable<int> Primes()
        {
            return PotentialPrimes().Where(x =>
                {
                    double sqrt = Math.Sqrt(x);
                    return !PotentialPrimes()
                                .TakeWhile(y => y <= sqrt)
                                .Any(y => x % y == 0);
                });
        }
    }
}