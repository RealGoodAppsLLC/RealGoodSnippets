// <copyright file="ValueImmutableHashSet.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.Immutability
{
    /// <summary>
    /// Static class with static method to create value immutable hash sets.
    /// </summary>
    public static class ValueImmutableHashSet
    {
        /// <summary>
        /// Create a value immutable hash set of items.
        /// </summary>
        /// <param name="items">The items in the immutable hash set.</param>
        /// <typeparam name="T">The type of item.</typeparam>
        /// <returns>An instance of <see cref="ValueImmutableHashSet{T}"/>.</returns>
        public static ValueImmutableHashSet<T> Create<T>(params T[] items) => new ValueImmutableHashSet<T>(ImmutableHashSet.Create(items));
    }

    /// <summary>
    /// A value immutable hash set which is compared by value instead of by reference to other hash sets.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
#pragma warning disable SA1402
    [JsonConverter(typeof(ValueImmutableHashSetJsonConverter))]
    public sealed class ValueImmutableHashSet<T> : IEnumerable<T>, IEquatable<ValueImmutableHashSet<T>>
#pragma warning restore SA1402
    {
        private readonly ImmutableHashSet<T> hashSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueImmutableHashSet{T}"/> class.
        /// </summary>
        /// <param name="hashSet">An instance of <see cref="ImmutableHashSet{T}"/>.</param>
        public ValueImmutableHashSet(ImmutableHashSet<T> hashSet) => this.hashSet = hashSet;

        /// <inheritdoc cref="ImmutableHashSet{T}"/>
        public int Count => this.hashSet.Count;

        /// <inheritdoc cref="ImmutableHashSet{T}"/>
        public bool IsEmpty => this.hashSet.IsEmpty;

        /// <inheritdoc cref="IEnumerable{T}"/>
        public IEnumerator<T> GetEnumerator() => this.hashSet.GetEnumerator();

        /// <inheritdoc cref="IEnumerable{T}"/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc cref="ImmutableHashSet{T}"/>
        public ValueImmutableHashSet<T> Clear() => new ValueImmutableHashSet<T>(this.hashSet.Clear());

        /// <inheritdoc cref="ImmutableHashSet{T}"/>
        public ValueImmutableHashSet<T> Add(T item) => new ValueImmutableHashSet<T>(this.hashSet.Add(item));

        /// <inheritdoc cref="ImmutableHashSet{T}"/>
        public ValueImmutableHashSet<T> Remove(T item) => new ValueImmutableHashSet<T>(this.hashSet.Remove(item));

        /// <inheritdoc cref="ImmutableHashSet{T}"/>
        public bool TryGetValue(T equalValue, out T actualValue) => this.hashSet.TryGetValue(equalValue, out actualValue);

        /// <inheritdoc cref="ImmutableHashSet{T}"/>
        public ValueImmutableHashSet<T> Union(IEnumerable<T> other) => new ValueImmutableHashSet<T>(this.hashSet.Union(other));

        /// <inheritdoc cref="ImmutableHashSet{T}"/>
        public ValueImmutableHashSet<T> Intersect(IEnumerable<T> other) => new ValueImmutableHashSet<T>(this.hashSet.Intersect(other));

        /// <inheritdoc cref="ImmutableHashSet{T}"/>
        public ValueImmutableHashSet<T> Except(IEnumerable<T> other) => new ValueImmutableHashSet<T>(this.hashSet.Except(other));

        /// <inheritdoc cref="ImmutableHashSet{T}"/>
        public ValueImmutableHashSet<T> SymmetricExcept(IEnumerable<T> other) => new ValueImmutableHashSet<T>(this.hashSet.SymmetricExcept(other));

        /// <inheritdoc cref="ImmutableHashSet{T}"/>
        public bool SetEquals(IEnumerable<T> other) => this.hashSet.SetEquals(other);

        /// <inheritdoc cref="ImmutableHashSet{T}"/>
        public bool IsProperSubsetOf(IEnumerable<T> other) => this.hashSet.IsProperSubsetOf(other);

        /// <inheritdoc cref="ImmutableHashSet{T}"/>
        public bool IsProperSupersetOf(IEnumerable<T> other) => this.hashSet.IsProperSupersetOf(other);

        /// <inheritdoc cref="ImmutableHashSet{T}"/>
        public bool IsSubsetOf(IEnumerable<T> other) => this.hashSet.IsSubsetOf(other);

        /// <inheritdoc cref="ImmutableHashSet{T}"/>
        public bool IsSupersetOf(IEnumerable<T> other) => this.hashSet.IsSupersetOf(other);

        /// <inheritdoc cref="ImmutableHashSet{T}"/>
        public bool Overlaps(IEnumerable<T> other) => this.hashSet.Overlaps(other);

        /// <inheritdoc cref="IEquatable{T}"/>
        public bool Equals(ValueImmutableHashSet<T>? other) =>
            !ReferenceEquals(null, other)
            && (ReferenceEquals(this, other) || this.hashSet.SetEquals(other.hashSet));

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

            if (!(obj is ValueImmutableHashSet<T> objAsValueImmutableHashSet))
            {
                return false;
            }

            return this.Equals(objAsValueImmutableHashSet);
        }

        /// <inheritdoc cref="IEquatable{T}"/>
        public override int GetHashCode() => this.hashSet.GetHashCode();

        /// <summary>
        /// Converts the value hash set to a value immutable list.
        /// </summary>
        /// <returns>An instance of <see cref="ValueImmutableList{T}"/>.</returns>
        public ValueImmutableList<T> ToValueImmutableList() => new ValueImmutableList<T>(this.hashSet.ToImmutableList());

        /// <summary>
        /// Returns the reference-based immutable hash set which has reference-based equality checks.
        /// </summary>
        /// <returns>An instance of <see cref="ImmutableHashSet{T}"/>.</returns>
        public ImmutableHashSet<T> AsImmutableHashSet() => this.hashSet;
    }
}
