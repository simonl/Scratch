using System;

namespace Scratch
{
    public interface ISet<in T>
    {
        bool Contains(T element);
    }

    public sealed class Set<T> : ISet<T>
    {
        private readonly Func<T, bool> ContainsF;

        public Set(Func<T, bool> containsF)
        {
            ContainsF = containsF;
        }

        public bool Contains(T element)
        {
            return this.ContainsF(element);
        }
    }
}