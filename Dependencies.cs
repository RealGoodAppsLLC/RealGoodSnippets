// <copyright file="Dependencies.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps
{
    /// <summary>
    /// The default dependency injection support in Xamarin Forms works nicely for platform-specific implementations, but
    /// otherwise is fairly limited. Here we provide support for Microsoft's full featured dependency injection framework.
    /// The main advantages are: constructor injection, logging, configuration extensions, and HTTP extensions with Polly support.
    /// For more information, see: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.1.
    /// Please note that while the above page is specific to ASP.NET Core, the framework itself works in any .NET Standard project.
    /// </summary>
    public static class Dependencies
    {
        private static IServiceProvider? serviceProvider;

        /// <summary>
        /// Gets or sets the service provider.
        /// This is used so we can continue to use dependency injection in our XAML code-behind files.
        /// Because the classes such as "page", "layout", etc are constructed by the Xamarin Forms SDK, we do not have
        /// control over the constructor or the lifecycle of the class.
        /// If you need access to a dependency in one of these classes, simply use the GetRequiredService method on
        /// Dependencies.ServiceProvider and pass in the interface as the type parameter.
        /// </summary>
        public static IServiceProvider ServiceProvider
        {
            get
            {
                if (serviceProvider == null)
                {
                    throw new Exception("The service provider is null!");
                }

                return serviceProvider;
            }
            set => serviceProvider = value;
        }

        /// <summary>
        /// Configure the services in our dependency injection container.
        /// At the end of this method, the service provider is built and ready for use.
        /// </summary>
        public static void ConfigureServices()
        {
            var services = new ServiceCollection();

            // For more information, see: https://github.com/reactiveui/splat/tree/main/src/Splat.Microsoft.Extensions.DependencyInjection
            services.UseMicrosoftDependencyResolver();
            var resolver = Locator.CurrentMutable;
            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();

            var configurationRoot = LoadConfiguration();

            services.AddSingleton<ICustomAttributeProvider>(_ => typeof(Dependencies).Assembly);

            services.AddApiServices(configurationRoot.GetSection("Api"));
            services.AddSingleton<IRegisterExceptionPromptHandlerService, RegisterExceptionPromptHandlerService>();
            
            // Since Android and iOS provide their own implementations of a number interfaces, we need a way to register
            // them inside our service collection. The trick here is to use the built-in Xamarin Forms dependency injection container
            // one time to locate a service that can register the platform-specific services with our service collection.
            var platformDependencyRegistrationService = DependencyService.Get<IPlatformDependencyRegistrationService>();
            platformDependencyRegistrationService.Register(services);

            ServiceProvider = services.BuildServiceProvider();

            // For more information, see: https://github.com/reactiveui/splat/tree/main/src/Splat.Microsoft.Extensions.DependencyInjection
            ServiceProvider.UseMicrosoftDependencyResolver();
        }

        /// <summary>
        /// Load the appsettings.json file from the platform (iOS or Android) and return a configuration root interface.
        /// </summary>
        /// <returns>An instance of <see cref="IConfigurationRoot"/>.</returns>
        private static IConfigurationRoot LoadConfiguration()
        {
            var appSettingsStream = FileSystem
                .OpenAppPackageFileAsync("appsettings.json")
                .Result;

            var reader = new StreamReader(appSettingsStream, Encoding.UTF8);
            var appSettings = reader.ReadToEnd();
            var appSettingsBytes = Encoding.UTF8.GetBytes(appSettings);

            // If you are wondering why we don't just pass the appSettingsStream directly into AddJsonStream below:
            // It is possible that file streams are automatically closed by the platform after some period of time.
            // If it were possible to pass the appSettings string directly into AddJsonStream, that would be the ideal scenario.
            // However, since we can not and we are unsure if file streams are automatically garbage collected in Android/iOS
            // we can simply write appSettings to a MemoryStream that we construct which should be safe from the platform garbage collection.
            var memoryStream = new MemoryStream();
            memoryStream.Write(appSettingsBytes, 0, appSettingsBytes.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonStream(memoryStream);

            return configurationBuilder.Build();
        }
    }
}
