using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scratch
{
    public struct CubieFace
    {
        public readonly Arrow Cubie;
        public readonly Arrow Face;

        public CubieFace(Arrow cubie, Arrow face)
        {
            this.Cubie = cubie;
            this.Face = face;

            this.Check();
        }

        public override string ToString()
        {
            return this.Cubie + ":" + this.Face;
        }
    }

    public struct FaceTurn
    {
        public readonly Arrow Axis;

        public FaceTurn(Arrow axis)
        {
            axis.CheckUnit();

            this.Axis = axis;
        }
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
        public static IEnumerable<Arrow> Inverse(Arrow axis)
        {
            yield return axis;
            yield return axis;
            yield return axis;
        }

        public static IAuto<CubieFace> Turn(this FaceTurn turn)
        {
            var rotation = turn.Axis.Rotate();

            return new Auto<CubieFace>(
                morphF: arrows =>
                {
                    if (arrows.Cubie.Dot(turn.Axis) > 0)
                    {
                        return new CubieFace(
                            cubie: rotation.Morph(arrows.Cubie),
                            face: rotation.Morph(arrows.Face));
                    }

                    return arrows;
                });
        }

        public static IAuto<IRubik> TurnCube(this FaceTurn turn)
        {
            return new Auto<IRubik>(
                morphF: cube =>
                {
                    return new RubikCube(
                        getF: face =>
                        {
                            return cube[turn.Turn().Morph(face)];
                        });
                });
        }
        
        public static IGraph<CubieFace, Arrow> Neighbourhood()
        {
            return new Graph<CubieFace, Arrow>(
                edgesF: node =>
                {
                    return new Constraint<Arrow>(
                        checkF: direction =>
                        {
                            node.Check(direction);
                        });
                },
                followF: (node, direction) =>
                {
                    return node.Neighbour(direction);
                });
        }

        public static CubieFace Neighbour(this CubieFace face, Arrow direction)
        {
            face.Check(direction);

            if (face.Cubie.Dot(direction) > 0)
            {
                return new CubieFace(face.Cubie, direction);
            }

            var cubie = face.Cubie.Add(direction);

            return new CubieFace(cubie, face.Face);
        }
        
        public static Arrow ReOrient(this CubieFace face, Arrow direction)
        {
            face.Check(direction);

            if (face.Cubie.Dot(direction) > 0)
            {
                return face.Face.Negate();
            }

            return direction;
        }

        public static Arrow ReOrient(this CubieFace face, Arrow direction, uint turn)
        {
            return face.Face.Rotate().Power(turn).Morph(direction);
        }

        public static CubieFace Follow(this CubieFace face, Arrow direction, params uint[] turns)
        {
            foreach (var turn in turns)
            {
                var neighbour = face.Neighbour(direction);
                var next = face.ReOrient(direction);
                var turned = neighbour.ReOrient(next, turn);

                face = neighbour;
                direction = turned;
            }

            return face;
        }

        public static CubieFace Loop(this CubieFace face, Arrow direction)
        {
            return face.Follow(direction, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        public static void Check(this CubieFace face)
        {
            face.Cubie.CheckNonZero();

            face.Face.CheckUnit();

            face.Cubie.CheckParallel(face.Face);
        }

        public static void Check(this CubieFace face, Arrow direction)
        {
            direction.CheckUnit();

            face.Face.CheckPerpendicular(direction);
        }
    }
}
