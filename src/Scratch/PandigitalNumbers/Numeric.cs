//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************
namespace Scratch.PandigitalNumbers
{
    /// <summary>
    /// http://stackoverflow.com/questions/2484892/fastest-algorithm-to-check-if-a-number-is-pandigital/3742448#3742448
    /// </summary>
    public static class Numeric
    {
        public static bool IsPandigital(int n)
        {
            int count = 0;
            int digits = 0;
            int digit;
            int bit;
            do
            {
                digit = n - ((n /= 10) * 10);
                if (digit == 0)
                {
                    return false;
                }
                bit = 1 << digit;

                if (digits == (digits |= bit))
                {
                    return false;
                }

                count++;
            } while (n > 0);
            return (1 << count) - 1 == digits >> 1;
        }
    }
}