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

namespace Scratch.ListPermutation
{
    /// <summary>
    /// http://handcraftsman.wordpress.com/2010/11/11/generating-all-permutations-of-a-sequence-in-csharp/
    /// </summary>
    public static class Int32Extensions
    {
        public static int Factorial(this int n)
        {
            if (n < 0)
            {
                throw new ArgumentOutOfRangeException("n", "input must be greater than or equal to 0");
            }
            if (n == 0)
            {
                return 1;
            }
            return n * Factorial(n - 1);
        }
    }
}