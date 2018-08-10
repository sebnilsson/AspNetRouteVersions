using System;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace AspNetRouteVersions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RouteVersionAttribute : Attribute, IActionConstraintFactory
    {
        public RouteVersionAttribute(object version)
            : this(version != null ? Convert.ToString(version) : null, isDefault: false)
        {
        }

        public RouteVersionAttribute(object version, bool isDefault)
            : this(version != null ? Convert.ToString(version) : null, isDefault: isDefault)
        {
        }

        public RouteVersionAttribute(string version)
            : this(version, isDefault: false)
        {
        }

        public RouteVersionAttribute(string version, bool isDefault)
        {
            Version = version ?? throw new ArgumentNullException(nameof(version));
            IsDefault = isDefault;
        }

        public bool IsDefault { get; set; }

        public bool IsReusable => true;

        public string Version { get; }

        public IActionConstraint CreateInstance(IServiceProvider services)
        {
            var options = (services.GetService(typeof(RouteVersionsOptions)) as RouteVersionsOptions)
                ?? new RouteVersionsOptions();

            var hasAnyUse = options.UseAcceptHeader
                || options.UseCustomHeader
                || options.UseQuery
                || options.UseRoute;

            if (!hasAnyUse)
            {
                return null;
            }

            return new RouteVersionConstraint(Version, IsDefault, options);
        }
    }
}