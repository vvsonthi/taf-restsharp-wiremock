using System;
using System.Collections.Generic;
using TafRestSharpWireMock.Tests.Models;
using Reqnroll;

namespace TafRestSharpWireMock.Tests.Utils.ApiServices
{
    public class ApiServiceManager
    {
        private readonly ScenarioContext _context;
        private const string CURRENT_API_SERVICE_KEY = "CurrentApiService";
        private const string CURRENT_API_CONFIG_KEY = "CurrentApiConfig";

        public ApiServiceManager(ScenarioContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Initializes an API service for the current scenario
        /// </summary>
        public void InitializeService(string apiName)
        {
            var config = ApiServiceLoader.LoadApiService(apiName);
            _context[CURRENT_API_SERVICE_KEY] = apiName;
            _context[CURRENT_API_CONFIG_KEY] = config;
        }

        /// <summary>
        /// Gets the current API service name
        /// </summary>
        public string GetCurrentServiceName()
        {
            if (!_context.ContainsKey(CURRENT_API_SERVICE_KEY))
            {
                throw new InvalidOperationException("No API service has been initialized. Use 'Given I have the {ApiName} service initialized' first.");
            }

            return (string)_context[CURRENT_API_SERVICE_KEY];
        }

        /// <summary>
        /// Gets the current API service configuration
        /// </summary>
        public ApiServiceConfig GetCurrentServiceConfig()
        {
            if (!_context.ContainsKey(CURRENT_API_CONFIG_KEY))
            {
                throw new InvalidOperationException("No API service has been initialized. Use 'Given I have the {ApiName} service initialized' first.");
            }

            return (ApiServiceConfig)_context[CURRENT_API_CONFIG_KEY];
        }

        /// <summary>
        /// Gets an endpoint from the current API service
        /// </summary>
        public EndpointConfig GetEndpoint(string endpointName)
        {
            var serviceName = GetCurrentServiceName();
            return ApiServiceLoader.GetEndpoint(serviceName, endpointName);
        }

        /// <summary>
        /// Builds the full endpoint URL with path parameters
        /// </summary>
        public string BuildEndpointUrl(string endpointName, Dictionary<string, string> pathParams = null)
        {
            var serviceName = GetCurrentServiceName();
            return ApiServiceLoader.BuildEndpointUrl(serviceName, endpointName, pathParams);
        }

        /// <summary>
        /// Gets the base URL for the current API service
        /// </summary>
        public string GetBaseUrl()
        {
            var config = GetCurrentServiceConfig();
            return config.BaseUrl;
        }

        /// <summary>
        /// Gets the timeout for a specific endpoint (or default)
        /// </summary>
        public int GetTimeout(string endpointName = null)
        {
            var config = GetCurrentServiceConfig();

            if (!string.IsNullOrWhiteSpace(endpointName))
            {
                var endpoint = GetEndpoint(endpointName);
                if (endpoint.Timeout.HasValue)
                {
                    return endpoint.Timeout.Value;
                }
            }

            return config.Timeout;
        }

        /// <summary>
        /// Gets default headers for the current API service
        /// </summary>
        public Dictionary<string, string> GetDefaultHeaders()
        {
            var config = GetCurrentServiceConfig();
            return config.DefaultHeaders ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets authentication configuration
        /// </summary>
        public AuthenticationConfig GetAuthentication()
        {
            var config = GetCurrentServiceConfig();
            return config.Authentication;
        }

        /// <summary>
        /// Checks if an API service has been initialized
        /// </summary>
        public bool IsServiceInitialized()
        {
            return _context.ContainsKey(CURRENT_API_SERVICE_KEY);
        }
    }
}
