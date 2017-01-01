using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scratch
{
    public interface IConstraint<in T>
    {
        void Check(T element);
    }

    public sealed class Constraint<T> : IConstraint<T>
    {
        private readonly Action<T> CheckF;

        public Constraint(Action<T> checkF)
        {
            this.CheckF = checkF;
        }

        public void Check(T element)
        {
            this.CheckF(element);
        }
    }

    public interface IGraph<N, in E>
    {
        IConstraint<E> Edges(N node);

        N Follow(N node, E edge);
    }

    public sealed class Graph<N, E> : IGraph<N, E>
    {
        private readonly Func<N, IConstraint<E>> EdgesF;
        private readonly Func<N, E, N> FollowF;

        public Graph(Func<N, IConstraint<E>> edgesF, Func<N, E, N> followF) 
        {
            this.EdgesF = edgesF;
            this.FollowF = followF;
        }

        public IConstraint<E> Edges(N node)
        {
            return this.EdgesF(node);
        }

        public N Follow(N node, E edge) 
        {
            return this.FollowF(node, edge);
        }
    }
}
