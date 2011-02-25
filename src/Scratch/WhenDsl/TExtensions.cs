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

namespace Scratch.WhenDsl
{
	public static class TExtensions
	{
		public static T Unless<T>(this T item, Func<T, bool> condition, Action<T> doIfNotTrue)
		{
			if (!condition(item))
			{
				doIfNotTrue(item);
			}
			return item;
		}

		public static T When<T>(this T item, Func<T, bool> condition, Action<T> doIfTrue)
		{
			if (condition(item))
			{
				doIfTrue(item);
			}
			return item;
		}
	}
}