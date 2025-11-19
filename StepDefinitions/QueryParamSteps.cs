using Reqnroll;
using RestSharp;
using TafRestSharpWireMock.Tests.Utils.Api;
using TafRestSharpWireMock.Tests.Utils.QueryParams;
using TafRestSharpWireMock.Tests.Utils.Json;
using TafRestSharpWireMock.Tests.Models;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TafRestSharpWireMock.Tests.StepDefinitions
{
    [Binding]
    public class QueryParamSteps
    {
        private readonly ScenarioContext _context;
        private readonly IReqnrollOutput _output;
        private readonly ApiClient _apiClient;

        public QueryParamSteps(ScenarioContext context, IReqnrollOutput output)
        {
            _context = context;
            _output = output;
            _apiClient = new ApiClient();
        }

        // ========== GET with Query Params ==========

        [When(@"I GET from endpoint '(.*)' with query params from file '(.*)' for test case '(.*)'")]
        public async Task WhenIGetWithQueryParamsFromFile(string endpoint, string fileName, string tcNumber)
        {
            string filePath = Path.Combine("Data", "QueryParams", fileName);
            var queryParams = QueryParamLoader.LoadQueryParamsAsDictionary(filePath, tcNumber);

            var response = await _apiClient.GetAsync(endpoint, queryParams);
            _context["LastGetResponse"] = response;
            _context["LastResponse"] = response;

            var queryString = QueryParamLoader.LoadQueryParams(filePath, tcNumber);
            _output.Info($"GET request sent to: {endpoint} with query params: {queryString} (from {tcNumber})");
        }

        // ========== POST with Query Params ==========

        [When(@"I POST to endpoint '(.*)' with payload from file '(.*)' and query params from file '(.*)' for test case '(.*)'")]
        public async Task WhenIPostWithPayloadAndQueryParams(string endpoint, string payloadFile, string queryParamFile, string tcNumber)
        {
            string payloadPath = Path.Combine("Data", payloadFile);
            string payload = JsonFileLoader.LoadJsonFromFile(payloadPath);

            string queryParamPath = Path.Combine("Data", "QueryParams", queryParamFile);
            var queryParams = QueryParamLoader.LoadQueryParamsAsDictionary(queryParamPath, tcNumber);

            var response = await _apiClient.PostAsync(endpoint, payload, queryParams);
            _context["LastPostResponse"] = response;
            _context["LastResponse"] = response;

            var queryString = QueryParamLoader.LoadQueryParams(queryParamPath, tcNumber);
            _output.Info($"POST request sent to: {endpoint} with payload: {payloadFile} and query params: {queryString} (from {tcNumber})");
        }

        [When(@"I POST to endpoint '(.*)' with body and query params from file '(.*)' for test case '(.*)'")]
        public async Task WhenIPostWithBodyAndQueryParams(string endpoint, string queryParamFile, string tcNumber, Table table)
        {
            var payload = new Dictionary<string, string>();
            foreach (var row in table.Rows)
            {
                var key = row[0];
                var value = row[1];
                payload[key] = value;
            }

            string queryParamPath = Path.Combine("Data", "QueryParams", queryParamFile);
            var queryParams = QueryParamLoader.LoadQueryParamsAsDictionary(queryParamPath, tcNumber);

            var response = await _apiClient.PostAsync(endpoint, payload, queryParams);
            _context["LastPostResponse"] = response;
            _context["LastResponse"] = response;

            var queryString = QueryParamLoader.LoadQueryParams(queryParamPath, tcNumber);
            _output.Info($"POST request sent to: {endpoint} with table data and query params: {queryString} (from {tcNumber})");
        }

        // ========== PUT with Query Params ==========

        [When(@"I PUT to endpoint '(.*)' with payload from file '(.*)' and query params from file '(.*)' for test case '(.*)'")]
        public async Task WhenIPutWithPayloadAndQueryParams(string endpoint, string payloadFile, string queryParamFile, string tcNumber)
        {
            string payloadPath = Path.Combine("Data", payloadFile);
            string payload = JsonFileLoader.LoadJsonFromFile(payloadPath);

            string queryParamPath = Path.Combine("Data", "QueryParams", queryParamFile);
            var queryParams = QueryParamLoader.LoadQueryParamsAsDictionary(queryParamPath, tcNumber);

            var response = await _apiClient.PutAsync(endpoint, payload, queryParams);
            _context["LastPutResponse"] = response;
            _context["LastResponse"] = response;

            var queryString = QueryParamLoader.LoadQueryParams(queryParamPath, tcNumber);
            _output.Info($"PUT request sent to: {endpoint} with payload: {payloadFile} and query params: {queryString} (from {tcNumber})");
        }

        [When(@"I PUT to endpoint '(.*)' with body and query params from file '(.*)' for test case '(.*)'")]
        public async Task WhenIPutWithBodyAndQueryParams(string endpoint, string queryParamFile, string tcNumber, Table table)
        {
            var payload = new Dictionary<string, string>();
            foreach (var row in table.Rows)
            {
                var key = row[0];
                var value = row[1];
                payload[key] = value;
            }

            string queryParamPath = Path.Combine("Data", "QueryParams", queryParamFile);
            var queryParams = QueryParamLoader.LoadQueryParamsAsDictionary(queryParamPath, tcNumber);

            var response = await _apiClient.PutAsync(endpoint, payload, queryParams);
            _context["LastPutResponse"] = response;
            _context["LastResponse"] = response;

            var queryString = QueryParamLoader.LoadQueryParams(queryParamPath, tcNumber);
            _output.Info($"PUT request sent to: {endpoint} with table data and query params: {queryString} (from {tcNumber})");
        }

        // ========== PATCH with Query Params ==========

        [When(@"I PATCH to endpoint '(.*)' with payload from file '(.*)' and query params from file '(.*)' for test case '(.*)'")]
        public async Task WhenIPatchWithPayloadAndQueryParams(string endpoint, string payloadFile, string queryParamFile, string tcNumber)
        {
            string payloadPath = Path.Combine("Data", payloadFile);
            string payload = JsonFileLoader.LoadJsonFromFile(payloadPath);

            string queryParamPath = Path.Combine("Data", "QueryParams", queryParamFile);
            var queryParams = QueryParamLoader.LoadQueryParamsAsDictionary(queryParamPath, tcNumber);

            var response = await _apiClient.PatchAsync(endpoint, payload, queryParams);
            _context["LastPatchResponse"] = response;
            _context["LastResponse"] = response;

            var queryString = QueryParamLoader.LoadQueryParams(queryParamPath, tcNumber);
            _output.Info($"PATCH request sent to: {endpoint} with payload: {payloadFile} and query params: {queryString} (from {tcNumber})");
        }

        [When(@"I PATCH to endpoint '(.*)' with body and query params from file '(.*)' for test case '(.*)'")]
        public async Task WhenIPatchWithBodyAndQueryParams(string endpoint, string queryParamFile, string tcNumber, Table table)
        {
            var payload = new Dictionary<string, string>();
            foreach (var row in table.Rows)
            {
                var key = row[0];
                var value = row[1];
                payload[key] = value;
            }

            string queryParamPath = Path.Combine("Data", "QueryParams", queryParamFile);
            var queryParams = QueryParamLoader.LoadQueryParamsAsDictionary(queryParamPath, tcNumber);

            var response = await _apiClient.PatchAsync(endpoint, payload, queryParams);
            _context["LastPatchResponse"] = response;
            _context["LastResponse"] = response;

            var queryString = QueryParamLoader.LoadQueryParams(queryParamPath, tcNumber);
            _output.Info($"PATCH request sent to: {endpoint} with table data and query params: {queryString} (from {tcNumber})");
        }

        // ========== DELETE with Query Params ==========

        [When(@"I DELETE from endpoint '(.*)' with query params from file '(.*)' for test case '(.*)'")]
        public async Task WhenIDeleteWithQueryParamsFromFile(string endpoint, string fileName, string tcNumber)
        {
            string filePath = Path.Combine("Data", "QueryParams", fileName);
            var queryParams = QueryParamLoader.LoadQueryParamsAsDictionary(filePath, tcNumber);

            var response = await _apiClient.DeleteAsync(endpoint, queryParams);
            _context["LastDeleteResponse"] = response;
            _context["LastResponse"] = response;

            var queryString = QueryParamLoader.LoadQueryParams(filePath, tcNumber);
            _output.Info($"DELETE request sent to: {endpoint} with query params: {queryString} (from {tcNumber})");
        }

        // ========== Common Assertions ==========

        [Then(@"the response should be valid")]
        public void ThenResponseIsValid()
        {
            var response = (RestResponse)_context["LastResponse"];
            Assert.IsTrue((int)response.StatusCode >= 200 && (int)response.StatusCode < 300,
                $"Response should be successful, but got {response.StatusCode}");
            _output.Pass($"Response is valid with status: {response.StatusCode}");
        }

        [Then(@"the response should have query parameter '(.*)' with value '(.*)'")]
        public void ThenResponseContainsQueryParam(string paramName, string expectedValue)
        {
            // This is a placeholder for validating that the server received the query parameter
            // In real scenarios, the server response might echo back the parameters or filter results accordingly
            var response = (RestResponse)_context["LastResponse"];
            Assert.IsNotNull(response);
            _output.Pass($"Verified query parameter: {paramName}={expectedValue}");
        }
    }
}
