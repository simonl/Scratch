using System;
using System.Collections.Generic;
using System.Linq;

namespace Scratch
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            //foreach (var matrix in Matrices())
            //{
            //    Console.WriteLine(string.Format("{0}", matrix.Determinant()));
            //}
            
            //Console.ReadLine();
            
            foreach (var cubie in Arrows())
            foreach (var face in Basis().Where(_ => cubie.Dot(_) == 1))
            foreach (var direction in Basis().Where(_ => face.Dot(_) == 0))
            {
                CubieFace result = new CubieFace(cubie, face);

                Console.WriteLine(string.Format("{0} -> {1} = {2}", result, direction, result.Loop(direction)));
            }
            
            Console.ReadLine();

            foreach (var first in Arrows())
            foreach (var second in Basis())
            {
                var cross = second.Rotate().Morph(first);

                Console.WriteLine(string.Format("{0} x {1} = {2}", first, second, cross));

                if (first.Magnitude() * second.Magnitude() == cross.Magnitude())
                {
                    Console.WriteLine(string.Format("{0} * {1} = {2} Ok", first.Magnitude(), second.Magnitude(), cross.Magnitude()));
                }
                else
                {
                    Console.WriteLine(string.Format("{0} * {1} = {2}", first.Magnitude(), second.Magnitude(), cross.Magnitude()));
                }
            }
            
            Console.ReadLine();
            
            foreach (var face in Basis())
            foreach (var cubie in Arrows())
            {
                var rotated = face.Rotate().Power(4).Morph(cubie);

                Console.WriteLine(string.Format("{0} ~> {1} = {2}", cubie, face, rotated));
            }
            
            Console.ReadLine();
            
            uint count = 0;
            foreach (var cubie in Arrows())
            foreach (var face in Basis().Where(_ => cubie.Dot(_) == 1))
            {
                Console.WriteLine(string.Format("{0} : {1}", cubie, face));
                count++;
            }

            Console.WriteLine("Faces: " + count);
            
            Console.ReadLine();

            foreach (var cubie in Arrows())
            foreach (var face in Basis().Where(_ => cubie.Dot(_) == 1))
            foreach (var direction in Basis().Where(_ => face.Dot(_) == 0))
            {
                CubieFace result = new CubieFace(cubie, face);

                FollowDirections(result, direction);
            }
            
            Console.ReadLine();

            foreach (var arrow in Arrows())
            {
                Console.Write(arrow);

                foreach (var direction in Basis().Where(_ => arrow.Dot(_) == 0))
                {
                    Console.WriteLine(" -> " + arrow);

                }

                Console.WriteLine(arrow);
            }

            Console.ReadLine();
        }

        public static CubieFace FollowDirections(CubieFace result, Arrow direction)
        {
            foreach (var _ in Enumerable.Repeat<object>(null, 12))
            {
                var next = result.Neighbour(direction);

                Console.WriteLine(string.Format("{0} : {1} -> {2} = {3} : {4}", result.Cubie, result.Face, direction, next.Cubie, next.Face));
                
                direction = result.ReOrient(direction);
                result = next;
            }
                
            Console.ReadLine();

            return result;
        }

        public static IEnumerable<Arrow> Basis()
        {
            return Arrows().Where(arrow => arrow.Dot(arrow) == 1);
        }

        public static IEnumerable<Arrow> Arrows()
        {
            foreach (Sign x in Enum.GetValues(typeof (Sign)))
            foreach (Sign y in Enum.GetValues(typeof (Sign)))
            foreach (Sign z in Enum.GetValues(typeof (Sign)))
            {
                var arrow = new Arrow(x, y, z);

                yield return arrow;
            }
        }
        
        public static IEnumerable<Sign[,]> Matrices()
        {
            foreach (Sign a in Enum.GetValues(typeof (Sign)))
            foreach (Sign b in Enum.GetValues(typeof (Sign)))
            foreach (Sign c in Enum.GetValues(typeof (Sign)))
            foreach (Sign d in Enum.GetValues(typeof (Sign)))
            {
                var matrix = new Sign[2, 2] { 
                    { a, b },
                    { c, d },
                };

                yield return matrix;
            }
        }

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
