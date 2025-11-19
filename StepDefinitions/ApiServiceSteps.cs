using Reqnroll;
using RestSharp;
using TafRestSharpWireMock.Tests.Utils.Api;
using TafRestSharpWireMock.Tests.Utils.ApiServices;
using TafRestSharpWireMock.Tests.Utils.QueryParams;
using TafRestSharpWireMock.Tests.Utils.Headers;
using TafRestSharpWireMock.Tests.Utils.Json;
using TafRestSharpWireMock.Tests.Utils.Validation;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace TafRestSharpWireMock.Tests.StepDefinitions
{
    [Binding]
    public class ApiServiceSteps
    {
        private readonly ScenarioContext _context;
        private readonly IReqnrollOutput _output;
        private readonly ApiServiceManager _serviceManager;
        private ApiClient _apiClient;

        public ApiServiceSteps(ScenarioContext context, IReqnrollOutput output)
        {
            _context = context;
            _output = output;
            _serviceManager = new ApiServiceManager(context);
        }

        // ========== Given Steps - API Service Initialization ==========

        [Given(@"I have the ""(.*)"" service initialized")]
        public void GivenIHaveTheServiceInitialized(string apiName)
        {
            _serviceManager.InitializeService(apiName);

            // Create API client with the service's base URL and timeout
            var baseUrl = _serviceManager.GetBaseUrl();
            var timeout = _serviceManager.GetTimeout();
            _apiClient = new ApiClient(baseUrl, timeout);

            _output.Info($"Initialized API service: {apiName} | Base URL: {baseUrl} | Timeout: {timeout}ms");
        }

        // ========== When Steps - HTTP Requests with Endpoint Names ==========

        [When(@"I create a GET request to ""(.*)"" endpoint with test case ""(.*)"" query params ""(.*)"" headers ""(.*)""")]
        public async Task WhenICreateGetRequestToEndpoint(string endpointName, string tcNo, string queryParams, string headers)
        {
            var endpoint = BuildEndpointFromName(endpointName);
            var apiName = _serviceManager.GetCurrentServiceName();
            var queryParamsDict = QueryParamLoader.LoadQueryParamsAuto(queryParams, tcNo, apiName);
            var headersDict = MergeHeaders(HeaderLoader.ParseHeaderString(headers));

            var response = await ExecuteGetRequest(endpoint, queryParamsDict, headersDict);
            StoreResponse(response);

            _output.Info($"GET {tcNo}: {endpointName} -> {endpoint} | Query: {queryParams}");
        }

        [When(@"I create a POST request to ""(.*)"" endpoint with test case ""(.*)"" query params ""(.*)"" headers ""(.*)"" with payload (.*)")]
        public async Task WhenICreatePostRequestToEndpoint(string endpointName, string tcNo, string queryParams, string headers, string payload)
        {
            var endpoint = BuildEndpointFromName(endpointName);
            var apiName = _serviceManager.GetCurrentServiceName();
            var queryParamsDict = QueryParamLoader.LoadQueryParamsAuto(queryParams, tcNo, apiName);
            var headersDict = MergeHeaders(HeaderLoader.ParseHeaderString(headers));
            var body = LoadPayload(payload);

            var response = await ExecutePostRequest(endpoint, body, queryParamsDict, headersDict);
            StoreResponse(response);

            _output.Info($"POST {tcNo}: {endpointName} -> {endpoint} | Payload: {payload}");
        }

        [When(@"I create a PUT request to ""(.*)"" endpoint with test case ""(.*)"" query params ""(.*)"" headers ""(.*)"" with payload (.*)")]
        public async Task WhenICreatePutRequestToEndpoint(string endpointName, string tcNo, string queryParams, string headers, string payload)
        {
            var endpoint = BuildEndpointFromName(endpointName);
            var apiName = _serviceManager.GetCurrentServiceName();
            var queryParamsDict = QueryParamLoader.LoadQueryParamsAuto(queryParams, tcNo, apiName);
            var headersDict = MergeHeaders(HeaderLoader.ParseHeaderString(headers));
            var body = LoadPayload(payload);

            var response = await ExecutePutRequest(endpoint, body, queryParamsDict, headersDict);
            StoreResponse(response);

            _output.Info($"PUT {tcNo}: {endpointName} -> {endpoint}");
        }

        [When(@"I create a PATCH request to ""(.*)"" endpoint with test case ""(.*)"" query params ""(.*)"" headers ""(.*)"" with payload (.*)")]
        public async Task WhenICreatePatchRequestToEndpoint(string endpointName, string tcNo, string queryParams, string headers, string payload)
        {
            var endpoint = BuildEndpointFromName(endpointName);
            var apiName = _serviceManager.GetCurrentServiceName();
            var queryParamsDict = QueryParamLoader.LoadQueryParamsAuto(queryParams, tcNo, apiName);
            var headersDict = MergeHeaders(HeaderLoader.ParseHeaderString(headers));
            var body = LoadPayload(payload);

            var response = await ExecutePatchRequest(endpoint, body, queryParamsDict, headersDict);
            StoreResponse(response);

            _output.Info($"PATCH {tcNo}: {endpointName} -> {endpoint}");
        }

        [When(@"I create a DELETE request to ""(.*)"" endpoint with test case ""(.*)"" query params ""(.*)"" headers ""(.*)""")]
        public async Task WhenICreateDeleteRequestToEndpoint(string endpointName, string tcNo, string queryParams, string headers)
        {
            var endpoint = BuildEndpointFromName(endpointName);
            var apiName = _serviceManager.GetCurrentServiceName();
            var queryParamsDict = QueryParamLoader.LoadQueryParamsAuto(queryParams, tcNo, apiName);
            var headersDict = MergeHeaders(HeaderLoader.ParseHeaderString(headers));

            var response = await ExecuteDeleteRequest(endpoint, queryParamsDict, headersDict);
            StoreResponse(response);

            _output.Info($"DELETE {tcNo}: {endpointName} -> {endpoint}");
        }

        // ========== With Path Parameters ==========

        [When(@"I create a GET request to ""(.*)"" endpoint with path params ""(.*)"" test case ""(.*)"" query params ""(.*)"" headers ""(.*)""")]
        public async Task WhenICreateGetRequestWithPathParams(string endpointName, string pathParams, string tcNo, string queryParams, string headers)
        {
            var pathParamsDict = ParsePathParams(pathParams);
            var endpoint = BuildEndpointFromName(endpointName, pathParamsDict);
            var apiName = _serviceManager.GetCurrentServiceName();
            var queryParamsDict = QueryParamLoader.LoadQueryParamsAuto(queryParams, tcNo, apiName);
            var headersDict = MergeHeaders(HeaderLoader.ParseHeaderString(headers));

            var response = await ExecuteGetRequest(endpoint, queryParamsDict, headersDict);
            StoreResponse(response);

            _output.Info($"GET {tcNo}: {endpointName} -> {endpoint} | Path params: {pathParams}");
        }

        [When(@"I create a PUT request to ""(.*)"" endpoint with path params ""(.*)"" test case ""(.*)"" query params ""(.*)"" headers ""(.*)"" with payload (.*)")]
        public async Task WhenICreatePutRequestWithPathParams(string endpointName, string pathParams, string tcNo, string queryParams, string headers, string payload)
        {
            var pathParamsDict = ParsePathParams(pathParams);
            var endpoint = BuildEndpointFromName(endpointName, pathParamsDict);
            var apiName = _serviceManager.GetCurrentServiceName();
            var queryParamsDict = QueryParamLoader.LoadQueryParamsAuto(queryParams, tcNo, apiName);
            var headersDict = MergeHeaders(HeaderLoader.ParseHeaderString(headers));
            var body = LoadPayload(payload);

            var response = await ExecutePutRequest(endpoint, body, queryParamsDict, headersDict);
            StoreResponse(response);

            _output.Info($"PUT {tcNo}: {endpointName} -> {endpoint} | Path params: {pathParams}");
        }

        [When(@"I create a DELETE request to ""(.*)"" endpoint with path params ""(.*)"" test case ""(.*)"" query params ""(.*)"" headers ""(.*)""")]
        public async Task WhenICreateDeleteRequestWithPathParams(string endpointName, string pathParams, string tcNo, string queryParams, string headers)
        {
            var pathParamsDict = ParsePathParams(pathParams);
            var endpoint = BuildEndpointFromName(endpointName, pathParamsDict);
            var apiName = _serviceManager.GetCurrentServiceName();
            var queryParamsDict = QueryParamLoader.LoadQueryParamsAuto(queryParams, tcNo, apiName);
            var headersDict = MergeHeaders(HeaderLoader.ParseHeaderString(headers));

            var response = await ExecuteDeleteRequest(endpoint, queryParamsDict, headersDict);
            StoreResponse(response);

            _output.Info($"DELETE {tcNo}: {endpointName} -> {endpoint} | Path params: {pathParams}");
        }

        // ========== Helper Methods ==========

        private string BuildEndpointFromName(string endpointName, Dictionary<string, string> pathParams = null)
        {
            return _serviceManager.BuildEndpointUrl(endpointName, pathParams);
        }

        private Dictionary<string, string> ParsePathParams(string pathParams)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(pathParams) || pathParams.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                return result;
            }

            // Format: id=2 or id=2&categoryId=5
            var pairs = pathParams.Split('&');
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split('=');
                if (keyValue.Length == 2)
                {
                    result[keyValue[0].Trim()] = keyValue[1].Trim();
                }
            }

            return result;
        }

        private Dictionary<string, string> MergeHeaders(Dictionary<string, string> customHeaders)
        {
            var defaultHeaders = _serviceManager.GetDefaultHeaders();
            var mergedHeaders = new Dictionary<string, string>(defaultHeaders);

            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    mergedHeaders[header.Key] = header.Value;  // Custom headers override defaults
                }
            }

            return mergedHeaders;
        }

        private string LoadPayload(string payload)
        {
            if (string.IsNullOrWhiteSpace(payload) || payload.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                return "{}";
            }

            if (payload.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                // Get current API name to determine folder structure
                string apiName = _serviceManager.GetCurrentServiceName();

                // Build path: Data/Payloads/{ApiName}/{payload}
                string filePath = Path.Combine("Data", "Payloads", apiName, payload);
                return JsonFileLoader.LoadJsonFromFile(filePath);
            }

            return payload;
        }

        private async Task<RestResponse> ExecuteGetRequest(string endpoint, Dictionary<string, string> queryParams, Dictionary<string, string> headers)
        {
            if (queryParams.Count > 0)
            {
                return await _apiClient.GetAsync(endpoint, queryParams);
            }
            return await _apiClient.GetAsync(endpoint);
        }

        private async Task<RestResponse> ExecutePostRequest(string endpoint, string body, Dictionary<string, string> queryParams, Dictionary<string, string> headers)
        {
            if (queryParams.Count > 0)
            {
                return await _apiClient.PostAsync(endpoint, body, queryParams);
            }
            return await _apiClient.PostAsync(endpoint, body);
        }

        private async Task<RestResponse> ExecutePutRequest(string endpoint, string body, Dictionary<string, string> queryParams, Dictionary<string, string> headers)
        {
            if (queryParams.Count > 0)
            {
                return await _apiClient.PutAsync(endpoint, body, queryParams);
            }
            return await _apiClient.PutAsync(endpoint, body);
        }

        private async Task<RestResponse> ExecutePatchRequest(string endpoint, string body, Dictionary<string, string> queryParams, Dictionary<string, string> headers)
        {
            if (queryParams.Count > 0)
            {
                return await _apiClient.PatchAsync(endpoint, body, queryParams);
            }
            return await _apiClient.PatchAsync(endpoint, body);
        }

        private async Task<RestResponse> ExecuteDeleteRequest(string endpoint, Dictionary<string, string> queryParams, Dictionary<string, string> headers)
        {
            if (queryParams.Count > 0)
            {
                return await _apiClient.DeleteAsync(endpoint, queryParams);
            }
            return await _apiClient.DeleteAsync(endpoint);
        }

        private void StoreResponse(RestResponse response)
        {
            _context["LastResponse"] = response;
            _context["LastGetResponse"] = response;
            _context["LastPostResponse"] = response;
            _context["LastPutResponse"] = response;
            _context["LastPatchResponse"] = response;
            _context["LastDeleteResponse"] = response;
        }
    }
}
