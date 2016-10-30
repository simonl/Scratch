using System;
using System.Collections.Generic;
using System.Linq;

namespace Scratch
{
    public static class Program
    {
        public static void Main1(string[] args)
        {
            var points = new IPoint<decimal>[]
            {
                Points.New(0m, 0m, 0m),
                Points.New(0m, 1m, 0m),
                Points.New(1m, 1m, 0m),
                Points.New(1m, 0m, 0m),
            };
            
            var curve = VectorSpaces.Interpolation.Geometry().Bezier1(points.Abstract());

            for (decimal scale = 0m; scale < 1.1m; scale += 0.1m)
            {
                var point = curve[new Fraction(scale)];

                Console.WriteLine(point);
            }
            
            Console.ReadLine();
        }

        public static void Main2(string[] args)
        {
            Console.WriteLine("> Get FizzBuzz");
            Console.ReadLine();

            foreach (var text in FizzBuzz.FizzBuzzes())
            {
                Console.WriteLine(text);
                Console.ReadLine();
            }
        }
    }
}
