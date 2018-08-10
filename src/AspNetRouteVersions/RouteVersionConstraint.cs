using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace AspNetRouteVersions
{
    public class RouteVersionConstraint : IActionConstraint
    {
        private readonly Regex _acceptHeaderRegex;
        private readonly RouteVersionsOptions _options;

        public RouteVersionConstraint(string version, bool isDefault, RouteVersionsOptions options)
        {
            Version = version ?? throw new ArgumentNullException(nameof(version));
            IsDefault = isDefault;
            _options = options ?? throw new ArgumentNullException(nameof(options));

            _acceptHeaderRegex = new Regex(options.AcceptRegexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public bool IsDefault { get; }

        public int Order { get; set; }

        public string Version { get; }

        public bool Accept(ActionConstraintContext context)
        {
            var versionCandidates = GetDistinctVersionCandidates(context);

            if (versionCandidates.Count > 1)
            {
                var message = GetMultipleCandidatesExceptionMessage(versionCandidates);
                throw new InvalidOperationException(message);
            }

            if (!versionCandidates.Any())
            {
                return IsDefault;
            }

            var candidate = versionCandidates.First();

            var isMatchingVersion = candidate.Equals(Version, StringComparison.InvariantCultureIgnoreCase);
            return isMatchingVersion;
        }

        private IReadOnlyCollection<string> GetDistinctVersionCandidates(ActionConstraintContext context)
        {
            var versionCandidates = GetVersionCandidates(context);

            var validCandidates = versionCandidates
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim());

            return validCandidates
                .GroupBy(x => x)
                .Select(x => x.First())
                .ToList();
        }

        private IEnumerable<string> GetVersionCandidates(ActionConstraintContext context)
        {
            if (_options.UseRoute)
            {
                yield return GetVersionInRoute(context);
            }
            if (_options.UseQuery)
            {
                yield return GetVersionInQueryString(context);
            }
            if (_options.UseCustomHeader)
            {
                yield return GetVersionInCustomHeader(context);
            }
            if (_options.UseAcceptHeader)
            {
                yield return GetVersionInAcceptHeader(context);
            }
        }

        private string GetMultipleCandidatesExceptionMessage(IEnumerable<string> candidates)
        {
            var joinedCandidates = string.Join(Environment.NewLine, candidates);

            return "Mismatching versions specified. The following versions where specified:" +
                $"{Environment.NewLine}{Environment.NewLine}{joinedCandidates}";
        }

        private string GetVersionInRoute(ActionConstraintContext context)
        {
            return Convert.ToString(context.RouteContext.RouteData.Values[_options.RouteKey]);
        }

        private string GetVersionInQueryString(ActionConstraintContext context)
        {
            return Convert.ToString(context.RouteContext.HttpContext.Request.Query[_options.QueryKey]);
        }

        private string GetVersionInCustomHeader(ActionConstraintContext context)
        {
            return Convert.ToString(context.RouteContext.HttpContext.Request.Headers[_options.CustomHeaderKey]);
        }

        private string GetVersionInAcceptHeader(ActionConstraintContext context)
        {
            var accept =
                Convert.ToString(context.RouteContext.HttpContext.Request.Headers["Accept"]);

            var match = accept != null ? _acceptHeaderRegex.Match(accept) : null;

            var group = match?.Groups.Count > 1 ? match.Groups[1] : null;

            return group != null && group.Success ? group.Value : null;
        }
    }
}