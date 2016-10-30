using System;

namespace Scratch
{
    public interface ICurve<out V>
    {
        V this[Fraction position] { get; }
    }

    public sealed class Curve<V> : ICurve<V>
    {
        private readonly Func<Fraction, V> AtF;

        public Curve(Func<Fraction, V> atF)
        {
            AtF = atF;
        }

        public V this[Fraction position]
        {
            get { return this.AtF(position); }
        }
    }
}