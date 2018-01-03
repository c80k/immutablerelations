// The author licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace C80k.Collections.Immutable
{
    /// <summary>
    /// A <see cref="ITupleAdapter{TupleType}"/> implementation for unary tuples.
    /// </summary>
    /// <typeparam name="T">Item type</typeparam>
    public class TupleProjector<T> : ITupleAdapter<T>
    {
        readonly IEqualityComparer<T> _ec;

        /// <summary>
        /// Constructs an instance.
        /// </summary>
        /// <param name="ec">Equality comparer to use</param>
        public TupleProjector(IEqualityComparer<T> ec)
        {
            _ec = ec ?? EqualityComparer<T>.Default;
        }

        /// <summary>
        /// Always 1.
        /// </summary>
        public int Rank => 1;

        /// <summary>
        /// Determines whether the specified items are equal.
        /// </summary>
        /// <param name="x">first item</param>
        /// <param name="y">second item</param>
        /// <returns>true iff the items are equal</returns>
        public bool Equals(T x, T y)
        {
            return _ec.Equals(x, y);
        }

        /// <summary>
        /// Computes a hash code for the given item.
        /// </summary>
        /// <param name="obj">Item</param>
        /// <returns>hash value</returns>
        public int GetHashCode(T obj)
        {
            return _ec.GetHashCode(obj);
        }

        /// <summary>
        /// Computes the hash code of a particular tuple item.
        /// </summary>
        /// <param name="item">Item</param>
        /// <param name="dim">must be 0</param>
        /// <returns>hash value</returns>
        public int GetItemHash(T item, int dim)
        {
            if (dim != 0) throw new ArgumentOutOfRangeException(nameof(dim));
            return _ec.GetHashCode(item);
        }

        /// <summary>
        /// Compares two items of tuples.
        /// </summary>
        /// <param name="x">First tuple</param>
        /// <param name="y">Second tuple</param>
        /// <param name="dim">0-based item index to compare</param>
        /// <returns>true iff the compared items are equal</returns>
        public bool ItemEquals(T x, T y, int dim)
        {
            if (dim != 0) throw new ArgumentOutOfRangeException(nameof(dim));
            return _ec.Equals(x, y);
        }
    }
}
