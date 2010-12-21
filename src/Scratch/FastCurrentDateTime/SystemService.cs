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

namespace Scratch.FastCurrentDateTime
{
    /// <summary>
    /// http://stackoverflow.com/questions/1561791/optimizing-alternatives-to-datetime-now/3095823#3095823
    /// </summary>
    public class SystemService : ISystemService
    {
        private static readonly TimeSpan LocalUtcOffset;

        static SystemService()
        {
            LocalUtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
        }

        public DateTime CurrentDateTime()
        {
            return DateTime.SpecifyKind(DateTime.UtcNow + LocalUtcOffset, DateTimeKind.Local);
        }
    }

    public interface ISystemService
    {
        DateTime CurrentDateTime();
    }
}