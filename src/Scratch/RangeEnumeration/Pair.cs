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