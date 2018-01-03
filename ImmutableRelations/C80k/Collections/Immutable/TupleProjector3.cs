// The author licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace C80k.Collections.Immutable
{
    /// <summary>
    /// A <see cref="ITupleAdapter{TupleType}"/> implementation for triples.
    /// </summary>
    /// <typeparam name="T1">Type of first dimension</typeparam>
    /// <typeparam name="T2">Type of second dimension</typeparam>
    /// <typeparam name="T3">Type of third dimension</typeparam>
    public class TupleProjector<T1, T2, T3> : ITupleAdapter<(T1, T2, T3)>
    {
        readonly IEqualityComparer<T1> _ec1;
        readonly IEqualityComparer<T2> _ec2;
        readonly IEqualityComparer<T3> _ec3;

        /// <summary>
        /// Constructs an instance.
        /// </summary>
        /// <param name="ec1">Equality comparer to use for first dimension</param>
        /// <param name="ec2">Equality comparer to use for second dimension</param>
        /// <param name="ec3">Equality comparer to use for third dimension</param>
        public TupleProjector(
            IEqualityComparer<T1> ec1,
            IEqualityComparer<T2> ec2,
            IEqualityComparer<T3> ec3)
        {
            _ec1 = ec1 ?? EqualityComparer<T1>.Default;
            _ec2 = ec2 ?? EqualityComparer<T2>.Default;
            _ec3 = ec3 ?? EqualityComparer<T3>.Default;
        }

        /// <summary>
        /// Always 3.
        /// </summary>
        public int Rank => 3;

        /// <summary>
        /// Compares two items of tuples.
        /// </summary>
        /// <param name="x">First tuple</param>
        /// <param name="y">Second tuple</param>
        /// <param name="dim">0-based item index to compare</param>
        /// <returns>true iff the compared items are equal</returns>
        public bool ItemEquals((T1, T2, T3) x, (T1, T2, T3) y, int dim)
        {
            switch (dim)
            {
                case 0: return _ec1.Equals(x.Item1, y.Item1);
                case 1: return _ec2.Equals(x.Item2, y.Item2);
                case 2: return _ec3.Equals(x.Item3, y.Item3);
                default: throw new ArgumentOutOfRangeException(nameof(dim));
            }
        }

        /// <summary>
        /// Determines whether the specified tuples are equal.
        /// </summary>
        /// <param name="x">first tuple</param>
        /// <param name="y">second tuple</param>
        /// <returns>true iff the tuples are equal</returns>
        public bool Equals((T1, T2, T3) x, (T1, T2, T3) y)
        {
            return _ec1.Equals(x.Item1, y.Item1) &&
                   _ec2.Equals(x.Item2, y.Item2) &&
                   _ec3.Equals(x.Item3, y.Item3);
        }

        /// <summary>
        /// Computes the hash code of a particular tuple item.
        /// </summary>
        /// <param name="tuple">A tuple</param>
        /// <param name="dim">0-based index of the tuple's item</param>
        /// <returns>desired hash value</returns>
        public int GetItemHash((T1, T2, T3) tuple, int dim)
        {
            switch (dim)
            {
                case 0: return _ec1.GetHashCode(tuple.Item1);
                case 1: return _ec2.GetHashCode(tuple.Item2);
                case 2: return _ec3.GetHashCode(tuple.Item3);
                default: throw new ArgumentOutOfRangeException(nameof(dim));
            }
        }

        /// <summary>
        /// Computes a hash code for the given tuple.
        /// </summary>
        /// <param name="obj">tuple</param>
        /// <returns>hash value</returns>
        public int GetHashCode((T1, T2, T3) obj)
        {
            return (_ec1.GetHashCode(obj.Item1),
                    _ec2.GetHashCode(obj.Item2),
                    _ec3.GetHashCode(obj.Item3)).GetHashCode();
        }
    }
}
