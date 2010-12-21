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

namespace Scratch.ManOrBoyCompiler
{
    /// <summary>
    /// http://stackoverflow.com/questions/1746931/how-does-the-man-or-boy-knuth-test-work
    /// http://en.wikipedia.org/wiki/Man_or_boy_test
    /// </summary>
    [TestFixture]
    public class Tests
    {
        private int _indent;

        [Test]
        public void Given_k_is_0()
        {
            const int k = 0;
            int result = CallA(k);
            result.ShouldBeEqualTo(1);
        }

        [Test]
        public void Given_k_is_1()
        {
            const int k = 1;
            int result = CallA(k);
            result.ShouldBeEqualTo(0);
        }

        [Test]
        public void Given_k_is_10()
        {
            const int k = 10;
            int result = CallA(k);
            result.ShouldBeEqualTo(-67);
        }

        [Test]
        public void Given_k_is_2()
        {
            const int k = 2;
            int result = CallA(k);
            result.ShouldBeEqualTo(-2);
        }

        [Test]
        public void Given_k_is_3()
        {
            const int k = 3;
            int result = CallA(k);
            result.ShouldBeEqualTo(0);
        }

        [Test]
        public void Given_k_is_4()
        {
            const int k = 4;
            int result = CallA(k);
            result.ShouldBeEqualTo(1);
        }

        [Test]
        public void Given_k_is_5()
        {
            const int k = 5;
            int result = CallA(k);
            result.ShouldBeEqualTo(0);
        }

        [Test]
        public void Given_k_is_6()
        {
            const int k = 6;
            int result = CallA(k);
            result.ShouldBeEqualTo(1);
        }

        [Test]
        public void Given_k_is_7()
        {
            const int k = 7;
            int result = CallA(k);
            result.ShouldBeEqualTo(-1);
        }

        [Test]
        public void Given_k_is_8()
        {
            const int k = 8;
            int result = CallA(k);
            result.ShouldBeEqualTo(-10);
        }

        [Test]
        public void Given_k_is_9()
        {
            const int k = 9;
            int result = CallA(k);
            result.ShouldBeEqualTo(-30);
        }

        public int A(int k, object x1, object x2, object x3, object x4, object x5)
        {
            _indent++;
            Console.WriteLine("".PadLeft(_indent * 2) + "k = " + k);
            Func<int> b = () => 0;
            int result = 0;
            b = () =>
                    {
                        k--;
                        result = A(k, b, x1, x2, x3, x4);
                        return result;
                    };
            if (k <= 0)
            {
                bool x4IsInt = x4 is int;
                if (!x4IsInt)
                {
                    Console.WriteLine("".PadLeft(_indent * 2) + "calling x4()");
                }
                int x4Value = x4IsInt ? (int)x4 : ((Func<int>)x4)();
                Console.WriteLine("".PadLeft(_indent * 2) + "x4 = " + x4Value);
                bool x5IsInt = x5 is int;
                if (!x5IsInt)
                {
                    Console.WriteLine("".PadLeft(_indent * 2) + "calling x5()");
                }
                int x5Value = x5IsInt ? (int)x5 : ((Func<int>)x5)();
                Console.WriteLine("".PadLeft(_indent * 2) + "x5 = " + x5Value);
                result = x4Value + x5Value;
                Console.WriteLine("".PadLeft(_indent * 2) + "result of x4 + x5 = " + result);
            }
            else
            {
                b();
                Console.WriteLine("".PadLeft(_indent * 2) + "result of b() = " + result);
            }
            _indent--;
            return result;
        }

        private int CallA(int k)
        {
            return A(k, 1, -1, -1, 1, 0);
        }
    }
}