// <copyright file="ValueImmutableList.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.Immutability
{
    /// <summary>
    /// Static class with static method to create value immutable lists.
    /// </summary>
    public static class ValueImmutableList
    {
        /// <summary>
        /// Create a value immutable list of items.
        /// </summary>
        /// <param name="items">The items in the immutable list.</param>
        /// <typeparam name="T">The type of item.</typeparam>
        /// <returns>An instance of <see cref="ValueImmutableList{T}"/>.</returns>
        public static ValueImmutableList<T> Create<T>(params T[] items) => new ValueImmutableList<T>(ImmutableList.Create(items));
    }

    /// <summary>
    /// A value immutable list which is compared by value instead of by reference to other lists.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
#pragma warning disable SA1402
    [JsonConverter(typeof(ValueImmutableListJsonConverter))]
    public sealed class ValueImmutableList<T> : IEnumerable<T>, IEquatable<ValueImmutableList<T>>
#pragma warning restore SA1402
    {
        private readonly ImmutableList<T> list;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueImmutableList{T}"/> class.
        /// </summary>
        /// <param name="list">An instance of <see cref="ImmutableList{T}"/>.</param>
        public ValueImmutableList(ImmutableList<T> list) => this.list = list;

        /// <inheritdoc cref="ImmutableList{T}"/>
        public bool IsEmpty => this.list.IsEmpty;

        /// <inheritdoc cref="ImmutableList{T}"/>
        public int Count => this.list.Count;

        /// <inheritdoc cref="ImmutableList{T}"/>
        public T this[int index] => this.list[index];

        /// <inheritdoc cref="IEnumerable{T}"/>
        public IEnumerator<T> GetEnumerator() => this.list.GetEnumerator();

        /// <inheritdoc cref="IEnumerable{T}"/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc cref="IEquatable{T}"/>
        public bool Equals(ValueImmutableList<T>? other) =>
            !ReferenceEquals(null, other)
            && (ReferenceEquals(this, other) || this.list.SequenceEqual(other.list));

        /// <inheritdoc cref="IEquatable{T}"/>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (!(obj is ValueImmutableList<T> objAsValueImmutableList))
            {
                return false;
            }

            return this.Equals(objAsValueImmutableList);
        }

        /// <inheritdoc cref="IEquatable{T}"/>
        public override int GetHashCode() => this.list.GetHashCode();

        /// <summary>
        /// Converts the value list to a value immutable hash set.
        /// </summary>
        /// <returns>An instance of <see cref="ValueImmutableHashSet{T}"/>.</returns>
        public ValueImmutableHashSet<T> ToValueImmutableHashSet() => new ValueImmutableHashSet<T>(this.list.ToImmutableHashSet());

        /// <summary>
        /// Returns the reference-based immutable list which has reference-based equality checks.
        /// </summary>
        /// <returns>An instance of <see cref="ImmutableList{T}"/>.</returns>
        public ImmutableList<T> AsImmutableList() => this.list;

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> Clear() => new ValueImmutableList<T>(this.list.Clear());

        /// <inheritdoc cref="ImmutableList{T}"/>
        public int BinarySearch(T item) => this.list.BinarySearch(item);

        /// <inheritdoc cref="ImmutableList{T}"/>
        public int BinarySearch(T item, IComparer<T> comparer) => this.list.BinarySearch(item, comparer);

        /// <inheritdoc cref="ImmutableList{T}"/>
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer) => this.list.BinarySearch(index, count, item, comparer);

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> Add(T value) => new ValueImmutableList<T>(this.list.Add(value));

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> AddRange(IEnumerable<T> items) => new ValueImmutableList<T>(this.list.AddRange(items));

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> Insert(int index, T item) => new ValueImmutableList<T>(this.list.Insert(index, item));

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> InsertRange(int index, IEnumerable<T> items) => new ValueImmutableList<T>(this.list.InsertRange(index, items));

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> Remove(T value) => new ValueImmutableList<T>(this.list.Remove(value));

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> Remove(T value, IEqualityComparer<T> equalityComparer) => new ValueImmutableList<T>(this.list.Remove(value, equalityComparer));

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> RemoveRange(int index, int count) => new ValueImmutableList<T>(this.list.RemoveRange(index, count));

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> RemoveRange(IEnumerable<T> items) => new ValueImmutableList<T>(this.list.RemoveRange(items));

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> RemoveRange(IEnumerable<T> items, IEqualityComparer<T> equalityComparer) => new ValueImmutableList<T>(this.list.RemoveRange(items, equalityComparer));

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> RemoveAt(int index) => new ValueImmutableList<T>(this.list.RemoveAt(index));

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> RemoveAll(Predicate<T> match) => new ValueImmutableList<T>(this.list.RemoveAll(match));

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> SetItem(int index, T value) => new ValueImmutableList<T>(this.list.SetItem(index, value));

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> Replace(T oldValue, T newValue) => new ValueImmutableList<T>(this.list.Replace(oldValue, newValue));

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> Replace(T oldValue, T newValue, IEqualityComparer<T> equalityComparer) => new ValueImmutableList<T>(this.list.Replace(oldValue, newValue, equalityComparer));

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> Reverse() => new ValueImmutableList<T>(this.list.Reverse());

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> Reverse(int index, int count) => new ValueImmutableList<T>(this.list.Reverse(index, count));

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> Sort() => new ValueImmutableList<T>(this.list.Sort());

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> Sort(Comparison<T> comparison) => new ValueImmutableList<T>(this.list.Sort(comparison));

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> Sort(IComparer<T> comparer) => new ValueImmutableList<T>(this.list.Sort(comparer));

        /// <inheritdoc cref="ImmutableList{T}"/>
        public ValueImmutableList<T> Sort(int index, int count, IComparer<T> comparer) => new ValueImmutableList<T>(this.list.Sort(index, count, comparer));
    }
}
