using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using TafRestSharpWireMock.Tests.Models;
using TafRestSharpWireMock.Tests.Utils.Configuration;

namespace TafRestSharpWireMock.Tests.Utils.ApiServices
{
    public class ApiServiceLoader
    {
        private static Dictionary<string, ApiServiceConfig> _cachedServices =
            new Dictionary<string, ApiServiceConfig>();

        /// <summary>
        /// Loads an API service configuration from YAML file
        /// </summary>
        /// <param name="apiName">Name of the API service (e.g., "UserAPI")</param>
        /// <returns>Loaded API service configuration</returns>
        public static ApiServiceConfig LoadApiService(string apiName)
        {
            // Check cache first
            if (_cachedServices.ContainsKey(apiName))
            {
                return _cachedServices[apiName];
            }

            // Construct file path
            string fileName = $"{apiName}.yaml";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Config", "ApiServices", fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"API service configuration file not found: {filePath}");
            }

            // Read YAML file
            var yamlContent = File.ReadAllText(filePath);

            // Deserialize YAML
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var config = deserializer.Deserialize<ApiServiceConfig>(yamlContent);

            // Apply environment-specific overrides
            ApplyEnvironmentOverrides(config);

            // Cache the loaded config
            _cachedServices[apiName] = config;

            return config;
        }

        /// <summary>
        /// Applies environment-specific overrides to the configuration
        /// </summary>
        private static void ApplyEnvironmentOverrides(ApiServiceConfig config)
        {
            var currentEnv = ConfigurationManager.Instance.CurrentEnvironment;

            if (config.Environments != null && config.Environments.ContainsKey(currentEnv))
            {
                var envConfig = config.Environments[currentEnv];

                // Override base URL if specified
                if (!string.IsNullOrWhiteSpace(envConfig.BaseUrl))
                {
                    config.BaseUrl = envConfig.BaseUrl;
                }

                // Override timeout if specified
                if (envConfig.Timeout.HasValue)
                {
                    config.Timeout = envConfig.Timeout.Value;
                }

                // Override connection string if specified
                if (!string.IsNullOrWhiteSpace(envConfig.ConnectionString))
                {
                    config.ConnectionString = envConfig.ConnectionString;
                }
            }
        }

        /// <summary>
        /// Gets an endpoint configuration by name
        /// </summary>
        public static EndpointConfig GetEndpoint(string apiName, string endpointName)
        {
            var service = LoadApiService(apiName);

            if (!service.Endpoints.ContainsKey(endpointName))
            {
                throw new KeyNotFoundException($"Endpoint '{endpointName}' not found in API service '{apiName}'");
            }

            return service.Endpoints[endpointName];
        }

        /// <summary>
        /// Builds the complete endpoint URL with path parameters replaced
        /// Example: /api/users/{id} with {id: 2} becomes /api/users/2
        /// </summary>
        public static string BuildEndpointUrl(string apiName, string endpointName, Dictionary<string, string> pathParams = null)
        {
            var endpoint = GetEndpoint(apiName, endpointName);
            var path = endpoint.Path;

            // Replace path parameters
            if (pathParams != null && pathParams.Count > 0)
            {
                foreach (var param in pathParams)
                {
                    path = path.Replace($"{{{param.Key}}}", param.Value);
                }
            }

            return path;
        }

        /// <summary>
        /// Clears the cache (useful for testing)
        /// </summary>
        public static void ClearCache()
        {
            _cachedServices.Clear();
        }

        /// <summary>
        /// Loads all API services from the Config/ApiServices directory
        /// </summary>
        public static Dictionary<string, ApiServiceConfig> LoadAllApiServices()
        {
            var services = new Dictionary<string, ApiServiceConfig>();
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "Config", "ApiServices");

            if (!Directory.Exists(directory))
            {
                return services;
            }

            var yamlFiles = Directory.GetFiles(directory, "*.yaml");

            foreach (var filePath in yamlFiles)
            {
                var apiName = Path.GetFileNameWithoutExtension(filePath);
                services[apiName] = LoadApiService(apiName);
            }

            return services;
        }
    }
}
