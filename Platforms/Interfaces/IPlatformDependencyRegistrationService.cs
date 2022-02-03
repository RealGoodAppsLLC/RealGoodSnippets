// <copyright file="IPlatformDependencyRegistrationService.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.Platforms.Interfaces
{
    /// <summary>
    /// A service for registering platform-specific services with a service collection.
    /// </summary>
    public interface IPlatformDependencyRegistrationService
    {
        /// <summary>
        /// Register platform-specific services with a service collection.
        /// </summary>
        /// <param name="services">An instance of <see cref="IServiceCollection"/>.</param>
        void Register(IServiceCollection services);
    }
}
