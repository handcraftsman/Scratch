//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************
namespace Scratch.RangeEnumeration
{
    /// <summary>
    /// http://stackoverflow.com/questions/4271060/can-someone-come-up-with-a-better-version-of-this-enumerator
    /// </summary>
    public class Pair<T, T1>
    {
        public Pair(T first, T1 second)
        {
            First = first;
            Second = second;
        }

        public Pair()
        {
        }

        public T First { get; set; }
        public T1 Second { get; set; }
    }
}