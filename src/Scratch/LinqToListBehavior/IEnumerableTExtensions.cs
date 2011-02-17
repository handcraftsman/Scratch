using System.Collections.Generic;
using System.Linq;

namespace Scratch.LinqToListBehavior
{
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