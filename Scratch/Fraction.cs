using System;

namespace Scratch
{
    public struct Fraction
    {
        public readonly decimal Content;

        public Fraction(decimal content)
        {
            if (content < 0m || 1m < content)
            {
                throw new ArgumentException("Fraction must be in the range [0, 1]: " + content);
            }

            Content = content;
        }
    }

    public static class Fractions
    {
        public static Fraction Complement(this Fraction fraction)
        {
            return new Fraction(1m - fraction.Content);
        }

        public static IMonoid<Fraction> Interval
        {
            get
            {
                return new Monoid<Fraction>(
                    @null: new Fraction(1m),
                    joinF: (left, right) => new Fraction(left.Content * right.Content));
            }
        }

        public static Fraction Power(this Fraction fraction, uint exponent)
        {
            return Interval.Power(fraction, exponent);
        }
    }
}