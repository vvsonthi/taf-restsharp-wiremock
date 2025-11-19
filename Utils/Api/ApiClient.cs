using RestSharp;
using TafRestSharpWireMock.Tests.Utils.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TafRestSharpWireMock.Tests.Utils.Api
{
    public class ApiClient
    {
        private readonly RestClient _client;
        private readonly string _apiKey;

        public ApiClient()
        {
            var config = ConfigurationManager.Instance;
            var baseUrl = config.UseWireMock
                ? $"http://localhost:{config.WireMockPort}"
                : config.UserApiBaseUrl;

            var options = new RestClientOptions(baseUrl)
            {
                MaxTimeout = config.UserApiTimeout
            };

            _client = new RestClient(options);
            _apiKey = config.GetApiKey();
        }

        public ApiClient(string customBaseUrl, int timeout = 8000)
        {
            var options = new RestClientOptions(customBaseUrl)
            {
                MaxTimeout = timeout
            };

            _client = new RestClient(options);
            _apiKey = ConfigurationManager.Instance.GetApiKey();
        }

        private RestRequest CreateRequest(string endpoint, Method method)
        {
            var request = new RestRequest(endpoint, method);

            // Add API key if configured
            if (!string.IsNullOrEmpty(_apiKey))
            {
                request.AddHeader("X-API-Key", _apiKey);
            }

            return request;
        }

        private void AddQueryParameters(RestRequest request, Dictionary<string, string> queryParams)
        {
            if (queryParams != null && queryParams.Count > 0)
            {
                foreach (var param in queryParams)
                {
                    request.AddQueryParameter(param.Key, param.Value);
                }
            }
        }

        public async Task<RestResponse> GetAsync(string endpoint)
        {
            var request = CreateRequest(endpoint, Method.Get);
            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse<T>> GetAsync<T>(string endpoint)
        {
            var request = CreateRequest(endpoint, Method.Get);
            return await _client.ExecuteAsync<T>(request);
        }

        public async Task<RestResponse> PostAsync(string endpoint, object body)
        {
            var request = CreateRequest(endpoint, Method.Post);
            request.AddJsonBody(body);
            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse> PostAsync(string endpoint, string jsonBody)
        {
            var request = CreateRequest(endpoint, Method.Post);
            request.AddStringBody(jsonBody, DataFormat.Json);
            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse<T>> PostAsync<T>(string endpoint, object body)
        {
            var request = CreateRequest(endpoint, Method.Post);
            request.AddJsonBody(body);
            return await _client.ExecuteAsync<T>(request);
        }

        public async Task<RestResponse> PutAsync(string endpoint, object body)
        {
            var request = CreateRequest(endpoint, Method.Put);
            request.AddJsonBody(body);
            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse<T>> PutAsync<T>(string endpoint, object body)
        {
            var request = CreateRequest(endpoint, Method.Put);
            request.AddJsonBody(body);
            return await _client.ExecuteAsync<T>(request);
        }

        public async Task<RestResponse> PatchAsync(string endpoint, object body)
        {
            var request = CreateRequest(endpoint, Method.Patch);
            request.AddJsonBody(body);
            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse<T>> PatchAsync<T>(string endpoint, object body)
        {
            var request = CreateRequest(endpoint, Method.Patch);
            request.AddJsonBody(body);
            return await _client.ExecuteAsync<T>(request);
        }

        public async Task<RestResponse> DeleteAsync(string endpoint)
        {
            var request = CreateRequest(endpoint, Method.Delete);
            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse<T>> DeleteAsync<T>(string endpoint)
        {
            var request = CreateRequest(endpoint, Method.Delete);
            return await _client.ExecuteAsync<T>(request);
        }

        public async Task<RestResponse> ExecuteAsync(RestRequest request)
        {
            return await _client.ExecuteAsync(request);
        }

        // ========== Methods with Query Parameter Support ==========

        public async Task<RestResponse> GetAsync(string endpoint, Dictionary<string, string> queryParams)
        {
            var request = CreateRequest(endpoint, Method.Get);
            AddQueryParameters(request, queryParams);
            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse<T>> GetAsync<T>(string endpoint, Dictionary<string, string> queryParams)
        {
            var request = CreateRequest(endpoint, Method.Get);
            AddQueryParameters(request, queryParams);
            return await _client.ExecuteAsync<T>(request);
        }

        public async Task<RestResponse> PostAsync(string endpoint, object body, Dictionary<string, string> queryParams)
        {
            var request = CreateRequest(endpoint, Method.Post);
            AddQueryParameters(request, queryParams);
            request.AddJsonBody(body);
            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse> PostAsync(string endpoint, string jsonBody, Dictionary<string, string> queryParams)
        {
            var request = CreateRequest(endpoint, Method.Post);
            AddQueryParameters(request, queryParams);
            request.AddStringBody(jsonBody, DataFormat.Json);
            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse<T>> PostAsync<T>(string endpoint, object body, Dictionary<string, string> queryParams)
        {
            var request = CreateRequest(endpoint, Method.Post);
            AddQueryParameters(request, queryParams);
            request.AddJsonBody(body);
            return await _client.ExecuteAsync<T>(request);
        }

        public async Task<RestResponse> PutAsync(string endpoint, object body, Dictionary<string, string> queryParams)
        {
            var request = CreateRequest(endpoint, Method.Put);
            AddQueryParameters(request, queryParams);
            request.AddJsonBody(body);
            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse<T>> PutAsync<T>(string endpoint, object body, Dictionary<string, string> queryParams)
        {
            var request = CreateRequest(endpoint, Method.Put);
            AddQueryParameters(request, queryParams);
            request.AddJsonBody(body);
            return await _client.ExecuteAsync<T>(request);
        }

        public async Task<RestResponse> PatchAsync(string endpoint, object body, Dictionary<string, string> queryParams)
        {
            var request = CreateRequest(endpoint, Method.Patch);
            AddQueryParameters(request, queryParams);
            request.AddJsonBody(body);
            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse<T>> PatchAsync<T>(string endpoint, object body, Dictionary<string, string> queryParams)
        {
            var request = CreateRequest(endpoint, Method.Patch);
            AddQueryParameters(request, queryParams);
            request.AddJsonBody(body);
            return await _client.ExecuteAsync<T>(request);
        }

        public async Task<RestResponse> DeleteAsync(string endpoint, Dictionary<string, string> queryParams)
        {
            var request = CreateRequest(endpoint, Method.Delete);
            AddQueryParameters(request, queryParams);
            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse<T>> DeleteAsync<T>(string endpoint, Dictionary<string, string> queryParams)
        {
            var request = CreateRequest(endpoint, Method.Delete);
            AddQueryParameters(request, queryParams);
            return await _client.ExecuteAsync<T>(request);
        }
    }
}
