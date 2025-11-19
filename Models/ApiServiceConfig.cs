using System.Collections.Generic;

namespace TafRestSharpWireMock.Tests.Models
{
    public class ApiServiceConfig
    {
        public string ApiName { get; set; }
        public string Description { get; set; }
        public string BaseUrl { get; set; }
        public int Timeout { get; set; }
        public int RetryCount { get; set; }
        public string ConnectionString { get; set; }

        public Dictionary<string, EnvironmentConfig> Environments { get; set; }
        public Dictionary<string, EndpointConfig> Endpoints { get; set; }
        public Dictionary<string, string> DefaultHeaders { get; set; }
        public AuthenticationConfig Authentication { get; set; }

        public ApiServiceConfig()
        {
            Environments = new Dictionary<string, EnvironmentConfig>();
            Endpoints = new Dictionary<string, EndpointConfig>();
            DefaultHeaders = new Dictionary<string, string>();
            Authentication = new AuthenticationConfig();
        }
    }

    public class EnvironmentConfig
    {
        public string BaseUrl { get; set; }
        public int? Timeout { get; set; }
        public string ConnectionString { get; set; }
    }

    public class EndpointConfig
    {
        public string Path { get; set; }
        public string Method { get; set; }
        public string Description { get; set; }
        public int? Timeout { get; set; }  // Optional override
    }

    public class AuthenticationConfig
    {
        public string Type { get; set; }  // none, apiKey, bearer, basic
        public string ApiKeyHeader { get; set; }
        public string ApiKeyValue { get; set; }
        public string BearerToken { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public AuthenticationConfig()
        {
            Type = "none";
        }
    }
}
