using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetRouteVersions
{
    public static class RouteVersionsServiceCollectionExtensions
    {
        public static void ConfigureRouteVersions(
            this IServiceCollection services,
            RouteVersionsOptions options)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            services.TryAddSingleton(options);
        }
        public static void ConfigureRouteVersions(
            this IServiceCollection services,
            Action<RouteVersionsOptions> configureRouteVersions)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (configureRouteVersions == null)
                throw new ArgumentNullException(nameof(configureRouteVersions));

            var options = new RouteVersionsOptions();

            configureRouteVersions(options);

            services.ConfigureRouteVersions(options);
        }
    }
}