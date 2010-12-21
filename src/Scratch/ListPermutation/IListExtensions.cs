using System;
using System.Collections.Generic;
using System.Linq;

namespace Scratch.ListPermutation
{
    /// <summary>
    /// http://handcraftsman.wordpress.com/2010/11/11/generating-all-permutations-of-a-sequence-in-csharp/
    /// </summary>
    public static class IListExtensions
    {
        public static IEnumerable<IList<T>> Permute<T>(this IList<T> list, int length)
        {
            if (list == null || list.Count == 0 || length <= 0)
            {
                yield break;
            }

            if (length > list.Count)
            {
                throw new ArgumentOutOfRangeException("length",
                                                      "length must be between 1 and the length of the list inclusive");
            }

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var initial = new[] { item };
                if (length == 1)
                {
                    yield return initial;
                }
                else
                {
                    foreach (var variation in Permute(list.Where((x, index) => index != i).ToList(), length - 1))
                    {
                        yield return initial.Concat(variation).ToList();
                    }
                }
            }
        }
    }
}