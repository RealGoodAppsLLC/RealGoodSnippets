// <copyright file="IApiClient.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.Api.Interfaces
{
    public interface IApiClient
    {
        Task<ThingResponseModel> NewThingAsync(
            ThingRequestModel requestModel,
            CancellationToken cancellationToken);
    }
}
