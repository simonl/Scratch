using System;

namespace Scratch
{
    public enum Axis
    {
        X, Y, Z,
    }

    public interface IPoint<out V>
    {
        V On(Axis axis);
    }

    public sealed class Point<V> : IPoint<V>
    {
        private readonly Func<Axis, V> OnF;

        public Point(Func<Axis, V> onF)
        {
            OnF = onF;
        }

        public V On(Axis axis)
        {
            return this.OnF(axis);
        }

        public override string ToString()
        {
            return this.On(Axis.X) + "," + this.On(Axis.Y) + "," + this.On(Axis.Z);
        }
    }

    public static class Points
    {
        public static IPoint<V> New<V>(V x, V y, V z)
        {
            return new Point<V>(
                onF: axis =>
                {
                    switch (axis)
                    {
                        case Axis.X: return x;
                        case Axis.Y: return y;
                        case Axis.Z: return z;
                        default:
                            throw new ArgumentOutOfRangeException("axis", axis, null);
                    }
                });
        }
    }
}