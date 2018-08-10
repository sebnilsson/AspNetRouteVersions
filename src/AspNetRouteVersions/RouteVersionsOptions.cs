using System;

namespace AspNetRouteVersions
{
    public class RouteVersionsOptions
    {
        public const string DefaultAcceptRegexPattern = @"application\/vnd\.api-version\.v([\d]+)\+json";
        public const string DefaultCustomHeaderKey = "api-version";
        public const string DefaultQueryKey = "api-version";
        public const string DefaultRouteKey = "api-version";

        private string _acceptRegexPattern = DefaultAcceptRegexPattern;
        private string _customHeaderKey = DefaultCustomHeaderKey;
        private string _queryKey = DefaultQueryKey;
        private string _routeKey = DefaultRouteKey;

        public string AcceptRegexPattern
        {
            get => _acceptRegexPattern;
            set => _acceptRegexPattern = GetValidValue(value, DefaultAcceptRegexPattern);
        }
        public string CustomHeaderKey
        {
            get => _customHeaderKey;
            set => _customHeaderKey = GetValidValue(value, DefaultCustomHeaderKey);
        }
        public string QueryKey
        {
            get => _queryKey;
            set => _queryKey = GetValidValue(value, DefaultQueryKey);
        }
        public string RouteKey
        {
            get => _routeKey;
            set => _routeKey = GetValidValue(value, DefaultRouteKey);
        }
        public bool UseAcceptHeader { get; set; } = true;
        public bool UseCustomHeader { get; set; } = true;
        public bool UseRoute { get; set; } = true;
        public bool UseQuery { get; set; } = true;

        public void SetAcceptHeaderVendor(string vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            AcceptRegexPattern = $"application\\/vnd\\.{vendor}\\.v([\\d]+)\\+json";
        }

        private static string GetValidValue(string value, string fallback)
        {
            return !string.IsNullOrWhiteSpace(value) ? value : fallback;
        }
    }
}