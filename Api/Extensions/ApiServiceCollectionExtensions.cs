// <copyright file="ApiServiceCollectionExtensions.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.Api.Extensions
{
    /// <summary>
    /// Extension methods to register API dependencies.
    /// </summary>
    public static class ApiServiceCollectionExtensions
    {
        /// <summary>
        /// Below, we configure our API driver to retry transient failures with a delay between each retry.
        /// This array informs the policy how many times to retry and the number of seconds between each subsequent attempt.
        /// </summary>
        private static readonly List<int> Retries = new() { 1, 5, 10 };

        /// <summary>
        /// Adds the services to a collection related to using the API.
        /// </summary>
        /// <param name="services">An instance of <see cref="ServiceCollection"/>.</param>
        /// <param name="configurationSection">The configuration section for the API.</param>
        public static void AddApiServices(
            this ServiceCollection services,
            IConfigurationSection configurationSection)
        {
            services.Configure<ApiConfiguration>(configurationSection);
            services
                .AddHttpClient<IApiDriver, ApiDriver>((clientServices, client) =>
                {
                    var apiConfiguration = clientServices.GetRequiredService<IOptions<ApiConfiguration>>();
                    client.BaseAddress = apiConfiguration.Value.BaseUrl;
                })
                .AddPolicyHandler((_, _) => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(Retries.Select(seconds => TimeSpan.FromSeconds(seconds))));

            services.AddSingleton<IApiClient, ApiClient>();
        }
    }
}
