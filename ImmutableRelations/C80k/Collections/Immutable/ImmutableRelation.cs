// The author licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace C80k.Collections.Immutable
{
    /// <summary>
    /// An immutable n-ary relation.
    /// </summary>
    /// <typeparam name="TupleType">The type of tuples stored inside the relation. This
    /// is typicially - but not necessarily - a value tuple, such as (int, int)</typeparam>
    public sealed class ImmutableRelation<TupleType> : IEnumerable<TupleType>
    {
        struct Bucket
        {
            public ImmutableRelation<TupleType> Subrel;
            public bool ItemValid;
            public TupleType Item;

            public Bucket(TupleType item)
            {
                Item = item;
                Subrel = null;
                ItemValid = true;
            }

            public Bucket(ImmutableRelation<TupleType> subrel)
            {
                Item = default;
                Subrel = subrel;
                ItemValid = false;
            }

            public Bucket(TupleType item, ImmutableRelation<TupleType> subrel)
            {
                Item = item;
                Subrel = subrel;
                ItemValid = true;
            }

            public bool HasContent => ItemValid || Subrel != null;
        }

        const int BucketBits = 3;
        const int NumBuckets = 1 << BucketBits;

        Bucket _bucket0;
        Bucket _bucket1;
        Bucket _bucket2;
        Bucket _bucket3;
        Bucket _bucket4;
        Bucket _bucket5;
        Bucket _bucket6;
        Bucket _bucket7;
        object _liquidToken;
        readonly ITupleAdapter<TupleType> _proj;
        readonly byte _dim;
        readonly byte _level;
        int _count;

        /// <summary>
        /// Creates an empty relation.
        /// </summary>
        /// <param name="proj">The tuple adapter to use.</param>
        [Pure]
        public static ImmutableRelation<TupleType> Create(ITupleAdapter<TupleType> proj)
        {
            return new ImmutableRelation<TupleType>(proj ?? throw new ArgumentNullException(nameof(proj)));
        }

        ImmutableRelation(ITupleAdapter<TupleType> proj)
        {
            _proj = proj;
        }

        ImmutableRelation(
            ImmutableRelation<TupleType> other,
            byte dim, byte level,
            object liquidToken)
        {
            _bucket0 = other._bucket0;
            _bucket1 = other._bucket1;
            _bucket2 = other._bucket2;
            _bucket3 = other._bucket3;
            _bucket4 = other._bucket4;
            _bucket5 = other._bucket5;
            _bucket6 = other._bucket6;
            _bucket7 = other._bucket7;

            _proj = other._proj;
            _count = other._count;

            _dim = dim;
            _level = level;
            _liquidToken = liquidToken;
        }

        ImmutableRelation(ITupleAdapter<TupleType> proj, byte dim, byte level, object liquidToken)
        {
            _proj = proj;
            _dim = dim;
            _level = level;
            _liquidToken = liquidToken;
        }

        ImmutableRelation<TupleType> Fork()
        {
            byte dim = (byte)(_dim + 1);
            byte level = _level;

            if (dim == _proj.Rank)
            {
                dim = 0;
                ++level;
            }

            return new ImmutableRelation<TupleType>(_proj, dim, level, _liquidToken ?? this);
        }

        bool AllMatch(ItemMatcher<TupleType>[] qis, TupleType tuple)
        {
            if (qis == null) return true;

            for (int i = 0; i < qis.Length; i++)
            {
                if (!qis[i].Matches(tuple, i, _proj))
                {
                    return false;
                }
            }

            return true;
        }

        ref Bucket GetBucket(int hash)
        {
            int index = (hash >> (BucketBits * _level)) & (NumBuckets - 1);

            switch (index)
            {
                case 0: return ref _bucket0;
                case 1: return ref _bucket1;
                case 2: return ref _bucket2;
                case 3: return ref _bucket3;
                case 4: return ref _bucket4;
                case 5: return ref _bucket5;
                case 6: return ref _bucket6;
                default: return ref _bucket7;
            }
        }

        void Expand(
            ref List<TupleType> list,
            ref Stack<ImmutableRelation<TupleType>> stack,
            params ItemMatcher<TupleType>[] qis)
        {
            void LocalExpand(
                ref List<TupleType> llist,
                ref Stack<ImmutableRelation<TupleType>> lstack,
                ref Bucket bucket)
            {
                if (bucket.ItemValid && AllMatch(qis, bucket.Item))
                {
                    llist = llist ?? new List<TupleType>();
                    llist.Add(bucket.Item);
                }

                if (bucket.Subrel != null)
                {
                    lstack = lstack ?? new Stack<ImmutableRelation<TupleType>>();
                    lstack.Push(bucket.Subrel);
                }
            }

            if (qis?[_dim].MatchesEverything != false)
            {
                LocalExpand(ref list, ref stack, ref _bucket0);
                LocalExpand(ref list, ref stack, ref _bucket1);
                LocalExpand(ref list, ref stack, ref _bucket2);
                LocalExpand(ref list, ref stack, ref _bucket3);
                LocalExpand(ref list, ref stack, ref _bucket4);
                LocalExpand(ref list, ref stack, ref _bucket5);
                LocalExpand(ref list, ref stack, ref _bucket6);
                LocalExpand(ref list, ref stack, ref _bucket7);
            }
            else
            {
                ref var bucket = ref GetBucket(qis[_dim].GetKeyHash(_dim, _proj));

                LocalExpand(ref list, ref stack, ref bucket);
            }
        }

        ImmutableRelation<TupleType> Clone(object liquidToken)
        {
            return new ImmutableRelation<TupleType>(this, _dim, _level, liquidToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ImmutableRelation<TupleType> GetMutable()
        {
            return _liquidToken == null ? Clone(null) : this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ImmutableRelation<TupleType> AboutToModify(ImmutableRelation<TupleType> sub)
        {
            if (_liquidToken == null)
            {
                sub.Freeze();
            }
            else
            {
                sub = sub.Unfreeze(_liquidToken);
            }

            return sub;
        }

        ImmutableRelation<TupleType> Unfreeze(object liquidToken)
        {
            if (_liquidToken != null && _liquidToken == liquidToken)
            {
                return this;
            }
            else
            {
                return Clone(liquidToken);
            }
        }

        void Freeze()
        {
            _liquidToken = null;
        }

        ImmutableRelation<TupleType> Add(int hash, TupleType tuple, int nhash)
        {
            if (hash == 0 || _dim >= 2)
            {
                hash = _proj.GetItemHash(tuple, _dim);
            }

            var result = GetMutable();
            ref var tbucket = ref result.GetBucket(hash);

            if (tbucket.ItemValid)
            {
                if (_proj.Equals(tuple, tbucket.Item))
                {
                    return this;
                }
                else
                {
                    if (_dim >= 2)
                    {
                        hash = nhash = 0;
                    }

                    if (tbucket.Subrel == null)
                    {
                        tbucket.Subrel = Fork().Add(nhash, tuple, hash);

                        if (_liquidToken == null)
                        {
                            tbucket.Subrel.Freeze();
                        }

                        ++result._count;

                        return result;
                    }
                    else
                    {
                        int oldCount = tbucket.Subrel.Count;

                        var sub = AboutToModify(tbucket.Subrel).Add(nhash, tuple, hash);

                        if (sub.Count > oldCount)
                        {
                            tbucket.Subrel = sub;
                            ++result._count;

                            return result;
                        }
                        else
                        {
                            return this;
                        }
                    }
                }
            }
            else
            {
                tbucket.Item = tuple;
                tbucket.ItemValid = true;
                ++result._count;

                return result;
            }
        }

        ImmutableRelation<TupleType> Remove(int hash, TupleType tuple, int nhash)
        {
            if (hash == 0 || _dim >= 2)
            {
                hash = _proj.GetItemHash(tuple, _dim);
            }

            ref var bucket = ref GetBucket(hash);

            if (bucket.ItemValid)
            {
                var item = bucket.Item;
                var sub = bucket.Subrel;

                if (_proj.Equals(tuple, item))
                {
                    if (sub == null)
                    {
                        var result = GetMutable();
                        ref var tbucket = ref result.GetBucket(hash);
                        tbucket = default;
                        --result._count;
                        return result;
                    }
                    else
                    {
                        item = sub[0];

                        sub = AboutToModify(sub).Remove(item);
                    }
                }
                else if (sub == null)
                {
                    return this;
                }
                else
                {
                    int oldCount = sub.Count;

                    if (_dim >= 2)
                    {
                        hash = nhash = 0;
                    }

                    sub = AboutToModify(sub).Remove(nhash, tuple, hash);

                    if (oldCount == sub.Count)
                    {
                        return this;
                    }
                }

                {
                    var result = GetMutable();
                    --result._count;
                    ref var tbucket = ref result.GetBucket(hash);
                    tbucket.Item = item;
                    tbucket.Subrel = sub.IsEmpty ? null : sub;
                    return result;
                }

            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// Finds all matching tuples.
        /// </summary>
        /// <param name="qis">An <see cref="ItemMatcher{TupleType}"/> instance for each dimension</param>
        /// <returns>Sequence of matching tuples</returns>
        internal IEnumerable<TupleType> Find(params ItemMatcher<TupleType>[] qis)
        {
            var s = default(Stack<ImmutableRelation<TupleType>>);
            var l = default(List<TupleType>);

            Expand(ref l, ref s, qis);

            if (l != null)
            {
                foreach (var item in l)
                {
                    yield return item;
                }

                l.Clear();
            }

            if (s != null)
            {
                while (s.Count > 0)
                {
                    var rel = s.Pop();

                    rel.Expand(ref l, ref s, qis);

                    if (l != null)
                    {
                        foreach (var item in l)
                        {
                            yield return item;
                        }

                        l.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// Removes all matching tuples.
        /// </summary>
        /// <param name="qis">An <see cref="ItemMatcher{TupleType}"/> instance for each dimension</param>
        /// <returns>Updated relation</returns>
        public ImmutableRelation<TupleType> Remove(params ItemMatcher<TupleType>[] qis)
        {
            void RemoveAtBucket(ref Bucket tbucket, ImmutableRelation<TupleType> mutable)
            {
                var sub = tbucket.Subrel;

                if (tbucket.ItemValid)
                {
                    if (AllMatch(qis, tbucket.Item))
                    {
                        if (sub == null)
                        {
                            tbucket = default;
                        }
                        else
                        {
                            int oldCount = sub.Count;

                            sub = AboutToModify(sub).Remove(qis);

                            mutable._count += (sub.Count - oldCount);

                            if (sub.IsEmpty)
                            {
                                tbucket = default;
                            }
                            else
                            {
                                var first = sub[0];

                                sub = sub.Remove(first);

                                tbucket.Item = first;
                                tbucket.Subrel = sub.IsEmpty ? null : sub;
                            }
                        }

                        --mutable._count;
                    }
                    else if (sub != null)
                    {
                        int oldCount = sub.Count;

                        sub = AboutToModify(sub).Remove(qis);

                        if (sub.Count != oldCount)
                        {
                            tbucket.Subrel = sub.IsEmpty ? null : sub;

                            mutable._count += (sub.Count - oldCount);
                        }
                    }
                }
            }

            var qi = qis[_dim];

            var result = GetMutable();

            if (qi.MatchesEverything)
            {
                RemoveAtBucket(ref result._bucket0, result);
                RemoveAtBucket(ref result._bucket1, result);
                RemoveAtBucket(ref result._bucket2, result);
                RemoveAtBucket(ref result._bucket3, result);
                RemoveAtBucket(ref result._bucket4, result);
                RemoveAtBucket(ref result._bucket5, result);
                RemoveAtBucket(ref result._bucket6, result);
                RemoveAtBucket(ref result._bucket7, result);
            }
            else
            {
                RemoveAtBucket(ref result.GetBucket(qi.GetKeyHash(_dim, _proj)), result);
            }

            if (IsFrozen)
            {
                result.Freeze();
            }

            return result;
        }

        /// <summary>
        /// Call this method when you intend to perform a sequence of many modifications on the data 
        /// structure without requiring immutability. 
        /// Doing so will result in a substantial performance gain.
        /// </summary>
        /// <param name="op">User-supplied function which performs the intended modifications.
        /// Note that inside that callback, the data structure temporarily loses its immutability!
        /// </param>
        /// <returns>updated relation</returns>
        public ImmutableRelation<TupleType> Bulk(
            Func<ImmutableRelation<TupleType>, ImmutableRelation<TupleType>> op)
        {
            var result = op(Unfreeze(this));
            result.Freeze();
            return result;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.
        /// </returns>
        [Pure]
        public IEnumerator<TupleType> GetEnumerator()
        {
            return Find(default).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.
        /// </returns>
        [Pure]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool IsFrozen => _liquidToken == null;

        /// <summary>
        /// Returns true iff the relation does not contain any data.
        /// </summary>
        public bool IsEmpty => _count == 0;

        /// <summary>
        /// Returns the amount of tuples stored inside the structure.
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// Returns the tuple at the specified index. There is no particular ordering
        /// guaranteed by the data structure. However, the returned tuple will alway be the
        /// same for the same instance/index.
        /// </summary>
        /// <param name="index">0-based index of the desiered tuple</param>
        public TupleType this[int index]
        {
            get
            {
                int localcount = 0;
                TupleType item = default;

                bool LocalCount(ref Bucket bucket)
                {
                    if (bucket.ItemValid)
                    {
                        if (localcount == index)
                        {
                            item = bucket.Item;
                            return true;
                        }
                        else
                        {
                            ++localcount;
                        }
                    }

                    if (bucket.Subrel != null)
                    {
                        if (index < localcount + bucket.Subrel.Count)
                        {
                            item = bucket.Subrel[index - localcount];
                            return true;
                        }
                        else
                        {
                            localcount += bucket.Subrel.Count;
                        }
                    }

                    return false;
                }

                if (LocalCount(ref _bucket0)) return item;
                if (LocalCount(ref _bucket1)) return item;
                if (LocalCount(ref _bucket2)) return item;
                if (LocalCount(ref _bucket3)) return item;
                if (LocalCount(ref _bucket4)) return item;
                if (LocalCount(ref _bucket5)) return item;
                if (LocalCount(ref _bucket6)) return item;
                if (LocalCount(ref _bucket7)) return item;

                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        /// <summary>
        /// Adds a tuple to the relation.
        /// </summary>
        /// <param name="tuple">Tuple to add</param>
        /// <returns>Updated relation</returns>
        public ImmutableRelation<TupleType> Add(TupleType tuple)
        {
            return Add(0, tuple, 0);
        }

        /// <summary>
        /// Removes a tuple from the relation.
        /// </summary>
        /// <param name="tuple">Tuple to be removed</param>
        /// <returns>Updated relation</returns>
        public ImmutableRelation<TupleType> Remove(TupleType tuple)
        {
            return Remove(0, tuple, 0);
        }
    }
}
