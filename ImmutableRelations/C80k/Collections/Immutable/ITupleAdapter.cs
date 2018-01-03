// The author licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace C80k.Collections.Immutable
{
    /// <summary>
    /// Adapter interface for handling tuples.
    /// </summary>
    /// <typeparam name="TupleType">Type of tuple handled by this interface. This is
    /// typically - but not necessarily - a value tuple, such as (int, int)</typeparam>
    public interface ITupleAdapter<TupleType>: IEqualityComparer<TupleType>
    {
        /// <summary>
        /// Arity of the tuple type
        /// </summary>
        int Rank { get; }

        /// <summary>
        /// Compares two items of tuples.
        /// </summary>
        /// <param name="x">First tuple</param>
        /// <param name="y">Second tuple</param>
        /// <param name="dim">0-based item index to compare</param>
        /// <returns>true iff the compared items are equal</returns>
        bool ItemEquals(TupleType x, TupleType y, int dim);

        /// <summary>
        /// Computes the hash code of a particular tuple item.
        /// </summary>
        /// <param name="tuple">A tuple</param>
        /// <param name="dim">0-based index of the tuple's item</param>
        /// <returns>desired hash value</returns>
        int GetItemHash(TupleType tuple, int dim);
    }
}
