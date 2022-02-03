// <copyright file="ApiDriver.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.Api.Services
{
    /// <inheritdoc cref="IApiDriver"/>
    public sealed class ApiDriver : IApiDriver
    {
        private const string JsonContentType = "application/json";

        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiDriver"/> class.
        /// </summary>
        /// <param name="httpClient">An instance of <see cref="HttpClient"/>.</param>
        public ApiDriver(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc cref="IApiDriver"/>
        public async Task<(TResponse Response, HttpStatusCode HttpStatusCode)> SendAsync<TResponse>(
            Uri requestUrl,
            HttpMethod method,
            object? request,
            CancellationToken cancellationToken)
            where TResponse : class
        {
            TResponse? response = null;

            var httpStatusCode = await this.InternalSendAsync(
                requestUrl,
                method,
                GetRequestHttpContentFromObject(request),
                responseText =>
                    response = JsonConvert.DeserializeObject<TResponse>(responseText),
                cancellationToken);

            if (response != null)
            {
                return (response, httpStatusCode);
            }

            var exception = await ApiException.CreateAsync(null);
            throw exception;
        }

        /// <inheritdoc cref="IApiDriver"/>
        public async Task<HttpStatusCode> SendAsync(
            Uri requestUrl,
            HttpMethod method,
            object? request,
            CancellationToken cancellationToken)
        {
            return await this.InternalSendAsync(
                requestUrl,
                method,
                GetRequestHttpContentFromObject(request),
                null,
                cancellationToken);
        }

        private static HttpContent? GetRequestHttpContentFromObject(object? request)
        {
            return request == null
                ? null
                : new StringContent(
                    JsonConvert.SerializeObject(request),
                    Encoding.UTF8,
                    JsonContentType);
        }

        private async Task<HttpStatusCode> InternalSendAsync(
            Uri requestUrl,
            HttpMethod method,
            HttpContent? requestContent,
            Action<string>? onResponseCallback,
            CancellationToken cancellationToken)
        {
            HttpRequestMessage? httpRequest = null;
            HttpResponseMessage? httpResponse = null;

            try
            {
                httpRequest = new HttpRequestMessage
                {
                    Method = method,
                    Content = requestContent,
                    RequestUri = requestUrl,
                };

                httpResponse = await this.httpClient.SendAsync(
                    httpRequest,
                    cancellationToken);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    throw await ApiException.CreateAsync(httpResponse);
                }

                if (onResponseCallback != null)
                {
                    var responseText = await httpResponse.Content.ReadAsStringAsync();
                    onResponseCallback.Invoke(responseText);
                }

                return httpResponse.StatusCode;
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    throw;
                }

                throw await ApiException.CreateAsync(httpResponse, ex);
            }
            finally
            {
                httpRequest?.Dispose();
                httpResponse?.Dispose();
                requestContent?.Dispose();
            }
        }
    }
}
