// <copyright file="ValueExtensions.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.Immutability
{
    /// <summary>
    /// Extension methods for converting immutable collections to value based immutable collections.
    /// </summary>
    public static class ValueExtensions
    {
        /// <summary>
        /// Converts an immutable hash set to a value immutable hash set.
        /// </summary>
        /// <param name="self">The source immutable hash set.</param>
        /// <typeparam name="T">The type of item.</typeparam>
        /// <returns>An instance of <see cref="ValueImmutableHashSet{T}"/>.</returns>
        public static ValueImmutableHashSet<T> ToValueImmutableHashSet<T>(this ImmutableHashSet<T> self) => new ValueImmutableHashSet<T>(self);

        /// <summary>
        /// Converts an immutable list to a value immutable list.
        /// </summary>
        /// <param name="self">The source immutable list.</param>
        /// <typeparam name="T">The type of item.</typeparam>
        /// <returns>An instance of <see cref="ValueImmutableList{T}"/>.</returns>
        public static ValueImmutableList<T> ToValueImmutableList<T>(this ImmutableList<T> self) => new ValueImmutableList<T>(self);

        /// <summary>
        /// Converts an immutable dictionary to a value immutable dictionary.
        /// </summary>
        /// <param name="self">The source immutable dictionary.</param>
        /// <typeparam name="TKey">The type of item key.</typeparam>
        /// <typeparam name="TValue">The type of item value.</typeparam>
        /// <returns>An instance of <see cref="ValueImmutableDictionary{TKey, TValue}"/>.</returns>
        public static ValueImmutableDictionary<TKey, TValue> ToValueImmutableDictionary<TKey, TValue>(this ImmutableDictionary<TKey, TValue> self)
            where TKey : notnull => new ValueImmutableDictionary<TKey, TValue>(self);

        /// <summary>
        /// Converts a dictionary to a value immutable dictionary.
        /// </summary>
        /// <param name="self">The source immutable dictionary.</param>
        /// <typeparam name="TKey">The type of item key.</typeparam>
        /// <typeparam name="TValue">The type of item value.</typeparam>
        /// <returns>An instance of <see cref="ValueImmutableDictionary{TKey, TValue}"/>.</returns>
        public static ValueImmutableDictionary<TKey, TValue> ToValueImmutableDictionary<TKey, TValue>(this IDictionary<TKey, TValue> self)
            where TKey : notnull => ValueImmutableDictionary.Create(self);

        /// <summary>
        /// Converts an enumerable to a value immutable list.
        /// </summary>
        /// <param name="self">The source enumerable.</param>
        /// <typeparam name="T">The type of item.</typeparam>
        /// <returns>An instance of <see cref="ValueImmutableList{T}"/>.</returns>
        public static ValueImmutableList<T> ToValueImmutableList<T>(this IEnumerable<T> self) => new ValueImmutableList<T>(self.ToImmutableList());

        /// <summary>
        /// Converts a list to a value immutable list.
        /// </summary>
        /// <param name="self">The source list.</param>
        /// <typeparam name="T">The type of item.</typeparam>
        /// <returns>An instance of <see cref="ValueImmutableList{T}"/>.</returns>
        public static ValueImmutableList<T> ToValueImmutableList<T>(this List<T> self) => new ValueImmutableList<T>(self.ToImmutableList());

        /// <summary>
        /// Converts an enumerable to a value immutable hash set.
        /// </summary>
        /// <param name="self">The source enumerable.</param>
        /// <typeparam name="T">The type of item.</typeparam>
        /// <returns>An instance of <see cref="ValueImmutableHashSet{T}"/>.</returns>
        public static ValueImmutableHashSet<T> ToValueImmutableHashSet<T>(this IEnumerable<T> self) => new ValueImmutableHashSet<T>(self.ToImmutableHashSet());

        /// <summary>
        /// Converts a list to a value immutable hash set.
        /// </summary>
        /// <param name="self">The source list.</param>
        /// <typeparam name="T">The type of item.</typeparam>
        /// <returns>An instance of <see cref="ValueImmutableHashSet{T}"/>.</returns>
        public static ValueImmutableHashSet<T> ToValueImmutableHashSet<T>(this List<T> self) => new ValueImmutableHashSet<T>(self.ToImmutableHashSet());
    }
}
