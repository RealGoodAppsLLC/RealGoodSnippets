// <copyright file="IApiDriver.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.Api.Interfaces
{
    /// <summary>
    /// A driver for sending HTTP requests to the API.
    /// The benefit of using a driver in our case is the ability to simplify the efforts of unit testing the API client.
    /// The following visual can help you understand why this is the case:
    ///
    /// Client Application / Test Suite -> API client interface -> API driver interface -> HTTP server
    ///
    /// By providing a good set of unit tests for the driver against a mock HTTP server, the unit tests for the API client
    /// can simply verify that the driver is invoked with the correct parameters. The alternative to this would require
    /// using a mock server for each individual method on the API client interface.
    /// </summary>
    public interface IApiDriver
    {
        /// <summary>
        /// Send an HTTP request to the API.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="method">The request HTTP method.</param>
        /// <param name="request">The request data (body) to be serialized.</param>
        /// <param name="cancellationToken">An instance of <see cref="CancellationToken"/>.</param>
        /// <typeparam name="TResponse">The response model type.</typeparam>
        /// <returns>An instance of <typeparamref name="TResponse"/> along with the HTTP response code.</returns>
        Task<(TResponse Response, HttpStatusCode HttpStatusCode)> SendAsync<TResponse>(
            Uri requestUrl,
            HttpMethod method,
            object? request,
            CancellationToken cancellationToken)
            where TResponse : class;

        /// <summary>
        /// Send an HTTP request to the API.
        /// </summary>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="method">The request HTTP method.</param>
        /// <param name="request">The request data (body) to be serialized.</param>
        /// <param name="cancellationToken">An instance of <see cref="CancellationToken"/>.</param>
        /// <returns>The HTTP response code.</returns>
        Task<HttpStatusCode> SendAsync(
            Uri requestUrl,
            HttpMethod method,
            object? request,
            CancellationToken cancellationToken);
    }
}
