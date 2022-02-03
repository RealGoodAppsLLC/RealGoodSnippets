// <copyright file="ValueImmutableDictionary.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.Immutability
{
    /// <summary>
    /// Static class with static method to create value immutable dictionaries.
    /// </summary>
    public static class ValueImmutableDictionary
    {
        /// <summary>
        /// Create a value immutable dictionary of items.
        /// </summary>
        /// <param name="items">The items in the immutable dictionary.</param>
        /// <typeparam name="TKey">The type of item key.</typeparam>
        /// <typeparam name="TValue">The type of item value.</typeparam>
        /// <returns>An instance of <see cref="ValueImmutableDictionary{TKey, TValue}"/>.</returns>
        public static ValueImmutableDictionary<TKey, TValue> Create<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> items)
            where TKey : notnull
        {
            return new ValueImmutableDictionary<TKey, TValue>(ImmutableDictionary.CreateRange(items));
        }
    }

    /// <summary>
    /// A value immutable dictionary which is compared by value instead of by reference to other dictionaries.
    /// </summary>
    /// <typeparam name="TKey">The type of item key.</typeparam>
    /// <typeparam name="TValue">The type of item value.</typeparam>
#pragma warning disable SA1402
    [JsonConverter(typeof(ValueImmutableDictionaryJsonConverter))]
    public sealed class ValueImmutableDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEquatable<ValueImmutableDictionary<TKey, TValue>>
        where TKey : notnull
#pragma warning restore SA1402
    {
        private readonly ImmutableDictionary<TKey, TValue> dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueImmutableDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">An instance of <see cref="ImmutableDictionary{TKey, TValue}"/>.</param>
        public ValueImmutableDictionary(ImmutableDictionary<TKey, TValue> dictionary) => this.dictionary = dictionary;

        /// <inheritdoc cref="ImmutableDictionary{TKey, TValue}"/>
        public int Count => this.dictionary.Count;

        /// <inheritdoc cref="ImmutableDictionary{TKey, TValue}"/>
        public bool IsEmpty => this.dictionary.IsEmpty;

        /// <inheritdoc cref="ImmutableDictionary{TKey, TValue}"/>
        public IEnumerable<TKey> Keys => this.dictionary.Keys;

        /// <inheritdoc cref="ImmutableDictionary{TKey, TValue}"/>
        public IEnumerable<TValue> Values => this.dictionary.Values;

        /// <inheritdoc cref="ImmutableDictionary{TKey, TValue}"/>
        public TValue this[TKey key] => this.dictionary[key];

        /// <inheritdoc cref="IEnumerable{T}"/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => this.dictionary.GetEnumerator();

        /// <inheritdoc cref="IEnumerable{T}"/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc cref="IEquatable{T}"/>
        public bool Equals(ValueImmutableDictionary<TKey, TValue>? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var dictionariesEqual = this.dictionary.Keys.Count() == other.dictionary.Keys.Count()
                                    && this.dictionary.Keys.All(k => other.dictionary.ContainsKey(k) && object.Equals(other.dictionary[k], this.dictionary[k]));

            return dictionariesEqual;
        }

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

            if (!(obj is ValueImmutableDictionary<TKey, TValue> objAsValueImmutableDictionary))
            {
                return false;
            }

            return this.Equals(objAsValueImmutableDictionary);
        }

        /// <inheritdoc cref="IEquatable{T}"/>
        public override int GetHashCode() => this.dictionary.GetHashCode();

        /// <summary>
        /// Returns the reference-based immutable dictionary which has reference-based equality checks.
        /// </summary>
        /// <returns>An instance of <see cref="ImmutableDictionary{TKey, TValue}"/>.</returns>
        public ImmutableDictionary<TKey, TValue> AsImmutableDictionary() => this.dictionary;

        /// <inheritdoc cref="ImmutableDictionary{TKey, TValue}"/>
        public ValueImmutableDictionary<TKey, TValue> Clear() => new(this.dictionary.Clear());

        /// <inheritdoc cref="ImmutableDictionary{TKey, TValue}"/>
        public ValueImmutableDictionary<TKey, TValue> Add(TKey key, TValue value) => new(this.dictionary.Add(key, value));

        /// <inheritdoc cref="ImmutableDictionary{TKey, TValue}"/>
        public ValueImmutableDictionary<TKey, TValue> AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
            => new(this.dictionary.AddRange(pairs));

        /// <inheritdoc cref="ImmutableDictionary{TKey, TValue}"/>
        public ValueImmutableDictionary<TKey, TValue> SetItem(TKey key, TValue value) => new(this.dictionary.SetItem(key, value));

        /// <inheritdoc cref="ImmutableDictionary{TKey, TValue}"/>
        public ValueImmutableDictionary<TKey, TValue> SetItems(IEnumerable<KeyValuePair<TKey, TValue>> items)
            => new(this.dictionary.SetItems(items));

        /// <inheritdoc cref="ImmutableDictionary{TKey, TValue}"/>
        public ValueImmutableDictionary<TKey, TValue> Remove(TKey key) => new(this.dictionary.Remove(key));

        /// <inheritdoc cref="ImmutableDictionary{TKey, TValue}"/>
        public ValueImmutableDictionary<TKey, TValue> RemoveRange(IEnumerable<TKey> keys)
            => new(this.dictionary.RemoveRange(keys));

        /// <inheritdoc cref="ImmutableDictionary{TKey, TValue}"/>
        public bool ContainsKey(TKey key) => this.dictionary.ContainsKey(key);

        /// <inheritdoc cref="ImmutableDictionary{TKey, TValue}"/>
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
            => this.dictionary.TryGetValue(key, out value);

        /// <inheritdoc cref="ImmutableDictionary{TKey, TValue}"/>
        public bool ContainsValue(TValue value) => this.dictionary.ContainsValue(value);
    }
}
