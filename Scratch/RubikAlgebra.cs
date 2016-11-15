using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scratch
{
    public enum Sign
    {
        Negative = -1,
        Zero = 0,
        Positive = +1,
    }

    public struct Arrow
    {
        public Sign X;
        public Sign Y;
        public Sign Z;

        public Arrow(Sign x, Sign y, Sign z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }

    public struct CubieFace
    {
        public Arrow Cubie;
        public Arrow Face;
    }

    public interface IRubik
    {
        Colors this[CubieFace face] { get; }
    }

    public sealed class RubikCube : IRubik
    {
        private readonly Func<CubieFace, Colors> GetF;

        public RubikCube(Func<CubieFace, Colors> getF)
        {
            this.GetF = getF;
        }

        public Colors this[CubieFace face]
        {
            get
            {
                return this.GetF(face);
            }
        }
    }

    public static class RubikAlgebra
    {
        public static Sign Negate(this Sign sign)
        {
            return (Sign)(-(int)sign);
        }
        
        public static Sign Multiply(this Sign left, Sign right)
        {
            return (Sign)((int)left * (int)right);
        }

        public static Sign Add(this Sign left, Sign right)
        {
            if (left.Multiply(right) == Sign.Positive)
            {
                throw new ArgumentException("Cannot add signs that are in alignment.");
            }

            return (Sign)((int)left + (int)right);
        }
        
        public static Arrow Negate(this Arrow arrow)
        {
            return new Arrow(arrow.X.Negate(), arrow.Y.Negate(), arrow.Z.Negate());
        }
        
        public static Arrow Scale(this Arrow arrow, Sign sign)
        {
            return new Arrow(arrow.X.Multiply(sign), arrow.Y.Multiply(sign), arrow.Z.Multiply(sign));
        }

        public static Arrow Add(this Arrow left, Arrow right)
        {
            return new Arrow(left.X.Add(right.X), left.Y.Add(right.Y), left.Z.Add(right.Z));
        }

        public static Arrow Cross(this Arrow left, Arrow right)
        {
            var x = Determinant(new Sign[2, 2] { 
              { left.Y, left.Z },
              { right.Y, right.Z },
            });

            var y = Determinant(new Sign[2, 2] { 
              { left.Z, left.X },
              { right.Z, right.X },
            });
            
            var z = Determinant(new Sign[2, 2] { 
              { left.X, left.Y },
              { right.X, right.Y },
            });

            return new Arrow((Sign) x, (Sign) y, (Sign) z);
        }

        public static int Determinant(Sign[,] matrix)
        {
            var diagonal = matrix[0, 0].Multiply(matrix[1, 1]);
            var antidiagonal = matrix[1, 0].Multiply(matrix[0, 1]);

            return (int)diagonal + (int)antidiagonal.Negate();
        }

        public static int Dot(this Arrow left, Arrow right)
        {
            return (int)left.X.Multiply(right.X) + (int)left.Y.Multiply(right.Y) + (int)left.Z.Multiply(right.Z);
        }

        public static Arrow Rotate(this Arrow arrow, Arrow axis)
        {
            axis.CheckUnit();

            var projection = axis.Scale((Sign)axis.Dot(arrow));

            return axis.Cross(arrow).Add(projection);
        }

        public static IEnumerable<Arrow> Inverse(Arrow axis)
        {
            yield return axis;
            yield return axis;
            yield return axis;
        }

        public static CubieFace Turn(this CubieFace arrows, Arrow axis)
        {
            if (arrows.Cubie.Dot(axis) > 0)
            {
                return new CubieFace
                { 
                    Cubie = arrows.Cubie.Rotate(axis),
                    Face = arrows.Face.Rotate(axis),
                };
            }

            return arrows;
        }

        public static IRubik Turn(this IRubik cube, Arrow axis)
        {
            return new RubikCube(
                getF: face =>
                {
                    return cube[face.Turn(axis)];
                });
        }

        public static CubieFace Neighbour(this Arrow cubie, Arrow face, ref Arrow direction)
        {
            Check(cubie, face, direction);

            if (cubie.Dot(direction) > 0)
            {
                var newFace = direction;
                direction = face.Negate();

                return new CubieFace { Cubie = cubie, Face = newFace };
            }

            cubie = cubie.Add(direction);

            return new CubieFace { Cubie = cubie, Face = face };
        }

        public static void Check(Arrow cubie, Arrow face)
        {
            cubie.CheckNonZero();

            face.CheckUnit();

            if (cubie.Dot(face) <= 0)
            {
                throw new ArgumentException("Face must point along the faces of the cube.");
            }
        }

        public static void Check(Arrow cubie, Arrow face, Arrow direction)
        {
            Check(cubie, face);

            direction.CheckUnit();

            if (face.Dot(direction) != 0)
            {
                throw new ArgumentException("Direction must be perpendicular to the face of the cube.");
            }
        }

        public static void CheckNonZero(this Arrow arrow)
        {
            if (arrow.Dot(arrow) == 0)
            {
                throw new ArgumentException("Arrow must be non-zero.");
            }
        }

        public static void CheckUnit(this Arrow arrow)
        {
            if (arrow.Dot(arrow) != 1)
            {
                throw new ArgumentException("Arrow must be a unit vector.");
            }
        }
    }
}
