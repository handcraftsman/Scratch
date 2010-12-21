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