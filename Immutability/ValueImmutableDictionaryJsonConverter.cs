// <copyright file="ValueImmutableDictionaryJsonConverter.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.Immutability
{
    /// <inheritdoc cref="JsonConverter{T}"/>
    public class ValueImmutableDictionaryJsonConverter : JsonConverter
    {
        /// <inheritdoc cref="JsonConverter{T}"/>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var boundedType = value.GetType();
            var method = boundedType.GetMethod("AsImmutableDictionary", BindingFlags.Public | BindingFlags.Instance);
            var underlyingDictionary = method?.Invoke(value, new object[] { });

            if (underlyingDictionary == null)
            {
                writer.WriteNull();
                return;
            }

            var jObject = JObject.FromObject(underlyingDictionary);
            jObject.WriteTo(writer);
        }

        /// <inheritdoc cref="JsonConverter{T}"/>
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var keyType = objectType.GenericTypeArguments[0];
            var valueType = objectType.GenericTypeArguments[1];
            var genericDictionary = typeof(IDictionary<,>);
            var boundedGenericDictionary = genericDictionary.MakeGenericType(keyType, valueType);

            var readOnlyDictionary = serializer.Deserialize(reader, boundedGenericDictionary);

            var method = typeof(ValueImmutableDictionaryJsonConverter).GetMethod(
                nameof(this.ToValueImmutableDictionary),
                BindingFlags.Static | BindingFlags.NonPublic);

            var genericMethod = method?.MakeGenericMethod(keyType, valueType);

            return genericMethod?.Invoke(null, new[] { readOnlyDictionary });
        }

        /// <inheritdoc cref="JsonConverter{T}"/>
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(ValueImmutableDictionary<,>));
        }

        private static ValueImmutableDictionary<TKey, TValue> ToValueImmutableDictionary<TKey, TValue>(IDictionary<TKey, TValue> input)
            where TKey : notnull
        {
            return input.ToValueImmutableDictionary();
        }
    }
}
