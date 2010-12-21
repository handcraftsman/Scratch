//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************
namespace Scratch.Calculator
{
    /// <summary>
    /// http://simpleprogrammer.com/2010/12/12/back-to-basics-why-unit-testing-is-hard/
    /// </summary>
    public interface IStorageService
    {
        int[] GetHistorySession(int count);
        bool IsServiceOnline();
        void Store(int value);
    }
}