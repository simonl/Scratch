using System;

namespace Scratch
{
    public interface IVectorSpace<in S, V>
    {
        V Zero { get; }
        V Add(V left, V right);
        V Scale(S scalar, V vector);
    }

    public sealed class VectorSpace<S, V> : IVectorSpace<S, V>
    {
        private readonly Func<V, V, V> AddF;
        private readonly Func<S, V, V> ScaleF;

        public VectorSpace(V zero, Func<V, V, V> addF, Func<S, V, V> scaleF)
        {
            Zero = zero;
            AddF = addF;
            ScaleF = scaleF;
        }

        public V Zero { get; private set; }

        public V Add(V left, V right)
        {
            return this.AddF(left, right);
        }

        public V Scale(S scalar, V vector)
        {
            return this.ScaleF(scalar, vector);
        }
    }

    public static class VectorSpaces
    {
        public static IVectorSpace<S, IPoint<V>> Geometry<S, V>(this IVectorSpace<S, V> space)
        {
            return new VectorSpace<S, IPoint<V>>(
                zero: new Point<V>(axis => space.Zero),
                addF: (left, right) =>
                {
                    return new Point<V>(axis => space.Add(left.On(axis), right.On(axis)));
                },
                scaleF: (scale, point) =>
                {
                    return new Point<V>(axis => space.Scale(scale, point.On(axis)));
                });
        }

        public static Func<Fraction, V> Interpolate<V>(this IVectorSpace<Fraction, V> space, V start, V end)
        {
            return scale =>
            {
                return space.Add(space.Scale(scale.Complement(), start), space.Scale(scale, end));
            };
        }

        public static IVectorSpace<Fraction, decimal> Interpolation
        {
            get
            {
                return new VectorSpace<Fraction, decimal>(
                    zero: 0m,
                    addF: (left, right) => left + right,
                    scaleF: (scale, point) => scale.Content * point);
            }
        }
        
        public static V Average<S, V>(this IVectorSpace<S, V> space, IArray<S> factors, IArray<V> vectors)
        {
            var summands = new Array<V>(
                size: factors.Size,
                atF: index => space.Scale(factors[index], vectors[index]));

            return space.Adding().Join(summands);
        }

        public static IMonoid<V> Adding<S, V>(this IVectorSpace<S, V> space)
        {
            return new Monoid<V>(space.Zero, space.Add);
        }
    }
}