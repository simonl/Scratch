using System;
using System.Collections.Generic;

namespace Scratch
{
    public static class Bezier
    {
        public static ICurve<V> Bezier1<V>(this IVectorSpace<Fraction, V> space, IArray<V> vectors)
        {
            return new Curve<V>(
                atF: scale =>
                {
                    if (vectors.Size == 0)
                    {
                        return space.Zero;
                    }

                    if (vectors.Size == 1)
                    {
                        return vectors[0];
                    }

                    IArray<V> segments = new Array<V>(
                        size: vectors.Size - 1U,
                        atF: index => space.Interpolate(vectors[index], vectors[index + 1])(scale));

                    segments = segments.Concrete().Abstract();

                    return space.Bezier1<V>(segments)[scale];
                });
        }

        public static ICurve<V> Bezier2<V>(this IVectorSpace<Fraction, V> space, IArray<V> vectors)
        {
            return new Curve<V>(
                atF: scale =>
                {
                    if (vectors.Size == 0)
                    {
                        return space.Zero;
                    }
            
                    if (vectors.Size == 1)
                    {
                        return vectors[0];
                    }

                    var left = space.Bezier2(vectors.Range(0, vectors.Size - 1))[scale];
                    var right = space.Bezier2(vectors.Range(1, vectors.Size))[scale];

                    return space.Interpolate(left, right)(scale);
                });
        }

        public static ICurve<V> Bezier3<V>(this IVectorSpace<Fraction, V> space, IArray<V> vectors)
        {
            return new Curve<V>(
                atF: scale =>
                {
                    var factors = scale.Binomial(vectors.Size);

                    return space.Average(factors, vectors);
                });
        }

        private static IArray<Fraction> Binomial(this Fraction fraction, uint size)
        {
            return new Array<Fraction>(
                size: size,
                atF: index =>
                {
                    var left = fraction.Complement().Power(index.Complement(size));
                    var right = fraction.Power(index);

                    var coefficient = size.Binomial()[index];

                    return new Fraction(Fractions.Interval.Join(left, right).Content*coefficient);
                });
        }

        private static IArray<uint> Binomial(this uint size)
        {
            return new Array<uint>(
                size: size,
                atF: index =>
                {
                    return size.Factorial()/(index.Factorial()*index.Complement(size).Factorial());
                });
        }

        private static uint Factorial(this uint number)
        {
            var product = 1U;

            while (number > 1)
            {
                product *= number;

                number--;
            }

            return product;
        }
    }
}