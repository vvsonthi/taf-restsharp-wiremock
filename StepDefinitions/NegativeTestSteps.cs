using Reqnroll;
using RestSharp;
using TafRestSharpWireMock.Tests.Utils.Api;
using TafRestSharpWireMock.Tests.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TafRestSharpWireMock.Tests.Utils.Configuration;

namespace TafRestSharpWireMock.Tests.StepDefinitions
{
    [Binding]
    public class NegativeTestSteps
    {
        private readonly ScenarioContext _context;
        private readonly IReqnrollOutput _output;
        private readonly ApiClient _apiClient;

        public NegativeTestSteps(ScenarioContext context, IReqnrollOutput output)
        {
            _context = context;
            _output = output;
            _apiClient = new ApiClient();
        }

        [When(@"I POST empty payload to endpoint '(.*)'")]
        public async Task WhenIPostEmptyPayload(string endpoint)
        {
            var payload = new { };
            var response = await _apiClient.PostAsync(endpoint, payload);
            _context["LastPostResponse"] = response;
            _output.Info($"POST request with empty payload sent to: {endpoint}");
        }

        [When(@"I POST invalid JSON to endpoint '(.*)'")]
        public async Task WhenIPostInvalidJson(string endpoint)
        {
            var config = ConfigurationManager.Instance;
            var baseUrl = config.UseWireMock
                ? $"http://localhost:{config.WireMockPort}"
                : config.UserApiBaseUrl;

            var client = new RestClient(baseUrl);
            var request = new RestRequest(endpoint, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddStringBody("{invalid-json-format", DataFormat.Json);

            var response = await client.ExecuteAsync(request);
            _context["LastPostResponse"] = response;
            _output.Info($"POST request with invalid JSON sent to: {endpoint}");
        }

        [When(@"I POST payload with only name '(.*)' to endpoint '(.*)'")]
        public async Task WhenIPostPayloadWithOnlyName(string name, string endpoint)
        {
            var payload = new { name = name };
            var response = await _apiClient.PostAsync(endpoint, payload);
            _context["LastPostResponse"] = response;
            _output.Info($"POST request with only name field sent to: {endpoint}");
        }

        [When(@"I POST payload with name '(.*)' and job '(.*)' to endpoint '(.*)'")]
        public async Task WhenIPostPayloadWithNameAndJob(string name, string job, string endpoint)
        {
            var payload = new CreateUserRequest
            {
                Name = name,
                Job = job
            };

            var response = await _apiClient.PostAsync(endpoint, payload);
            _context["LastPostResponse"] = response;
            _output.Info($"POST request sent to: {endpoint}");
        }

        [When(@"I PATCH empty payload to endpoint '(.*)'")]
        public async Task WhenIPatchEmptyPayload(string endpoint)
        {
            var payload = new { };
            var response = await _apiClient.PatchAsync(endpoint, payload);
            _context["LastPatchResponse"] = response;
            _output.Info($"PATCH request with empty payload sent to: {endpoint}");
        }

        [When(@"I send unauthenticated GET request to endpoint '(.*)'")]
        public async Task WhenISendUnauthenticatedGetRequest(string endpoint)
        {
            var config = ConfigurationManager.Instance;
            var baseUrl = config.UseWireMock
                ? $"http://localhost:{config.WireMockPort}"
                : config.UserApiBaseUrl;

            // Create a client without authentication
            var client = new RestClient(baseUrl);
            var request = new RestRequest(endpoint, Method.Get);

            var response = await client.ExecuteAsync(request);
            _context["LastResponse"] = response;
            _output.Info($"Unauthenticated GET request sent to: {endpoint}");
        }

        [Then(@"the response should have status (.*)")]
        public void ThenResponseStatus(int expectedStatus)
        {
            var response = (RestResponse)_context["LastResponse"];
            Assert.AreEqual(expectedStatus, (int)response.StatusCode);
            _output.Pass($"Response status validated: {expectedStatus}");
        }
    }
}
