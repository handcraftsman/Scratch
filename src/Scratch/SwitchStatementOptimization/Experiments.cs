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
using System.Diagnostics;
using System.Linq;

using NUnit.Framework;

namespace Scratch.SwitchStatementOptimization
{
    /// <summary>
    /// http://stackoverflow.com/questions/1837546/how-would-you-make-this-switch-statement-as-fast-as-possible
    /// </summary>
    [TestFixture]
    public class Experiments
    {
        private readonly string[] _allCodes = new[] { "", "A", "B", "BT", "C", "MW", "N", "PA", "Q", "QD", "W", "X", "Y", null };
        private List<string> _codes;

        [TestFixtureSetUp]
        public void BeforeFirstTest()
        {
            foreach (string code in _allCodes.Where(x => x != null))
            {
                Console.WriteLine(code + " = " + code.GetHashCode() + ",");
            }
            foreach (var code in Enum.GetValues(typeof(MarketDataExchange)))
            {
                Console.WriteLine((int)code);
            }

            _codes = Enumerable.Range(0, 10000000)
                .Select(x => _allCodes[x % _allCodes.Length])
                .ToList();
        }

        [Test]
        public void TimeJoaoAngeloEnum()
        {
            Console.WriteLine(Time<MarketDataExchangeJoaoAngelo>(GetMarketDataExchangeJoaoAngelo));
        }

        [Test]
        public void TimeMyEnum()
        {
            Console.WriteLine(Time<MarketDataExchange>(GetMarketDataExchange));
        }

        [Test]
        public void TimeOriginalEnum()
        {
            Console.WriteLine(Time<MarketDataExchangeOriginal>(GetMarketDataExchangeOriginal));
        }

        public static MarketDataExchange GetMarketDataExchange(string ActivCode)
        {
            if (ActivCode == null)
            {
                return MarketDataExchange.NONE;
            }
            if (ActivCode.Length == 0)
            {
                return MarketDataExchange.NBBO;
            }
            return (MarketDataExchange)((ActivCode[0] << ActivCode.Length));
        }

        public static MarketDataExchangeJoaoAngelo GetMarketDataExchangeJoaoAngelo(string ActivCode)
        {
            if (ActivCode == null)
            {
                return MarketDataExchangeJoaoAngelo.NONE;
            }

            return (MarketDataExchangeJoaoAngelo)ActivCode.GetHashCode();
        }

        public static MarketDataExchangeOriginal GetMarketDataExchangeOriginal(string ActivCode)
        {
            if (ActivCode == null)
            {
                return MarketDataExchangeOriginal.NONE;
            }

            switch (ActivCode)
            {
                case "":
                    return MarketDataExchangeOriginal.NBBO;
                case "A":
                    return MarketDataExchangeOriginal.AMEX;
                case "B":
                    return MarketDataExchangeOriginal.BSE;
                case "BT":
                    return MarketDataExchangeOriginal.BATS;
                case "C":
                    return MarketDataExchangeOriginal.NSE;
                case "MW":
                    return MarketDataExchangeOriginal.CHX;
                case "N":
                    return MarketDataExchangeOriginal.NYSE;
                case "PA":
                    return MarketDataExchangeOriginal.ARCA;
                case "Q":
                    return MarketDataExchangeOriginal.NASDAQ;
                case "QD":
                    return MarketDataExchangeOriginal.NASDAQ_ADF;
                case "W":
                    return MarketDataExchangeOriginal.CBOE;
                case "X":
                    return MarketDataExchangeOriginal.PHLX;
                case "Y":
                    return MarketDataExchangeOriginal.DIRECTEDGE;
            }

            return MarketDataExchangeOriginal.NONE;
        }

        private string Time<T>(Func<string, T> getMarketDataExchange)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < _codes.Count; i++)
            {
                getMarketDataExchange(_codes[i]);
            }
            stopwatch.Stop();
            return _codes.Count + " runs: " + stopwatch.ElapsedMilliseconds + " ms" + Environment.NewLine
                   + "Average: " + (stopwatch.ElapsedMilliseconds / (decimal)_codes.Count) + " microsecond";
        }
    }

    public enum MarketDataExchangeJoaoAngelo
    {
        NONE,
        NBBO = 371857150,
        AMEX = 372029405,
        BSE = 372029408,
        BATS = -1850320644,
        NSE = 372029407,
        CHX = -284236702,
        NYSE = 372029412,
        ARCA = -734575383,
        NASDAQ = 372029421,
        NASDAQ_ADF = -1137859911,
        CBOE = 372029419,
        PHLX = 372029430,
        DIRECTEDGE = 372029429
    }

    public enum MarketDataExchangeOriginal
    {
        NONE,
        NBBO,
        AMEX,
        BSE,
        BATS,
        NSE,
        CHX,
        NYSE,
        ARCA,
        NASDAQ,
        NASDAQ_ADF,
        CBOE,
        PHLX,
        DIRECTEDGE
    }

    public enum MarketDataExchange
    {
        NONE = 0,
        NBBO = 1,
        AMEX = ('A' << 1),
        BSE = ('B' << 1),
        BATS = ('B' << 2),
        NSE = ('C' << 1),
        CHX = ('M' << 2),
        NYSE = ('N' << 1),
        ARCA = ('P' << 2),
        NASDAQ = ('Q' << 1),
        NASDAQ_ADF = ('Q' << 2),
        CBOE = ('W' << 1),
        PHLX = ('X' << 1),
        DIRECTEDGE = ('Y' << 1),
    }
}