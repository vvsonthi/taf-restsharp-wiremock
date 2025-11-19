using Reqnroll;
using RestSharp;
using TafRestSharpWireMock.Tests.Utils.Api;
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
    public class UnifiedRestSteps
    {
        private readonly ScenarioContext _context;
        private readonly IReqnrollOutput _output;
        private readonly ApiClient _apiClient;

        public UnifiedRestSteps(ScenarioContext context, IReqnrollOutput output)
        {
            _context = context;
            _output = output;
            _apiClient = new ApiClient();
        }

        // ========== GET Request ==========

        [When(@"I create a GET request with test case ""(.*)"" uri params ""(.*)"" query params ""(.*)"" headers ""(.*)"" with payload (.*)")]
        public async Task WhenICreateGetRequest(string tcNo, string uriParams, string queryParamsFile, string headers, string payload)
        {
            var endpoint = BuildEndpoint(uriParams);
            var queryParams = LoadQueryParams(queryParamsFile, tcNo);
            var headerDict = HeaderLoader.ParseHeaderString(headers);

            var response = await ExecuteGetRequest(endpoint, queryParams, headerDict);
            StoreResponse(response);

            _output.Info($"GET {tcNo}: {endpoint} | Query: {queryParamsFile} | Headers: {headers}");
        }

        // ========== POST Request ==========

        [When(@"I create a POST request with test case ""(.*)"" uri params ""(.*)"" query params ""(.*)"" headers ""(.*)"" with payload (.*)")]
        public async Task WhenICreatePostRequest(string tcNo, string uriParams, string queryParamsFile, string headers, string payload)
        {
            var endpoint = BuildEndpoint(uriParams);
            var queryParams = LoadQueryParams(queryParamsFile, tcNo);
            var headerDict = HeaderLoader.ParseHeaderString(headers);
            var body = LoadPayload(payload);

            var response = await ExecutePostRequest(endpoint, body, queryParams, headerDict);
            StoreResponse(response);

            _output.Info($"POST {tcNo}: {endpoint} | Payload: {payload} | Query: {queryParamsFile}");
        }

        // ========== PUT Request ==========

        [When(@"I create a PUT request with test case ""(.*)"" uri params ""(.*)"" query params ""(.*)"" headers ""(.*)"" with payload (.*)")]
        public async Task WhenICreatePutRequest(string tcNo, string uriParams, string queryParamsFile, string headers, string payload)
        {
            var endpoint = BuildEndpoint(uriParams);
            var queryParams = LoadQueryParams(queryParamsFile, tcNo);
            var headerDict = HeaderLoader.ParseHeaderString(headers);
            var body = LoadPayload(payload);

            var response = await ExecutePutRequest(endpoint, body, queryParams, headerDict);
            StoreResponse(response);

            _output.Info($"PUT {tcNo}: {endpoint} | Payload: {payload}");
        }

        // ========== PATCH Request ==========

        [When(@"I create a PATCH request with test case ""(.*)"" uri params ""(.*)"" query params ""(.*)"" headers ""(.*)"" with payload (.*)")]
        public async Task WhenICreatePatchRequest(string tcNo, string uriParams, string queryParamsFile, string headers, string payload)
        {
            var endpoint = BuildEndpoint(uriParams);
            var queryParams = LoadQueryParams(queryParamsFile, tcNo);
            var headerDict = HeaderLoader.ParseHeaderString(headers);
            var body = LoadPayload(payload);

            var response = await ExecutePatchRequest(endpoint, body, queryParams, headerDict);
            StoreResponse(response);

            _output.Info($"PATCH {tcNo}: {endpoint} | Payload: {payload}");
        }

        // ========== DELETE Request ==========

        [When(@"I create a DELETE request with test case ""(.*)"" uri params ""(.*)"" query params ""(.*)"" headers ""(.*)"" with payload (.*)")]
        public async Task WhenICreateDeleteRequest(string tcNo, string uriParams, string queryParamsFile, string headers, string payload)
        {
            var endpoint = BuildEndpoint(uriParams);
            var queryParams = LoadQueryParams(queryParamsFile, tcNo);
            var headerDict = HeaderLoader.ParseHeaderString(headers);

            var response = await ExecuteDeleteRequest(endpoint, queryParams, headerDict);
            StoreResponse(response);

            _output.Info($"DELETE {tcNo}: {endpoint} | Query: {queryParamsFile}");
        }

        // ========== Response Status Assertions ==========

        [Then(@"I receive a response with HTTP status code ""(.*)"" with status text ""(.*)""")]
        public void ThenIReceiveResponseWithStatus(string statusCode, string statusText)
        {
            var response = GetLastResponse();

            Assert.AreEqual(int.Parse(statusCode), (int)response.StatusCode,
                $"Expected status code {statusCode}, but got {(int)response.StatusCode}");

            _output.Pass($"Response status: {statusCode} {statusText}");
        }

        // ========== Header Assertions ==========

        [Then(@"I confirm that the expected headers ""(.*)"" are present")]
        public void ThenIConfirmExpectedHeadersPresent(string expectedHeaders)
        {
            if (expectedHeaders.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                _output.Info("Skipping header validation (none specified)");
                return;
            }

            var response = GetLastResponse();
            var actualHeaders = response.Headers.ToDictionary(h => h.Name, h => h.Value?.ToString() ?? string.Empty);

            var missingHeaders = HeaderLoader.GetMissingHeaders(actualHeaders, expectedHeaders);

            Assert.AreEqual(0, missingHeaders.Count,
                $"Missing expected headers: {string.Join(", ", missingHeaders)}");

            _output.Pass($"All expected headers present: {expectedHeaders}");
        }

        // ========== Field Validation ==========

        [Then(@"I verify that the field ""(.*)"" in the response is ""(.*)""")]
        public void ThenIVerifyFieldInResponse(string fieldName, string fieldContents)
        {
            if (fieldName.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                _output.Info("Skipping field validation (none specified)");
                return;
            }

            var response = GetLastResponse();
            var json = JToken.Parse(response.Content);

            FieldValidator.ValidateField(json, fieldName, fieldContents);

            _output.Pass($"Field '{fieldName}' validated: {fieldContents}");
        }

        // ========== Key-Value Validation ==========

        [Then(@"the response should contain the expected ""(.*)"" text ""(.*)""")]
        public void ThenResponseShouldContainExpectedText(string expectedKey, string expectedValue)
        {
            if (expectedKey.Equals("none", StringComparison.OrdinalIgnoreCase) ||
                expectedValue.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                _output.Info("Skipping key-value validation (none specified)");
                return;
            }

            var response = GetLastResponse();
            var json = JToken.Parse(response.Content);

            FieldValidator.ValidateResponseContainsKeyValue(json, expectedKey, expectedValue);

            _output.Pass($"Response contains '{expectedKey}': '{expectedValue}'");
        }

        // ========== Helper Methods ==========

        private string BuildEndpoint(string uriParams)
        {
            if (string.IsNullOrWhiteSpace(uriParams) || uriParams.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                return "/api";
            }

            // If it's a full path like "/api/users", return as-is
            if (uriParams.StartsWith("/"))
            {
                return uriParams;
            }

            // If it's just "users" or "users/2", prepend /api/
            return $"/api/{uriParams}";
        }

        private Dictionary<string, string> LoadQueryParams(string queryParamsInput, string tcNo)
        {
            // Auto-detect: file (.txt), inline (page=1&status=active), or none
            return QueryParamLoader.LoadQueryParamsAuto(queryParamsInput, tcNo);
        }

        private string LoadPayload(string payload)
        {
            if (string.IsNullOrWhiteSpace(payload) || payload.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                return "{}";
            }

            // If it ends with .json, load from file
            if (payload.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                string filePath = Path.Combine("Data", payload);
                return JsonFileLoader.LoadJsonFromFile(filePath);
            }

            // Otherwise treat as inline JSON
            return payload;
        }

        private async Task<RestResponse> ExecuteGetRequest(string endpoint, Dictionary<string, string> queryParams, Dictionary<string, string> headers)
        {
            RestResponse response;

            if (queryParams.Count > 0)
            {
                response = await _apiClient.GetAsync(endpoint, queryParams);
            }
            else
            {
                response = await _apiClient.GetAsync(endpoint);
            }

            return response;
        }

        private async Task<RestResponse> ExecutePostRequest(string endpoint, string body, Dictionary<string, string> queryParams, Dictionary<string, string> headers)
        {
            RestResponse response;

            if (queryParams.Count > 0)
            {
                response = await _apiClient.PostAsync(endpoint, body, queryParams);
            }
            else
            {
                response = await _apiClient.PostAsync(endpoint, body);
            }

            return response;
        }

        private async Task<RestResponse> ExecutePutRequest(string endpoint, string body, Dictionary<string, string> queryParams, Dictionary<string, string> headers)
        {
            RestResponse response;

            if (queryParams.Count > 0)
            {
                response = await _apiClient.PutAsync(endpoint, body, queryParams);
            }
            else
            {
                response = await _apiClient.PutAsync(endpoint, body);
            }

            return response;
        }

        private async Task<RestResponse> ExecutePatchRequest(string endpoint, string body, Dictionary<string, string> queryParams, Dictionary<string, string> headers)
        {
            RestResponse response;

            if (queryParams.Count > 0)
            {
                response = await _apiClient.PatchAsync(endpoint, body, queryParams);
            }
            else
            {
                response = await _apiClient.PatchAsync(endpoint, body);
            }

            return response;
        }

        private async Task<RestResponse> ExecuteDeleteRequest(string endpoint, Dictionary<string, string> queryParams, Dictionary<string, string> headers)
        {
            RestResponse response;

            if (queryParams.Count > 0)
            {
                response = await _apiClient.DeleteAsync(endpoint, queryParams);
            }
            else
            {
                response = await _apiClient.DeleteAsync(endpoint);
            }

            return response;
        }

        private void StoreResponse(RestResponse response)
        {
            _context["LastResponse"] = response;
            _context["LastGetResponse"] = response; // For compatibility
            _context["LastPostResponse"] = response;
            _context["LastPutResponse"] = response;
            _context["LastPatchResponse"] = response;
            _context["LastDeleteResponse"] = response;
        }

        private RestResponse GetLastResponse()
        {
            if (!_context.ContainsKey("LastResponse"))
            {
                throw new InvalidOperationException("No response found in context. Make sure to execute a request first.");
            }

            return (RestResponse)_context["LastResponse"];
        }
    }
}
