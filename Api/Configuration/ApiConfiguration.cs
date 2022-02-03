// <copyright file="ApiConfiguration.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.Api.Configuration
{
    /// <summary>
    /// A configuration for the API client.
    /// </summary>
    public sealed class ApiConfiguration
    {
        /// <summary>
        /// Gets or sets the base URL of the API.
        /// </summary>
        public Uri? BaseUrl { get; set; }
    }
}
