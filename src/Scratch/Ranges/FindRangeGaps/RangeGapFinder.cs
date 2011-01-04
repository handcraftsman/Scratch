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

namespace Scratch.Ranges.FindRangeGaps
{
    /// <summary>
    /// http://stackoverflow.com/questions/2698143/i-need-to-add-ranges-and-if-a-range-is-missed-while-adding-i-should-display-a-mes/2762570#2762570
    /// </summary>
    public class RangeGapFinder
    {
        public Range FindGap(Range range1, Range range2)
        {
            if (range1 == null)
            {
                throw new ArgumentNullException("range1", "range1 cannot be null");
            }

            if (range2 == null)
            {
                throw new ArgumentNullException("range2", "range2 cannot be null");
            }

            if (range2.Start < range1.Start)
            {
                return FindGap(range2, range1);
            }

            if (range1.End < range1.Start)
            {
                range1 = new Range(range1.End, range1.Start);
            }

            if (range2.End < range2.Start)
            {
                range2 = new Range(range2.End, range2.Start);
            }

            if (range1.End + 1 >= range2.Start)
            {
                return null; // no gap
            }

            return new Range(range1.End + 1, range2.Start - 1);
        }
    }
}