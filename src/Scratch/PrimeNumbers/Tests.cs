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
using System.Diagnostics;
using System.Linq;

using FluentAssert;

using NUnit.Framework;

namespace Scratch.PrimeNumbers
{
    /// <summary>
    /// http://handcraftsman.wordpress.com/2010/09/02/ienumerable-of-prime-numbers-in-csharp/
    /// </summary>
    [TestFixture]
    public class Tests
    {
        /// <summary>
        /// http://www.bigprimes.net/cruncher/1299709/
        /// 
        /// expect ~0.68440924
        /// </summary>
        [Test]
        public void Time_to_get_100000th_Prime()
        {
            const int primeIndex = 100000;
            const int runs = 100;
            const int expect = 1299709;

            Time(runs, primeIndex, expect);
        }

        /// <summary>
        /// http://www.bigprimes.net/cruncher/104729/
        /// 
        /// expect ~0.02979869
        /// </summary>
        [Test]
        public void Time_to_get_10000th_Prime()
        {
            const int primeIndex = 10000;
            const int runs = 1000;
            const int expect = 104729;

            Time(runs, primeIndex, expect);
        }

        /// <summary>
        /// http://www.bigprimes.net/cruncher/7919/
        /// 
        /// expect ~0.00138686
        /// </summary>
        [Test]
        public void Time_to_get_1000th_Prime()
        {
            const int primeIndex = 1000;
            const int runs = 1000;
            const int expect = 7919;

            Time(runs, primeIndex, expect);
        }

        private static void Time(int runs, int primeIndex, int expect)
        {
            var stopwatch = new Stopwatch();
            int prime = 0;
            for (int i = 0; i < runs; i++)
            {
                stopwatch.Start();
                prime = Numeric.Primes().Skip(primeIndex - 1).First();
                stopwatch.Stop();
            }
            prime.ShouldBeEqualTo(expect);
            Console.WriteLine("avg time across " + runs + " runs to find Prime # " + primeIndex + " is " + stopwatch.Elapsed.TotalSeconds / runs);
        }
    }
}