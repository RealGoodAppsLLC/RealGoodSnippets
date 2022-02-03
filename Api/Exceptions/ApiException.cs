// <copyright file="ApiException.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.Api.Exceptions
{
    /// <summary>
    /// An exception indicating an error with a request to the API.
    /// </summary>
    /// <inheritdoc cref="Exception"/>
    public class ApiException : Exception
    {
        /// <inheritdoc cref="Exception"/>
        public ApiException()
        {
        }

        /// <inheritdoc cref="Exception"/>
        public ApiException(string message)
            : base(message)
        {
        }

        /// <inheritdoc cref="Exception"/>
        public ApiException(string message, Exception? inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Gets or sets the underlying HTTP response status code.
        /// </summary>
        public HttpStatusCode? StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the errors from the HTTP response.
        /// </summary>
        public ValueImmutableDictionary<string, string[]> Errors { get; set; } = ValueImmutableDictionary.Create(Enumerable.Empty<KeyValuePair<string, string[]>>());

        /// <summary>
        /// Gets or sets the extensions from the HTTP response.
        /// </summary>
        public ValueImmutableDictionary<string, string> Extensions { get; set; } = ValueImmutableDictionary.Create(Enumerable.Empty<KeyValuePair<string, string>>());

        /// <summary>
        /// Constructs an instance of <see cref="ApiException"/> using an instance of <see cref="HttpResponseMessage"/> and an optional inner exception.
        /// </summary>
        /// <param name="httpResponseMessage">An instance of <see cref="HttpResponseMessage"/>.</param>
        /// <param name="innerException">An optional instance of <see cref="Exception"/>.</param>
        /// <returns>An instance of <see cref="ApiException"/>.</returns>
        public static async Task<ApiException> CreateAsync(
            HttpResponseMessage? httpResponseMessage,
            Exception? innerException = null)
        {
            var responseContentString = await ExtractResponseContentStringAsync(httpResponseMessage?.Content);
            var (errors, extensions) = ExtractErrorsAndExtensions(responseContentString);

            var statusCode = httpResponseMessage?.StatusCode;
            var statusCodeInt = statusCode ?? 0;
            var exceptionMessage = $"An error occurred with an API request. (StatusCode = {statusCodeInt})";

            return new ApiException(exceptionMessage, innerException)
            {
                StatusCode = statusCode,
                Errors = errors,
                Extensions = extensions,
            };
        }

        /// <summary>
        /// Attempt to extract the response text from an instance of <see cref="HttpContent"/>.
        /// </summary>
        /// <param name="responseContent">An instance of <see cref="HttpContent"/>.</param>
        /// <returns>The response text, or null if it can not be extracted.</returns>
        private static async Task<string?> ExtractResponseContentStringAsync(HttpContent? responseContent)
        {
            string? responseContentString = null;

            if (responseContent != null)
            {
                responseContentString = await responseContent.ReadAsStringAsync();
            }

            return responseContentString;
        }

        /// <summary>
        /// Attempt to deserialize the response text and extract the errors and extensions.
        /// </summary>
        /// <param name="responseContentString">The response text to deserialize.</param>
        /// <returns>A tuple with a dictionary of error information and a dictionary of extensions.</returns>
        private static (ValueImmutableDictionary<string, string[]> Errors, ValueImmutableDictionary<string, string> Extensions) ExtractErrorsAndExtensions(string? responseContentString)
        {
            if (string.IsNullOrWhiteSpace(responseContentString))
            {
                return (ValueImmutableDictionary.Create(new Dictionary<string, string[]>()), ValueImmutableDictionary.Create(new Dictionary<string, string>()));
            }

            try
            {
                var responseWithErrors = JsonConvert.DeserializeObject<HttpResponseWithErrors>(responseContentString);

                if (responseWithErrors == null)
                {
                    throw new Exception("The HTTP response with errors must not be null.");
                }

                var errors = responseWithErrors.Errors;
                var extensions = responseWithErrors.Extensions;

                var baseExtensions = ExtractBaseExtensionsFromResponseContent(responseContentString);
                var allExtensions = CombineBaseExtensionsAndOtherExtensions(baseExtensions, extensions);

                return (errors, allExtensions);
            }
            catch
            {
                return (ValueImmutableDictionary.Create(new Dictionary<string, string[]>()), ValueImmutableDictionary.Create(new Dictionary<string, string>()));
            }
        }

        private static ValueImmutableDictionary<string, string> CombineBaseExtensionsAndOtherExtensions(
            ValueImmutableDictionary<string, string> baseExtensions,
            ValueImmutableDictionary<string, string> extensions)
        {
            var allExtensions = new Dictionary<string, string>();

            foreach (var (key, value) in baseExtensions)
            {
                allExtensions[key] = value;
            }

            foreach (var (key, value) in extensions)
            {
                allExtensions[key] = value;
            }

            return allExtensions.ToValueImmutableDictionary();
        }

        private static ValueImmutableDictionary<string, string> ExtractBaseExtensionsFromResponseContent(string responseContentString)
        {
            var dynamicObject = JsonConvert.DeserializeObject<JObject>(responseContentString);

            if (dynamicObject == null)
            {
                return new Dictionary<string, string>().ToValueImmutableDictionary();
            }

            var baseExtensions = new Dictionary<string, string>();
            var ignoreKeys = new List<string>
            {
                "title",
                "status",
                "errors",
                "extensions",
                "traceId",
                "type",
            };

            foreach (var dynamicProperty in dynamicObject.Properties())
            {
                var isIgnored = ignoreKeys.Any(ignoreKey =>
                    string.Equals(ignoreKey, dynamicProperty.Name, StringComparison.InvariantCultureIgnoreCase));

                if (isIgnored)
                {
                    continue;
                }

                var dynamicPropertyValue = dynamicProperty.Value;

                if (dynamicPropertyValue.Type != JTokenType.String)
                {
                    continue;
                }

                var dynamicPropertyValueAsJValue = dynamicPropertyValue as JValue;
                var dynamicPropertyValueAsString = dynamicPropertyValueAsJValue?.Value as string;

                if (dynamicPropertyValueAsString == null)
                {
                    continue;
                }

                baseExtensions[dynamicProperty.Name] = dynamicPropertyValueAsString;
            }

            return baseExtensions.ToValueImmutableDictionary();
        }

        /// <summary>
        /// The response model with errors in them.
        /// </summary>
        protected class HttpResponseWithErrors
        {
            /// <summary>
            /// Gets or sets the errors in the response.
            /// </summary>
            public ValueImmutableDictionary<string, string[]> Errors { get; set; } = ValueImmutableDictionary.Create(Enumerable.Empty<KeyValuePair<string, string[]>>());

            /// <summary>
            /// Gets or sets the extensions in the response.
            /// </summary>
            public ValueImmutableDictionary<string, string> Extensions { get; set; } = ValueImmutableDictionary.Create(Enumerable.Empty<KeyValuePair<string, string>>());
        }
    }
}
