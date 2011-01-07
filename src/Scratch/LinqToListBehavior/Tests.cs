//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************

using System.Collections.Generic;
using System.Linq;

using FluentAssert;

using NUnit.Framework;

namespace Scratch.LinqToListBehavior
{
    /// <summary>
    /// http://stackoverflow.com/questions/4621561/is-there-a-way-to-memorize-or-materialize-an-ienumerable
    /// </summary>
    [TestFixture]
    public class Tests
    {
        private IEnumerable<int> _original;

        [SetUp]
        public void BeforeEachTest()
        {
            _original = Enumerable.Range(0, 10).ToList();
        }

        [Test]
        [ExpectedException]
        public void Materialize_does_not_have_same_behavior_as_ToList()
        {
            var newList = _original.Materialize();

            ChangesToNewListShouldNotAffectOriginal(newList);
        }

        [Test]
        public void ToList_should_make_a_copy()
        {
            var newList = _original.ToList();

            ChangesToNewListShouldNotAffectOriginal(newList);
        }

        /// <summary>
        /// delete even indexed items
        /// </summary>
        /// <param name="newList"></param>
        private void ChangesToNewListShouldNotAffectOriginal<T>(IList<T> newList)
        {
            int count = _original.Count();
            int half = count / 2;
            for (int i = 0; i < half; i++)
            {
                newList.RemoveAt(i);
            }

            newList.Count.ShouldBeEqualTo(half);
            _original.Count().ShouldBeEqualTo(count);
        }
    }

    public static class IEnumerableTExtensions
    {
        public static IList<TSource> Materialize<TSource>(this IEnumerable<TSource> source)
        {
            if (source is IList<TSource>)
            {
                // Already a list, use it as is
                return (IList<TSource>)source;
            }
            else
            {
                // Not a list, materialize it to a list
                return source.ToList();
            }
        }
    }
}