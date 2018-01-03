using C80k.Collections.Immutable;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NUnit.Tests
{
    [TestFixture]
    public class ImmutableRelationTest
    {
        static bool SetBasedEquals<T>(IEnumerable<T> items1, params T[] items2)
        {
            return items1.Count() == items2.Length && new HashSet<T>(items1).SetEquals(items2);
        }

        static bool SequenceEqual<T>(IEnumerable<T> items1, params T[] items2)
        {
            return items1.SequenceEqual(items2);
        }

        [Test]
        public void ImmutableIntRelationBasics()
        {
            var rel = ImmutableRelations.Create(
                EqualityComparer<int>.Default,
                EqualityComparer<int>.Default);

            rel = rel.Add((1, 2)).Add((1, 3)).Add((2, 3));

            var items = rel.Find(default(int?), default(int?));
            Assert.IsTrue(SetBasedEquals(items, (1, 2), (1, 3), (2, 3)));

            items = rel.Find((int?)1, default(int?));
            Assert.IsTrue(SetBasedEquals(items, (1, 2), (1, 3)));

            items = rel.Find(default(int?), (int?)3);
            Assert.IsTrue(SetBasedEquals(items, (1, 3), (2, 3)));

            items = rel.Find((int?)1, (int?)3);
            Assert.IsTrue(SetBasedEquals(items, (1, 3)));

            items = rel.Find((int?)1, (int?)4);
            Assert.IsFalse(items.Any());

            var rel2 = rel.Add((1, 2));
            Assert.AreEqual(rel2, rel);

            rel = rel.Add((17, 18));
            items = rel;
            Assert.IsTrue(SetBasedEquals(items, (1, 2), (1, 3), (2, 3), (17, 18)));

            rel = rel.Add((17, 18));
            items = rel;
            Assert.IsTrue(SetBasedEquals(items, (1, 2), (1, 3), (2, 3), (17, 18)));

            rel = rel.Add((273, 274));
            items = rel;
            Assert.IsTrue(SetBasedEquals(items, (1, 2), (1, 3), (2, 3), (17, 18), (273, 274)));

            rel2 = rel.Add((273, 274));
            Assert.AreEqual(rel2, rel);

            rel2 = rel.Remove((17, 18));
            Assert.AreNotEqual(rel2, rel);
            rel = rel2;
            Assert.IsTrue(SetBasedEquals(rel, (1, 2), (1, 3), (2, 3), (273, 274)));

            rel = rel.Remove((4, 4));
            Assert.IsTrue(SetBasedEquals(rel, (1, 2), (1, 3), (2, 3), (273, 274)));

            rel = rel.Remove((int?)1, default(int?));
            Assert.IsTrue(SetBasedEquals(rel, (2, 3), (273, 274)));

            rel = rel.Remove(default(int?), (int?)3);
            Assert.IsTrue(SetBasedEquals(rel, (273, 274)));

            rel = rel.Remove(default(int?), default(int?));
            Assert.IsFalse(rel.Any());
        }

        [Test]
        public void ImmutableIntRelationBulk()
        {
            var rel = ImmutableRelations.Create(
                EqualityComparer<int>.Default,
                EqualityComparer<int>.Default);

            rel = rel.Bulk(_ => _.Add((1, 2)).Add((1, 3)).Add((2, 3)).Add((17, 18)).Add((273, 274)));

            Assert.AreEqual(5, rel.Count);
            var items = rel.Find(default(int?), default(int?));
            Assert.IsTrue(SetBasedEquals(items, (1, 2), (1, 3), (2, 3), (17, 18), (273, 274)));

            var rel1 = rel.Remove((273, 274));
            Assert.AreEqual(4, rel1.Count);
            Assert.AreEqual(5, rel.Count);
            Assert.IsTrue(SetBasedEquals(rel, (1, 2), (1, 3), (2, 3), (17, 18), (273, 274)));
            Assert.IsTrue(SetBasedEquals(rel1, (1, 2), (1, 3), (2, 3), (17, 18)));

            rel1 = rel.Bulk(_ => _.Remove(default(int?), default(int?)));
            Assert.AreEqual(0, rel1.Count);
            Assert.AreEqual(5, rel.Count);
            Assert.IsTrue(rel1.IsEmpty);
            Assert.IsTrue(SetBasedEquals(rel, (1, 2), (1, 3), (2, 3), (17, 18), (273, 274)));

            rel1 = rel.Add((1, 1));
            Assert.AreEqual(6, rel1.Count);
            Assert.AreEqual(5, rel.Count);
            Assert.IsTrue(SetBasedEquals(rel, (1, 2), (1, 3), (2, 3), (17, 18), (273, 274)));
            Assert.IsTrue(SetBasedEquals(rel1, (1, 1), (1, 2), (1, 3), (2, 3), (17, 18), (273, 274)));
        }

        class Key
        {
            readonly int _value;

            public Key(int value)
            {
                _value = value;
            }

            public override bool Equals(object obj)
            {
                if (obj is Key x) return _value == x._value;
                return false;
            }

            public override int GetHashCode()
            {
                return _value % 256;
            }

            public override string ToString()
            {
                return _value.ToString();
            }
        }

        [Test]
        public void ImmutableObjectRelationBasics()
        {
            var rel = ImmutableRelations.Create(
                EqualityComparer<Key>.Default,
                EqualityComparer<Key>.Default);

            var o1 = new Key(1);
            var o2 = new Key(2);
            var o3 = new Key(3);
            var o17 = new Key(17);
            var o18 = new Key(18);
            var o257 = new Key(257);
            var o258 = new Key(258);
            var o513 = new Key(513);
            var o514 = new Key(514);

            rel = rel.Add((o1, o2)).Add((o1, o3)).Add((o2, o3));

            var items = rel.Find(default, default);
            Assert.IsTrue(SetBasedEquals(items, (o1, o2), (o1, o3), (o2, o3)));

            items = rel.Find(o1, null);
            Assert.IsTrue(SetBasedEquals(items, (o1, o2), (o1, o3)));

            items = rel.Find(null, o3);
            Assert.IsTrue(SetBasedEquals(items, (o1, o3), (o2, o3)));

            items = rel.Find(o1, o3);
            Assert.IsTrue(SetBasedEquals(items, (o1, o3)));

            items = rel.Find(o3, o1);
            Assert.IsFalse(items.Any());

            rel = rel.Add((o17, o18)).Add((o257, o258)).Add((o513, o514)).Add((o513, o514));
            items = rel.Find(default, default);
            Assert.IsTrue(SetBasedEquals(items, (o1, o2), (o1, o3), (o2, o3), (o17, o18), (o257, o258), (o513, o514)));

            rel = rel.Remove(o257, o258);
            items = rel;
            Assert.IsTrue(SetBasedEquals(items, (o1, o2), (o1, o3), (o2, o3), (o17, o18), (o513, o514)));

            items = rel.Find(o1, null);
            Assert.IsTrue(SetBasedEquals(items, (o1, o2), (o1, o3)));

            rel = rel.Remove(default, default);
            items = rel;
            Assert.IsFalse(items.Any());
        }

        static List<long> Profile(int[] counts, Action action)
        {
            var results = new List<long>(counts.Length);

            var watch = new Stopwatch();

            foreach (int count in counts)
            {
                watch.Start();

                for (int i = 0; i < count; i++)
                {
                    action();
                }

                watch.Stop();

                results.Add(watch.ElapsedMilliseconds);

                watch.Reset();
            }

            return results;
        }

        [Test]
        public void ImmutableRelationPerfComp()
        {
            var counts = new int[] { 100, 1000, 10000 };

            var rel = ImmutableRelations.Create(
                EqualityComparer<int>.Default,
                EqualityComparer<int>.Default);

            var uset = ImmutableHashSet.Create<(int, int)>();

            var rnd = new Random(0);

            var addTimesRel = Profile(counts, () =>
            {
                var item = (rnd.Next(), rnd.Next());
                rel = rel.Add(item);
            });

            rnd = new Random(0);

            var addTimesSet = Profile(counts, () =>
            {
                var item = (rnd.Next(), rnd.Next());
                uset = uset.Add(item);
            });

            rnd = new Random(0);

            var findTimesRel = Profile(counts, () =>
            {
                var item = (rnd.Next(), rnd.Next());
                bool exists = rel.Find((int?)item.Item1, (int?)item.Item2).Any();
                Assert.IsTrue(exists);
                exists = rel.Find((int?)(item.Item1 + 1), (int?)(item.Item2 + 1)).Any();
            });

            rnd = new Random(0);

            var findTimesSet = Profile(counts, () =>
            {
                var item = (rnd.Next(), rnd.Next());
                bool exists = uset.Contains(item);
                Assert.IsTrue(exists);
                exists = uset.Contains((item.Item1 + 1, item.Item2 + 1));
            });

            rnd = new Random(0);

            var findRowTimesRel = Profile(counts, () =>
            {
                var item = (rnd.Next(), rnd.Next());
                rel.Find((int?)item.Item1, default(int?)).ToArray();
            });

            rnd = new Random(0);

            var findRowTimesSet = Profile(counts, () =>
            {
                var item = (rnd.Next(), rnd.Next());
                uset.Where(tup => tup.Item1 == item.Item1).ToArray();
            });

            var tmprel = rel;
            var tmpuset = uset;

            rnd = new Random(0);

            var removeTimesRel = Profile(counts, () =>
            {
                var item = (rnd.Next(), rnd.Next());
                tmprel = tmprel.Remove(item);
            });

            rnd = new Random(0);

            var removeTimesSet = Profile(counts, () =>
            {
                var item = (rnd.Next(), rnd.Next());
                tmpuset = tmpuset.Remove(item);
            });

            tmprel = rel;
            tmpuset = uset;

            rnd = new Random(0);

            var removeColTimesRel = Profile(counts, () =>
            {
                var item = (rnd.Next(), rnd.Next());
                tmprel = tmprel.Remove(default(int?), (int?)item.Item2);
            });

            rnd = new Random(0);

            var removeColTimesSet = Profile(counts, () =>
            {
                var item = (rnd.Next(), rnd.Next());
                var b = tmpuset.ToBuilder();
                foreach (var tup in tmpuset.Where(_ => _.Item2 == item.Item2))
                {
                    b.Remove(tup);
                }
                tmpuset = b.ToImmutable();
            });

            string str = string.Join(" / ", counts);
            Console.WriteLine($"Durations in [ms] for {str} items");
            str = string.Join(" / ", addTimesRel);
            Console.WriteLine($"Add to relation: {str}");
            str = string.Join(" / ", addTimesSet);
            Console.WriteLine($"Add to hash set: {str}");
            str = string.Join(" / ", findTimesRel);
            Console.WriteLine($"Query single item of relation: {str}");
            str = string.Join(" / ", findTimesSet);
            Console.WriteLine($"Query single item of hash set: {str}");
            str = string.Join(" / ", findRowTimesRel);
            Console.WriteLine($"Query row of relation: {str}");
            str = string.Join(" / ", findRowTimesSet);
            Console.WriteLine($"Query row of hash set: {str}");
            str = string.Join(" / ", removeTimesRel);
            Console.WriteLine($"Remove item from relation: {str}");
            str = string.Join(" / ", removeTimesSet);
            Console.WriteLine($"Remove item from hash set: {str}");
            str = string.Join(" / ", removeColTimesRel);
            Console.WriteLine($"Remove column from relation: {str}");
            str = string.Join(" / ", removeColTimesSet);
            Console.WriteLine($"Remove column from hash set: {str}");
        }

        [Test]
        public void ImmutableRelationConcurrency()
        {
            var rel = ImmutableRelations.Create(
                EqualityComparer<int>.Default,
                EqualityComparer<int>.Default);

            var set = new HashSet<(int, int)>();

            rel = rel.Bulk(r =>
            {
                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        var x = (1 << i, 1 << j);

                        r = r.Add(x);
                        set.Add(x);
                    }
                }
                return r;
            });

            Assert.AreEqual(32 * 32, rel.Count);
            Assert.IsTrue(set.SetEquals(rel));

            var ids = ImmutableRelations.Create<int>();

            Parallel.For(1, 20, k =>
            {
                var hset = new HashSet<(int, int)>(set);

                var hrel = rel.Bulk(r =>
                {
                    for (int i = k; i < 1000; i++)
                    {
                        var x = (int.MaxValue - i, int.MinValue + i);

                        r = r.Add(x);
                        hset.Add(x);
                    }

                    return r;
                });

                Assert.AreEqual(32 * 32 + 1000 - k, hrel.Count);
                Assert.IsTrue(hset.SetEquals(hrel));

                hrel = hrel.Remove(default(int?), default(int?));

                Assert.IsTrue(hrel.IsEmpty);

                ImmutableInterlocked.Update(
                    ref ids,
                    idset => idset.Add(Thread.CurrentThread.ManagedThreadId));
            });

            Assert.IsTrue(ids.Count >= 2);
            Assert.AreEqual(32 * 32, rel.Count);
            Assert.IsTrue(set.SetEquals(rel));
        }

        [Test]
        public void TernaryImmutableRelation()
        {
            var rel = ImmutableRelations.Create(
                EqualityComparer<int>.Default,
                EqualityComparer<int>.Default,
                EqualityComparer<int>.Default);

            var set = new HashSet<(int, int, int)>();

            for (int i = 0; i < 1000; i++)
            {
                int z = i % 10;
                int y = (i / 10) % 10;
                int x = (i / 100) % 10;

                rel = rel.Add((x, y, z));
                set.Add((x, y, z));
            }

            Assert.AreEqual(1000, rel.Count);
            Assert.IsTrue(set.SetEquals(rel));

            Assert.AreEqual(1, rel.Find((int?)1, (int?)2, (int?)3).Count());
            Assert.AreEqual(10, rel.Find((int?)2, (int?)5, null).Count());
            Assert.AreEqual(10, rel.Find((int?)2, null, (int?)5).Count());
            Assert.AreEqual(10, rel.Find(null, (int?)0, (int?)3).Count());
            Assert.AreEqual(10, rel.Find((int?)8, (int?)4, null).Count());
            Assert.AreEqual(100, rel.Find((int?)8, null, null).Count());
            Assert.AreEqual(100, rel.Find(null, (int?)4, null).Count());
            Assert.AreEqual(100, rel.Find(null, null, (int?)7).Count());
            Assert.AreEqual(1000, rel.Find(null, null, null).Count());
            Assert.AreEqual(0, rel.Find((int?)-1, null, null).Count());
            Assert.AreEqual(0, rel.Find(null, (int?)int.MaxValue, null).Count());
            Assert.AreEqual(0, rel.Find(null, null, (int?)10).Count());

            var rel2 = rel.Remove((int?)9, null, null);

            Assert.AreEqual(900, rel2.Count);

            rel2 = rel.Remove(null, (int?)2, (int?)6);

            Assert.AreEqual(990, rel2.Count);

            rel = rel
                .Add((int.MaxValue, int.MaxValue, int.MaxValue))
                .Add((int.MinValue, int.MinValue, int.MinValue));

            Assert.AreEqual(1002, rel.Count);

            Assert.AreEqual(1, rel.Find(null, (int?)int.MaxValue, null).Count());
            Assert.AreEqual(1, rel.Find((int?)int.MinValue, null, (int?)int.MinValue).Count());
        }
    }
}
