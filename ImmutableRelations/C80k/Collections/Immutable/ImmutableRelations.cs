// The author licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace C80k.Collections.Immutable
{
    /// <summary>
    /// A set of helper methods for dealing with immutable relations.
    /// </summary>
    public static class ImmutableRelations
    {
        #region Unary

        /// <summary>
        /// Creates an empty unary relation, using the default <see cref="EqualityComparer{T}"/>.
        /// This is a degenerated case, since it's principally the same as on ordinary hash set.
        /// </summary>
        /// <typeparam name="T">Type of elements to be stored in the relation</typeparam>
        public static ImmutableRelation<T> Create<T>()
        {
            return ImmutableRelation<T>.Create(new TupleProjector<T>(null));
        }

        /// <summary>
        /// Creates an empty unary relation, using the supplied <see cref="IEqualityComparer{T}"/>.
        /// This is a degenerated case, since it's principally the same as on ordinary hash set.
        /// </summary>
        /// <typeparam name="T">Type of elements to be stored in the relation</typeparam>
        public static ImmutableRelation<T> Create<T>(IEqualityComparer<T> ec)
        {
            return ImmutableRelation<T>.Create(new TupleProjector<T>(ec));
        }

        /// <summary>
        /// Searches for the specified element.
        /// </summary>
        /// <typeparam name="T">Type of elements stored in the relation</typeparam>
        /// <param name="rel">Unary relation</param>
        /// <param name="x">Element to look for</param>
        /// <returns>Sequence of occurences (either empty or containing exactly one element)</returns>
        public static IEnumerable<T> Find<T>(this ImmutableRelation<T> rel, T x)
            where T : class
        {
            return rel.Find(x == null ? ItemMatcher<T>.Any : new ItemMatcher<T>(x));
        }

        /// <summary>
        /// Searches for the specified element.
        /// </summary>
        /// <typeparam name="T">Type of elements stored in the relation</typeparam>
        /// <param name="rel">Unary relation</param>
        /// <param name="x">Element to look for</param>
        /// <returns>Sequence of occurences (either empty or containing exactly one element)</returns>
        public static IEnumerable<T> Find<T>(this ImmutableRelation<T> rel, T? x)
            where T : struct
        {
            return rel.Find(x == null ? ItemMatcher<T>.Any : new ItemMatcher<T>(x.Value));
        }

        /// <summary>
        /// Removes the specified element.
        /// </summary>
        /// <typeparam name="T">Type of elements stored in the relation</typeparam>
        /// <param name="rel">Unary relation</param>
        /// <param name="x">Element to remove</param>
        /// <returns>Updated relation</returns>
        public static ImmutableRelation<T> Remove<T>(this ImmutableRelation<T> rel, T x)
            where T : class
        {
            return rel.Remove(x == null ? ItemMatcher<T>.Any : new ItemMatcher<T>(x));
        }

        /// <summary>
        /// Removes the specified element.
        /// </summary>
        /// <typeparam name="T">Type of elements stored in the relation</typeparam>
        /// <param name="rel">Unary relation</param>
        /// <param name="x">Element to remove</param>
        /// <returns>Updated relation</returns>
        public static ImmutableRelation<T> Remove<T>(this ImmutableRelation<T> rel, T? x)
            where T : struct
        {
            return rel.Remove(x == null ? ItemMatcher<T>.Any : new ItemMatcher<T>(x.Value));
        }
        #endregion Unary

        #region Binary

        /// <summary>
        /// Creates an empty binary relation, using a default <see cref="EqualityComparer{T}"/>
        /// for each item type.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        public static ImmutableRelation<(T1, T2)> Create<T1, T2>()
        {
            return ImmutableRelation<(T1, T2)>.Create(
                new TupleProjector<T1, T2>(null, null));
        }

        /// <summary>
        /// Creates an empty binary relation, using the specified <see cref="EqualityComparer{T}"/>
        /// for each item type.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <param name="ec1">Equality comparer to use for first dimension</param>
        /// <param name="ec2">Equality comparer to use for second dimension</param>
        public static ImmutableRelation<(T1, T2)> Create<T1, T2>(
            IEqualityComparer<T1> ec1,
            IEqualityComparer<T2> ec2)
        {
            return ImmutableRelation<(T1, T2)>.Create(new TupleProjector<T1, T2>(ec1, ec2));
        }

        /// <summary>
        /// Finds all matching tuples inside the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <param name="rel">Binary relation</param>
        /// <param name="x">First-dimension object or null to match any object</param>
        /// <param name="y">Second-dimension object or null to match any object</param>
        /// <returns>Sequence of matching tuples</returns>
        public static IEnumerable<(T1, T2)> Find<T1, T2>(this ImmutableRelation<(T1, T2)> rel, T1 x, T2 y)
            where T1: class
            where T2: class
        {
            return rel.Find(
                x == null ? ItemMatcher<(T1, T2)>.Any : new ItemMatcher<(T1, T2)>((x, default(T2))),
                y == null ? ItemMatcher<(T1, T2)>.Any : new ItemMatcher<(T1, T2)>((default(T1), y)));
        }

        /// <summary>
        /// Finds all matching tuples inside the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <param name="rel">Binary relation</param>
        /// <param name="x">First-dimension value or null to match any value</param>
        /// <param name="y">Second-dimension value or null to match any value</param>
        /// <returns>Sequence of matching tuples</returns>
        public static IEnumerable<(T1, T2)> Find<T1, T2>(this ImmutableRelation<(T1, T2)> rel, T1? x, T2? y)
            where T1 : struct
            where T2 : struct
        {
            return rel.Find(
                x == null ? ItemMatcher<(T1, T2)>.Any : new ItemMatcher<(T1, T2)>((x.Value, default(T2))),
                y == null ? ItemMatcher<(T1, T2)>.Any : new ItemMatcher<(T1, T2)>((default(T1), y.Value)));
        }

        /// <summary>
        /// Finds all matching tuples inside the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <param name="rel">Binary relation</param>
        /// <param name="x">First-dimension object or null to match any object</param>
        /// <param name="y">Second-dimension value or null to match any value</param>
        /// <returns>Sequence of matching tuples</returns>
        public static IEnumerable<(T1, T2)> Find<T1, T2>(this ImmutableRelation<(T1, T2)> rel, T1 x, T2? y)
            where T1 : class
            where T2 : struct
        {
            return rel.Find(
                x == null ? ItemMatcher<(T1, T2)>.Any : new ItemMatcher<(T1, T2)>((x, default(T2))),
                y == null ? ItemMatcher<(T1, T2)>.Any : new ItemMatcher<(T1, T2)>((default(T1), y.Value)));
        }

        /// <summary>
        /// Finds all matching tuples inside the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <param name="rel">Binary relation</param>
        /// <param name="x">First-dimension value or null to match any value</param>
        /// <param name="y">Second-dimension object or null to match any object</param>
        /// <returns>Sequence of matching tuples</returns>
        public static IEnumerable<(T1, T2)> Find<T1, T2>(
            this ImmutableRelation<(T1, T2)> rel,
            T1? x, T2 y)
            where T1 : struct
            where T2 : class
        {
            return rel.Find(
                x == null ? ItemMatcher<(T1, T2)>.Any : new ItemMatcher<(T1, T2)>((x.Value, default(T2))),
                y == null ? ItemMatcher<(T1, T2)>.Any : new ItemMatcher<(T1, T2)>((default(T1), y)));
        }

        /// <summary>
        /// Removes all matching tuples from the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <param name="rel">Binary relation</param>
        /// <param name="x">First-dimension value or null to match any value</param>
        /// <param name="y">Second-dimension value or null to match any value</param>
        /// <returns>Updated relation</returns>
        public static ImmutableRelation<(T1, T2)> Remove<T1, T2>(this ImmutableRelation<(T1, T2)> rel, T1? x, T2? y)
            where T1 : struct
            where T2 : struct
        {
            return rel.Remove(
                x == null ? ItemMatcher<(T1, T2)>.Any : new ItemMatcher<(T1, T2)>((x.Value, default(T2))),
                y == null ? ItemMatcher<(T1, T2)>.Any : new ItemMatcher<(T1, T2)>((default(T1), y.Value)));
        }

        /// <summary>
        /// Removes all matching tuples from the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <param name="rel">Binary relation</param>
        /// <param name="x">First-dimension object or null to match any object</param>
        /// <param name="y">Second-dimension value or null to match any value</param>
        /// <returns>Updated relation</returns>
        public static ImmutableRelation<(T1, T2)> Remove<T1, T2>(this ImmutableRelation<(T1, T2)> rel, T1 x, T2? y)
            where T1 : class
            where T2 : struct
        {
            return rel.Remove(
                x == null ? ItemMatcher<(T1, T2)>.Any : new ItemMatcher<(T1, T2)>((x, default(T2))),
                y == null ? ItemMatcher<(T1, T2)>.Any : new ItemMatcher<(T1, T2)>((default(T1), y.Value)));
        }

        /// <summary>
        /// Removes all matching tuples from the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <param name="rel">Binary relation</param>
        /// <param name="x">First-dimension value or null to match any value</param>
        /// <param name="y">Second-dimension object or null to match any object</param>
        /// <returns>Updated relation</returns>
        public static ImmutableRelation<(T1, T2)> Remove<T1, T2>(this ImmutableRelation<(T1, T2)> rel, T1? x, T2 y)
            where T1 : struct
            where T2 : class
        {
            return rel.Remove(
                x == null ? ItemMatcher<(T1, T2)>.Any : new ItemMatcher<(T1, T2)>((x.Value, default(T2))),
                y == null ? ItemMatcher<(T1, T2)>.Any : new ItemMatcher<(T1, T2)>((default(T1), y)));
        }

        /// <summary>
        /// Removes all matching tuples from the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <param name="rel">Binary relation</param>
        /// <param name="x">First-dimension object or null to match any object</param>
        /// <param name="y">Second-dimension object or null to match any object</param>
        /// <returns>Updated relation</returns>
        public static ImmutableRelation<(T1, T2)> Remove<T1, T2>(this ImmutableRelation<(T1, T2)> rel, T1 x, T2 y)
            where T1 : class
            where T2 : class
        {
            return rel.Remove(
                x == null ? ItemMatcher<(T1, T2)>.Any : new ItemMatcher<(T1, T2)>((x, default(T2))),
                y == null ? ItemMatcher<(T1, T2)>.Any : new ItemMatcher<(T1, T2)>((default(T1), y)));
        }
        #endregion Binary

        #region Ternary

        /// <summary>
        /// Creates an empty ternary relation, using a default <see cref="EqualityComparer{T}"/>
        /// for each item type.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <typeparam name="T3">Type of third dimension</typeparam>
        public static ImmutableRelation<(T1, T2, T3)> Create<T1, T2, T3>()
        {
            return ImmutableRelation<(T1, T2, T3)>.Create(
                new TupleProjector<T1, T2, T3>(null, null, null));
        }

        /// <summary>
        /// Creates an empty binary relation, using the specified <see cref="EqualityComparer{T}"/>
        /// for each item type.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <typeparam name="T3">Type of third dimension</typeparam>
        /// <param name="ec1">Equality comparer to use for first dimension</param>
        /// <param name="ec2">Equality comparer to use for second dimension</param>
        /// <param name="ec3">Equality comparer to use for third dimension</param>
        public static ImmutableRelation<(T1, T2, T3)> Create<T1, T2, T3>(
            IEqualityComparer<T1> ec1,
            IEqualityComparer<T2> ec2,
            IEqualityComparer<T3> ec3)
        {
            return ImmutableRelation<(T1, T2, T3)>.Create(
                new TupleProjector<T1, T2, T3>(ec1, ec2, ec3));
        }

        /// <summary>
        /// Finds all matching tuples inside the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <typeparam name="T3">Type of third dimension</typeparam>
        /// <param name="rel">Ternary relation</param>
        /// <param name="x">First-dimension object or null to match any object</param>
        /// <param name="y">Second-dimension object or null to match any object</param>
        /// <param name="z">Third-dimension object or null to match any object</param>
        /// <returns>Sequence of matching tuples</returns>
        public static IEnumerable<(T1, T2, T3)> Find<T1, T2, T3>(
            this ImmutableRelation<(T1, T2, T3)> rel,
            T1 x, T2 y, T3 z)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return rel.Find(
                x == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((x, default(T2), default(T3))),
                y == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), y, default(T3))),
                z == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), default(T2), z)));
        }

        /// <summary>
        /// Finds all matching tuples inside the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <typeparam name="T3">Type of third dimension</typeparam>
        /// <param name="rel">Ternary relation</param>
        /// <param name="x">First-dimension object or null to match any object</param>
        /// <param name="y">Second-dimension object or null to match any object</param>
        /// <param name="z">Third-dimension value or null to match any value</param>
        /// <returns>Sequence of matching tuples</returns>
        public static IEnumerable<(T1, T2, T3)> Find<T1, T2, T3>(
            this ImmutableRelation<(T1, T2, T3)> rel,
            T1 x, T2 y, T3? z)
            where T1 : class
            where T2 : class
            where T3 : struct
        {
            return rel.Find(
                x == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((x, default(T2), default(T3))),
                y == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), y, default(T3))),
                z == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), default(T2), z.Value)));
        }

        /// <summary>
        /// Finds all matching tuples inside the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <typeparam name="T3">Type of third dimension</typeparam>
        /// <param name="rel">Ternary relation</param>
        /// <param name="x">First-dimension value or null to match any value</param>
        /// <param name="y">Second-dimension value or null to match any value</param>
        /// <param name="z">Third-dimension object or null to match any object</param>
        /// <returns>Sequence of matching tuples</returns>
        public static IEnumerable<(T1, T2, T3)> Find<T1, T2, T3>(
            this ImmutableRelation<(T1, T2, T3)> rel,
            T1? x, T2? y, T3 z)
            where T1 : struct
            where T2 : struct
            where T3 : class
        {
            return rel.Find(
                x == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((x.Value, default(T2), default(T3))),
                y == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), y.Value, default(T3))),
                z == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), default(T2), z)));
        }

        /// <summary>
        /// Finds all matching tuples inside the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <typeparam name="T3">Type of third dimension</typeparam>
        /// <param name="rel">Ternary relation</param>
        /// <param name="x">First-dimension value or null to match any value</param>
        /// <param name="y">Second-dimension value or null to match any value</param>
        /// <param name="z">Third-dimension value or null to match any value</param>
        /// <returns>Sequence of matching tuples</returns>
        public static IEnumerable<(T1, T2, T3)> Find<T1, T2, T3>(
            this ImmutableRelation<(T1, T2, T3)> rel,
            T1? x, T2? y, T3? z)
            where T1 : struct
            where T2 : struct
            where T3 : struct
        {
            return rel.Find(
                x == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((x.Value, default(T2), default(T3))),
                y == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), y.Value, default(T3))),
                z == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), default(T2), z.Value)));
        }

        /// <summary>
        /// Finds all matching tuples inside the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <typeparam name="T3">Type of third dimension</typeparam>
        /// <param name="rel">Ternary relation</param>
        /// <param name="x">First-dimension object or null to match any object</param>
        /// <param name="y">Second-dimension value or null to match any value</param>
        /// <param name="z">Third-dimension object or null to match any object</param>
        /// <returns>Sequence of matching tuples</returns>
        public static IEnumerable<(T1, T2, T3)> Find<T1, T2, T3>(
            this ImmutableRelation<(T1, T2, T3)> rel,
            T1 x, T2? y, T3 z)
            where T1 : class
            where T2 : struct
            where T3 : class
        {
            return rel.Find(
                x == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((x, default(T2), default(T3))),
                y == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), y.Value, default(T3))),
                z == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), default(T2), z)));
        }

        /// <summary>
        /// Finds all matching tuples inside the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <typeparam name="T3">Type of third dimension</typeparam>
        /// <param name="rel">Ternary relation</param>
        /// <param name="x">First-dimension object or null to match any object</param>
        /// <param name="y">Second-dimension value or null to match any value</param>
        /// <param name="z">Third-dimension value or null to match any value</param>
        /// <returns>Sequence of matching tuples</returns>
        public static IEnumerable<(T1, T2, T3)> Find<T1, T2, T3>(
            this ImmutableRelation<(T1, T2, T3)> rel,
            T1 x, T2? y, T3? z)
            where T1 : class
            where T2 : struct
            where T3 : struct
        {
            return rel.Find(
                x == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((x, default(T2), default(T3))),
                y == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), y.Value, default(T3))),
                z == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), default(T2), z.Value)));
        }

        /// <summary>
        /// Finds all matching tuples inside the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <typeparam name="T3">Type of third dimension</typeparam>
        /// <param name="rel">Ternary relation</param>
        /// <param name="x">First-dimension value or null to match any value</param>
        /// <param name="y">Second-dimension object or null to match any object</param>
        /// <param name="z">Third-dimension object or null to match any object</param>
        /// <returns>Sequence of matching tuples</returns>
        public static IEnumerable<(T1, T2, T3)> Find<T1, T2, T3>(
            this ImmutableRelation<(T1, T2, T3)> rel,
            T1? x, T2 y, T3 z)
            where T1 : struct
            where T2 : class
            where T3 : class
        {
            return rel.Find(
                x == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((x.Value, default(T2), default(T3))),
                y == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), y, default(T3))),
                z == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), default(T2), z)));
        }

        /// <summary>
        /// Finds all matching tuples inside the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <typeparam name="T3">Type of third dimension</typeparam>
        /// <param name="rel">Ternary relation</param>
        /// <param name="x">First-dimension value or null to match any value</param>
        /// <param name="y">Second-dimension object or null to match any object</param>
        /// <param name="z">Third-dimension value or null to match any value</param>
        /// <returns>Sequence of matching tuples</returns>
        public static IEnumerable<(T1, T2, T3)> Find<T1, T2, T3>(
            this ImmutableRelation<(T1, T2, T3)> rel,
            T1? x, T2 y, T3? z)
            where T1 : struct
            where T2 : class
            where T3 : struct
        {
            return rel.Find(
                x == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((x.Value, default(T2), default(T3))),
                y == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), y, default(T3))),
                z == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), default(T2), z.Value)));
        }

        /// <summary>
        /// Finds all matching tuples inside the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <typeparam name="T3">Type of third dimension</typeparam>
        /// <param name="rel">Ternary relation</param>
        /// <param name="x">First-dimension value or null to match any value</param>
        /// <param name="y">Second-dimension value or null to match any value</param>
        /// <param name="z">Third-dimension object or null to match any object</param>
        /// <returns>Sequence of matching tuples</returns>
        public static ImmutableRelation<(T1, T2, T3)> Remove<T1, T2, T3>(
            this ImmutableRelation<(T1, T2, T3)> rel,
            T1? x, T2? y, T3 z)
            where T1 : struct
            where T2 : struct
            where T3 : class
        {
            return rel.Remove(
                x == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((x.Value, default(T2), default(T3))),
                y == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), y.Value, default(T3))),
                z == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), default(T2), z)));
        }

        /// <summary>
        /// Removes all matching tuples from the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <typeparam name="T3">Type of third dimension</typeparam>
        /// <param name="rel">Ternary relation</param>
        /// <param name="x">First-dimension value or null to match any value</param>
        /// <param name="y">Second-dimension value or null to match any value</param>
        /// <param name="z">Third-dimension value or null to match any value</param>
        /// <returns>Updated relation</returns>
        public static ImmutableRelation<(T1, T2, T3)> Remove<T1, T2, T3>(
            this ImmutableRelation<(T1, T2, T3)> rel,
            T1? x, T2? y, T3? z)
            where T1 : struct
            where T2 : struct
            where T3 : struct
        {
            return rel.Remove(
                x == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((x.Value, default(T2), default(T3))),
                y == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), y.Value, default(T3))),
                z == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), default(T2), z.Value)));
        }

        /// <summary>
        /// Removes all matching tuples from the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <typeparam name="T3">Type of third dimension</typeparam>
        /// <param name="rel">Ternary relation</param>
        /// <param name="x">First-dimension object or null to match any object</param>
        /// <param name="y">Second-dimension value or null to match any value</param>
        /// <param name="z">Third-dimension object or null to match any object</param>
        /// <returns>Updated relation</returns>
        public static ImmutableRelation<(T1, T2, T3)> Remove<T1, T2, T3>(
            this ImmutableRelation<(T1, T2, T3)> rel,
            T1 x, T2? y, T3 z)
            where T1 : class
            where T2 : struct
            where T3 : class
        {
            return rel.Remove(
                x == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((x, default(T2), default(T3))),
                y == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), y.Value, default(T3))),
                z == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), default(T2), z)));
        }

        /// <summary>
        /// Removes all matching tuples from the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <typeparam name="T3">Type of third dimension</typeparam>
        /// <param name="rel">Ternary relation</param>
        /// <param name="x">First-dimension object or null to match any object</param>
        /// <param name="y">Second-dimension value or null to match any value</param>
        /// <param name="z">Third-dimension value or null to match any value</param>
        /// <returns>Updated relation</returns>
        public static ImmutableRelation<(T1, T2, T3)> Remove<T1, T2, T3>(
            this ImmutableRelation<(T1, T2, T3)> rel,
            T1 x, T2? y, T3? z)
            where T1 : class
            where T2 : struct
            where T3 : struct
        {
            return rel.Remove(
                x == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((x, default(T2), default(T3))),
                y == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), y.Value, default(T3))),
                z == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), default(T2), z.Value)));
        }

        /// <summary>
        /// Finds all matching tuples inside the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <typeparam name="T3">Type of third dimension</typeparam>
        /// <param name="rel">Ternary relation</param>
        /// <param name="x">First-dimension value or null to match any value</param>
        /// <param name="y">Second-dimension object or null to match any object</param>
        /// <param name="z">Third-dimension object or null to match any object</param>
        /// <returns>Sequence of matching tuples</returns>
        public static ImmutableRelation<(T1, T2, T3)> Remove<T1, T2, T3>(
            this ImmutableRelation<(T1, T2, T3)> rel,
            T1? x, T2 y, T3 z)
            where T1 : struct
            where T2 : class
            where T3 : class
        {
            return rel.Remove(
                x == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((x.Value, default(T2), default(T3))),
                y == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), y, default(T3))),
                z == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), default(T2), z)));
        }

        /// <summary>
        /// Finds all matching tuples inside the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <typeparam name="T3">Type of third dimension</typeparam>
        /// <param name="rel">Ternary relation</param>
        /// <param name="x">First-dimension value or null to match any value</param>
        /// <param name="y">Second-dimension object or null to match any object</param>
        /// <param name="z">Third-dimension value or null to match any value</param>
        /// <returns>Sequence of matching tuples</returns>
        public static ImmutableRelation<(T1, T2, T3)> Remove<T1, T2, T3>(
            this ImmutableRelation<(T1, T2, T3)> rel,
            T1? x, T2 y, T3? z)
            where T1 : struct
            where T2 : class
            where T3 : struct
        {
            return rel.Remove(
                x == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((x.Value, default(T2), default(T3))),
                y == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), y, default(T3))),
                z == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), default(T2), z.Value)));
        }

        /// <summary>
        /// Removes all matching tuples from the relation.
        /// </summary>
        /// <typeparam name="T1">Type of first dimension</typeparam>
        /// <typeparam name="T2">Type of second dimension</typeparam>
        /// <typeparam name="T3">Type of third dimension</typeparam>
        /// <param name="rel">Ternary relation</param>
        /// <param name="x">First-dimension object or null to match any object</param>
        /// <param name="y">Second-dimension object or null to match any object</param>
        /// <param name="z">Third-dimension object or null to match any object</param>
        /// <returns>Updated relation</returns>
        public static ImmutableRelation<(T1, T2, T3)> Remove<T1, T2, T3>(
            this ImmutableRelation<(T1, T2, T3)> rel,
            T1 x, T2 y, T3 z)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return rel.Remove(
                x == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((x, default(T2), default(T3))),
                y == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), y, default(T3))),
                z == null ? ItemMatcher<(T1, T2, T3)>.Any : new ItemMatcher<(T1, T2, T3)>((default(T1), default(T2), default(T3))));
        }

        #endregion Ternary
    }
}
