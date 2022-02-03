// <copyright file="ApiClient.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.Api.Services
{
    /// <inheritdoc cref="ApiClient" />
    public partial class ApiClient : IApiClient
    {
        private readonly IApiDriver apiDriver;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiClient"/> class.
        /// </summary>
        /// <param name="apiDriver">An instance of <see cref="IApiDriver"/>.</param>
        public ApiClient(IApiDriver apiDriver)
        {
            this.apiDriver = apiDriver;
        }

        /// <inheritdoc cref="IApiClient" />
        public async Task<ThingResponseModel> NewThingAsync(
            ThingRequestModel requestModel,
            CancellationToken cancellationToken)
        {
            var (response, _) = await this.apiDriver.SendAsync<ThingResponseModel>(
                new Uri($"/api/thing", UriKind.Relative),
                HttpMethod.Post,
                requestModel,
                cancellationToken);

            return response;
        }
    }
}
