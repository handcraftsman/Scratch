//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************
namespace Scratch.Ranges.FindRangeGaps
{
    /// <summary>
    /// http://stackoverflow.com/questions/2698143/i-need-to-add-ranges-and-if-a-range-is-missed-while-adding-i-should-display-a-mes/2762570#2762570
    /// </summary>
    public class Range
    {
        public int End;
        public int Start;

        public Range(int start, int end)
        {
            Start = start;
            End = end;
        }
    }
}