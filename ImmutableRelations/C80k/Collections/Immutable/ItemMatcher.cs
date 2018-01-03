// The author licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace C80k.Collections.Immutable
{
    /// <summary>
    /// A small wrapper class for matching tuples.
    /// </summary>
    /// <typeparam name="TupleType">Type of tuple to match</typeparam>
    public class ItemMatcher<TupleType>
    {
        readonly bool _any;
        readonly TupleType _item;

        /// <summary>
        /// Wildcard - matches any tuple.
        /// </summary>
        public static readonly ItemMatcher<TupleType> Any = new ItemMatcher<TupleType>(true);

        /// <summary>
        /// Constructs a new instance for matching the specified tuple.
        /// </summary>
        /// <param name="item">Tuple to match</param>
        public ItemMatcher(TupleType item)
        {
            _item = item;
        }

        ItemMatcher(bool any)
        {
            _any = true;
        }

        internal bool MatchesEverything => _any;

        internal int GetKeyHash(int dim, ITupleAdapter<TupleType> proj)
        {
            return proj.GetItemHash(_item, dim);
        }

        internal bool Matches(TupleType tuple, int dim, ITupleAdapter<TupleType> proj)
        {
            if (_any) return true;
            return proj.ItemEquals(tuple, _item, dim);
        }
    }
}
