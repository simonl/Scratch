using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scratch
{
    public enum Colors 
    {
        Green,
        Blue,
        Red,
        Orange,
        White,
        Yellow,
    }

    public enum Columns
    {
        Left = -1,
        Center = 0,
        Right = +1,
    }

    public enum Rows 
    {
        Top = -1,
        Middle = 0,
        Bottom = +1,
    }

    public enum Orientation 
    {
        Back = -1,
        Front = +1,
    }

    public interface IPosition
    {
        Columns Column { get; }
        Rows Row { get; }
    }

    public sealed class Position : IPosition
    {
        public Columns Column { get; private set; }
        public Rows Row { get; private set; }

        public Position(Columns column, Rows row)
        {
            this.Column = column;
            this.Row = row;
        }
    }

    public interface IFace
    {
        Axis Axis { get; }
        Orientation Orientation { get; }
    }

    public sealed class Face : IFace
    {
        public Axis Axis { get; private set; }
        public Orientation Orientation { get; private set; }

        public Face(Axis axis, Orientation orientation)
        {
            this.Axis = axis;
            this.Orientation = orientation;
        }
    }

    public interface ICoordinate
    {
        IFace Face { get; }
        IPosition Position { get; }
    }

    public sealed class Coordinate : ICoordinate
    {
        public IFace Face { get; private set; }
        public IPosition Position { get; private set; }

        public Coordinate(IFace face, IPosition Position)
        {
            this.Face = face;
            this.Position = Position;
        }
    }

    public interface IRubiksCube
    {
        Colors this[ICoordinate coordinate] { get; }
    }

    public sealed class RubiksCube : IRubiksCube
    {
        private readonly Func<ICoordinate, Colors> GetF;

        public RubiksCube(Func<ICoordinate, Colors> getF)
        {
            this.GetF = getF;
        }

        public Colors this[ICoordinate view]
        {
            get { return this.GetF(view); }
        }
    }

    public enum Alignment
    {
        Columns,
        Rows,
    }

    public enum Delta
    {
        Decrease = -1,
        Increase = +1,
    }

    public interface IDirection
    {
        Alignment Alignment { get; }
        Delta Delta { get; }
    }

    public sealed class Direction : IDirection
    {
        public Alignment Alignment { get; private set; }
        public Delta Delta { get; private set; }

        public Direction(Alignment alignment, Delta delta)
        {
            this.Alignment = alignment;
            this.Delta = delta;
        }
    }

    public static class Rubik
    {
        public static Columns Complement(this Columns column)
        {
            return (Columns)(-(int)column);
        }
        
        public static Rows Complement(this Rows row)
        {
            return (Rows)(-(int)row);
        }
        
        public static Rows Orthogonal(this Columns column)
        {
            return (Rows)((int)column);
        }
        
        public static Columns Orthogonal(this Rows row)
        {
            return (Columns)((int)row);
        }

        public static int Quantity(this Delta delta)
        {
            return (int) delta;
        }

        public static Delta Opposite(this Delta delta)
        {
            return (Delta)(-(int)delta);
        }

        public static Orientation Opposite(this Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.Front:

                    return Orientation.Back;
                case Orientation.Back:

                    return Orientation.Front;
                default:
                    throw new ArgumentException();
            }
        }

        public static Alignment Orthogonal(this Alignment alignment)
        {
            switch (alignment)
            {
                case Alignment.Columns:

                    return Alignment.Rows;
                case Alignment.Rows:

                    return Alignment.Columns;
                default:
                    throw new ArgumentException();
            }
        }

        public static int Along(this IPosition position, Alignment alignment)
        {
            switch (alignment)
            {
                case Alignment.Columns:

                    return (int)position.Column;
                case Alignment.Rows:

                    return (int)position.Row;
                default:
                    throw new ArgumentException();
            }
        }

        public static IPosition Opposite(this IFace view, IPosition position)
        {
            switch (view.Axis)
            {
                case Axis.Z:
                case Axis.X:

                    return new Position(position.Column.Complement(), position.Row);
                case Axis.Y:
                    
                    return new Position(position.Column, position.Row.Complement());
                default:
                    throw new ArgumentException();
            }
        }
        
        public static IDirection Opposite(this IDirection direction)
        {
            return new Direction(direction.Alignment, direction.Delta.Opposite());
        }

        public static ICoordinate Travel(this ICoordinate coordinate, ref IDirection direction, uint count)
        {
            while (count != 0)
            {
                count--;

                coordinate = coordinate.Neighbour(ref direction);
            }

            return coordinate;
        }

        public static ICoordinate Neighbour(this ICoordinate coordinate, ref IDirection direction)
        {
            var position = coordinate.Position.Neighbour(direction);

            if (position != null)
            {
                return new Coordinate(coordinate.Face, position);
            }

            var index = coordinate.Position.Along(direction.Alignment.Orthogonal());

            position = coordinate.Face.Matching(direction, index);
            var face = coordinate.Face.Neighbour(direction);
            
            direction = coordinate.Face.Matching(direction).Opposite();

            return new Coordinate(face, position);
        }

        public static IPosition Matching(this IFace view, IDirection direction, int index)
        {
            return view.Matching(direction).Edge(view.Mapping(direction) * index);
        }

        public static IPosition Neighbour(this IPosition position, IDirection direction)
        {
            var index = position.Along(direction.Alignment) + direction.Delta.Quantity();

            if (index < -1 || 1 < index)
            {
                return null;
            }

            switch (direction.Alignment)
            {
                case Alignment.Columns:

                    return new Position((Columns) index, position.Row);
                case Alignment.Rows:

                    return new Position(position.Column, (Rows) index);
                default:
                    throw new ArgumentException();
            }
        }

        public static IDirection Diff(this IFace origin, IFace view)
        {
            foreach (var direction in Directions())
            {
                if (origin.Neighbour(direction).Equals(view))
                {
                    return direction;
                }
            }

            return null;
        }

        public static IFace Neighbour(this IFace view, IDirection direction)
        { 
            switch (view.Axis)
            {
                case Axis.Z:

                    switch (view.Orientation)
                    {
                        case Orientation.Front:
                            
                            switch (direction.Alignment)
                            {
                                case Alignment.Columns:

                                    switch (direction.Delta)
                                    {
                                        case Delta.Decrease:
                                                            
                                            return new Face(Axis.X, Orientation.Back);
                                        case Delta.Increase:
                                               
                                            return new Face(Axis.X, Orientation.Front);
                                        default:
                                            throw new ArgumentException();
                                    }

                                case Alignment.Rows:
                                                
                                    switch (direction.Delta)
                                    {
                                        case Delta.Decrease:
                                                            
                                            return new Face(Axis.Y, Orientation.Front);
                                        case Delta.Increase:
                                               
                                            return new Face(Axis.Y, Orientation.Back);
                                        default:
                                            throw new ArgumentException();
                                    }

                                default:
                                    throw new ArgumentException();
                            }

                        case Orientation.Back:
                            
                            switch (direction.Alignment)
                            {
                                case Alignment.Columns:

                                    switch (direction.Delta)
                                    {
                                        case Delta.Decrease:
                                                            
                                            return new Face(Axis.X, Orientation.Front);
                                        case Delta.Increase:
                                               
                                            return new Face(Axis.X, Orientation.Back);
                                        default:
                                            throw new ArgumentException();
                                    }

                                case Alignment.Rows:
                                                
                                    switch (direction.Delta)
                                    {
                                        case Delta.Decrease:
                                                            
                                            return new Face(Axis.Y, Orientation.Front);
                                        case Delta.Increase:
                                               
                                            return new Face(Axis.Y, Orientation.Back);
                                        default:
                                            throw new ArgumentException();
                                    }

                                default:
                                    throw new ArgumentException();
                            }

                        default:
                            throw new ArgumentException();
                    }

                case Axis.X:
                    
                    switch (view.Orientation)
                    {
                        case Orientation.Front:
                            
                            switch (direction.Alignment)
                            {
                                case Alignment.Columns:

                                    switch (direction.Delta)
                                    {
                                        case Delta.Decrease:
                                                            
                                            return new Face(Axis.Z, Orientation.Front);
                                        case Delta.Increase:
                                               
                                            return new Face(Axis.Z, Orientation.Back);
                                        default:
                                            throw new ArgumentException();
                                    }

                                case Alignment.Rows:
                                                
                                    switch (direction.Delta)
                                    {
                                        case Delta.Decrease:
                                                            
                                            return new Face(Axis.Y, Orientation.Front);
                                        case Delta.Increase:
                                               
                                            return new Face(Axis.Y, Orientation.Back);
                                        default:
                                            throw new ArgumentException();
                                    }

                                default:
                                    throw new ArgumentException();
                            }

                        case Orientation.Back:
                            
                            switch (direction.Alignment)
                            {
                                case Alignment.Columns:

                                    switch (direction.Delta)
                                    {
                                        case Delta.Decrease:
                                                                
                                            return new Face(Axis.Z, Orientation.Back);
                                        case Delta.Increase:
                                               
                                            return new Face(Axis.Z, Orientation.Front);
                                        default:
                                            throw new ArgumentException();
                                    }

                                case Alignment.Rows:
                                                
                                    switch (direction.Delta)
                                    {
                                        case Delta.Decrease:
                                                        
                                            return new Face(Axis.Y, Orientation.Front);
                                        case Delta.Increase:
                                               
                                            return new Face(Axis.Y, Orientation.Back);
                                        default:
                                            throw new ArgumentException();
                                    }

                                default:
                                    throw new ArgumentException();
                            }

                        default:
                            throw new ArgumentException();
                    }
                    
                case Axis.Y:

                    switch (view.Orientation)
                    {
                        case Orientation.Front:
                            
                            switch (direction.Alignment)
                            {
                                case Alignment.Columns:

                                    switch (direction.Delta)
                                    {
                                        case Delta.Decrease:
                                                            
                                            return new Face(Axis.X, Orientation.Back);
                                        case Delta.Increase:
                                               
                                            return new Face(Axis.X, Orientation.Front);
                                        default:
                                            throw new ArgumentException();
                                    }

                                case Alignment.Rows:
                                                
                                    switch (direction.Delta)
                                    {
                                        case Delta.Decrease:
                                                            
                                            return new Face(Axis.Z, Orientation.Back);
                                        case Delta.Increase:
                                               
                                            return new Face(Axis.Z, Orientation.Front);
                                        default:
                                            throw new ArgumentException();
                                    }

                                default:
                                    throw new ArgumentException();
                            }

                        case Orientation.Back:
                            
                            switch (direction.Alignment)
                            {
                                case Alignment.Columns:

                                    switch (direction.Delta)
                                    {
                                        case Delta.Decrease:
                                                            
                                            return new Face(Axis.X, Orientation.Back);
                                        case Delta.Increase:
                                               
                                            return new Face(Axis.X, Orientation.Front);
                                        default:
                                            throw new ArgumentException();
                                    }

                                case Alignment.Rows:
                                                
                                    switch (direction.Delta)
                                    {
                                        case Delta.Decrease:
                                                            
                                            return new Face(Axis.Z, Orientation.Front);
                                        case Delta.Increase:
                                               
                                            return new Face(Axis.Z, Orientation.Back);
                                        default:
                                            throw new ArgumentException();
                                    }

                                default:
                                    throw new ArgumentException();
                            }

                        default:
                            throw new ArgumentException();
                    }
                    
                default:
                    throw new ArgumentException();
            }
        }

        public static IPosition Edge(this IDirection direction, int index)
        {
            switch (direction.Alignment)
            {
                case Alignment.Columns:

                    switch (direction.Delta)
                    {
                        case Delta.Decrease:
                                            
                            return new Position(Columns.Left, (Rows) index);
                        case Delta.Increase:
                                               
                            return new Position(Columns.Right, (Rows) index);
                        default:
                            throw new ArgumentException();
                    }

                case Alignment.Rows:
                                                
                    switch (direction.Delta)
                    {
                        case Delta.Decrease:
                                                         
                            return new Position((Columns) index, Rows.Top);
                        case Delta.Increase:
                                               
                            return new Position((Columns) index, Rows.Bottom);
                        default:
                            throw new ArgumentException();
                    }

                default:
                    throw new ArgumentException();
            }
        }

        public static int Mapping(this IFace view, IDirection direction)
        { 
            switch (view.Axis)
            {
                case Axis.Z:
                    
                    if (view.Orientation == Orientation.Back && direction.Alignment == Alignment.Rows)
                    {
                        return -1;
                    }

                    return +1;
                case Axis.X:

                    if (view.Orientation == Orientation.Front && direction.Alignment == Alignment.Rows && direction.Delta == Delta.Decrease)
                    {
                        return -1;
                    }
                    
                    if (view.Orientation == Orientation.Back && direction.Alignment == Alignment.Rows && direction.Delta == Delta.Increase)
                    {
                        return -1;
                    }

                    return +1;
                case Axis.Y:

                    return (view.Orientation == Orientation.Front ? +1 : -1) * (direction.Alignment == Alignment.Rows ? +1 : -1) * direction.Delta.Quantity();
                default:
                    throw new ArgumentException();
            }
        }

        public static IDirection Matching(this IFace view, IDirection direction)
        { 
            switch (view.Axis)
            {
                case Axis.Z:
                    
                    return direction.Opposite();
                case Axis.X:

                    if (direction.Alignment == Alignment.Columns)
                    {
                        return direction.Opposite();
                    }
                  
                    switch (view.Orientation)
                    {
                        case Orientation.Front:
                            
                            return new Direction(Alignment.Columns, Delta.Increase);
                        case Orientation.Back:
                                     
                            return new Direction(Alignment.Columns, Delta.Decrease);
                        default:
                            throw new ArgumentException();
                    }
                    
                case Axis.Y:

                    switch (view.Orientation)
                    {
                        case Orientation.Front:
                            
                            return new Direction(Alignment.Rows, Delta.Decrease);
                        case Orientation.Back:
                            
                            return new Direction(Alignment.Rows, Delta.Increase);
                        default:
                            throw new ArgumentException();
                    }
                    
                default:
                    throw new ArgumentException();
            }
        }

        public static IRubiksCube Clockwise(IRubiksCube cube, IFace face)
        {
        return new RubiksCube(
            getF: coordinate =>
            {
                if (coordinate.Face.Equals(face))
                {
                    return cube[new Coordinate(coordinate.Face, Rotate(coordinate.Position))];
                }

                var direction = face.Diff(coordinate.Face);

                if (direction != null)
                {
                    // WTF???

                    var rotated = Rotate(direction);
                    var before = face.Neighbour(rotated);

                    for (int index = -1; index < 2; index++)
                    {
                        if (coordinate.Position.Equals(face.Matching(direction, index)))
                        {
                            var relative = coordinate.Face.Diff(before);

                            coordinate = coordinate.Travel(ref relative, count: 3);

                            return cube[coordinate];
                        }
                    }
                }

                return cube[coordinate];
            });
        }

        public static IEnumerable<IDirection> Directions()
        {
            yield return new Direction(Alignment.Columns, Delta.Decrease);
            yield return new Direction(Alignment.Columns, Delta.Increase);
            yield return new Direction(Alignment.Rows, Delta.Decrease);
            yield return new Direction(Alignment.Rows, Delta.Increase);
        }

        public static IDirection Rotate(IDirection direction)
        {
            return new Direction(
                alignment: direction.Alignment.Orthogonal(),
                delta: direction.Alignment == Alignment.Columns ? direction.Delta.Opposite() : direction.Delta);
        }
        
        public static IPosition Rotate(IPosition position)
        {
            return new Position(
                column: position.Row.Orthogonal(),
                row: position.Column.Orthogonal().Complement());
        }


        public static void Check(this IRubiksCube cube)
        {
            foreach (Colors color in Enum.GetValues(typeof(Colors)))
            {
                if (Count(cube, color) != 9)
                {
                    throw new ArgumentException();
                }
            }
        }

        public static uint Count(IRubiksCube cube, Colors color)
        {
            uint count = 0;

            foreach (Columns column in Enum.GetValues(typeof(Columns)))
            {
                foreach (Rows row in Enum.GetValues(typeof(Rows)))
                {
                    foreach (Axis axis in Enum.GetValues(typeof(Axis)))
                    {
                        foreach (Orientation orientation in Enum.GetValues(typeof(Orientation)))
                        {
                            if (cube[new Coordinate(new Face(axis, orientation), new Position(column, row))] == color)
                            {
                                count++;
                            }
                        }
                    }
                }
            }

            return count;
        }
    }
}
