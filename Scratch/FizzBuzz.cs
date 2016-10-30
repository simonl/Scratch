using System;
using System.Collections.Generic;
using System.Linq;

namespace Scratch
{
    public enum Sounds
    {
        Fizz,
        Buzz,
    }

    public static class FizzBuzz
    {
        public static IEnumerable<string> FizzBuzzes()
        {
            foreach (var number in Numbers())
            {
                yield return number.Print();
            }
        }

        public static IEnumerable<uint> Numbers()
        {
            for (uint number = 0;; number++)
            {
                yield return number;
            }
        }

        public static string Print(this uint number)
        {
            return number.Noise().Print() ?? number.ToString();
        }

        public static string Print(this ISet<Sounds> sounds)
        {
            return Enum.GetValues(typeof(Sounds)).Cast<Sounds>()
                .Select(sounds.Print)
                .Fold(Concatenation().Nullable());
        }

        private static string Print(this ISet<Sounds> sounds, Sounds sound)
        {
            return sounds.Contains(sound) ? sound.ToString() : null;
        }

        public static IMonoid<string> Concatenation()
        {
            return new Monoid<string>(
                @null: "",
                joinF: (left, right) => left + right);
        }

        public static ISet<Sounds> Noise(this uint number)
        {
            return new Set<Sounds>(
                containsF: sound =>
                {
                    return sound.Factor().Multiples().Contains(number);
                });
        }

        public static uint Factor(this Sounds sound)
        {
            switch (sound)
            {
                case Sounds.Fizz:
                    return 3;
                case Sounds.Buzz:
                    return 5;
                default:
                    throw new ArgumentOutOfRangeException("sound", sound, null);
            }
        }

        public static ISet<uint> Multiples(this uint divisor)
        {
            return new Set<uint>(
                containsF: number =>
                {
                    return number%divisor == 0;
                });
        }
    }
}