using Reqnroll;
using RestSharp;
using TafRestSharpWireMock.Tests.Utils.Api;
using TafRestSharpWireMock.Tests.Utils.Json;
using TafRestSharpWireMock.Tests.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TafRestSharpWireMock.Tests.StepDefinitions
{
    [Binding]
    public class PatchSteps
    {
        private readonly ScenarioContext _context;
        private readonly IReqnrollOutput _output;
        private readonly ApiClient _apiClient;

        public PatchSteps(ScenarioContext context, IReqnrollOutput output)
        {
            _context = context;
            _output = output;
            _apiClient = new ApiClient();
        }

        [When(@"I PATCH the following payload to endpoint '(.*)'")]
        public async Task WhenIPatchPayload(string endpoint, Table table)
        {
            var payload = new Dictionary<string, string>();
            foreach (var row in table.Rows)
            {
                var key = row[0];
                var value = row[1];
                payload[key] = value;
            }

            var response = await _apiClient.PatchAsync(endpoint, payload);
            _context["LastPatchResponse"] = response;
            _output.Info($"PATCH request sent to: {endpoint}");
        }

        [When(@"I PATCH field '(.*)' with value '(.*)' to endpoint '(.*)'")]
        public async Task WhenIPatchField(string fieldName, string fieldValue, string endpoint)
        {
            var payload = new Dictionary<string, string>
            {
                { fieldName, fieldValue }
            };

            var response = await _apiClient.PatchAsync(endpoint, payload);
            _context["LastPatchResponse"] = response;
            _output.Info($"PATCH request sent to: {endpoint} with {fieldName}={fieldValue}");
        }

        [When(@"I PATCH the payload from file '(.*)' to endpoint '(.*)'")]
        public async Task WhenIPatchPayloadFromFile(string fileName, string endpoint)
        {
            string filePath = System.IO.Path.Combine("Data", fileName);
            string payload = JsonFileLoader.LoadJsonFromFile(filePath);

            var response = await _apiClient.PatchAsync(endpoint, payload);
            _context["LastPatchResponse"] = response;
            _output.Info($"PATCH request sent with payload from: {fileName}");
        }

        [Then(@"the PATCH response should have status (.*)")]
        public void ThenPatchResponseStatus(int expectedStatus)
        {
            var response = (RestResponse)_context["LastPatchResponse"];
            Assert.AreEqual(expectedStatus, (int)response.StatusCode);
            _output.Pass($"PATCH response status validated: {expectedStatus}");
        }

        [Then(@"the PATCH response should contain field '(.*)' with value '(.*)'")]
        public void ThenPatchResponseContainsField(string fieldName, string expectedValue)
        {
            var response = (RestResponse)_context["LastPatchResponse"];
            var jsonObject = JObject.Parse(response.Content);

            Assert.IsTrue(jsonObject.ContainsKey(fieldName), $"Response should contain field: {fieldName}");
            Assert.AreEqual(expectedValue, jsonObject[fieldName].ToString());
            _output.Pass($"PATCH response contains {fieldName}: {expectedValue}");
        }

        [Then(@"the PATCH response should have field '(.*)'")]
        public void ThenPatchResponseHasField(string fieldName)
        {
            var response = (RestResponse)_context["LastPatchResponse"];
            var jsonObject = JObject.Parse(response.Content);

            Assert.IsTrue(jsonObject.ContainsKey(fieldName), $"Response should contain field: {fieldName}");
            Assert.IsFalse(string.IsNullOrEmpty(jsonObject[fieldName].ToString()),
                $"Field {fieldName} should not be empty");
            _output.Pass($"PATCH response has field: {fieldName}");
        }

        [Then(@"the PATCH response should have valid update schema")]
        public void ThenPatchResponseHasValidUpdateSchema()
        {
            var response = (RestResponse)_context["LastPatchResponse"];
            var jsonObject = JObject.Parse(response.Content);

            Assert.IsTrue(jsonObject.ContainsKey("updatedAt"), "Response should contain updatedAt field");
            Assert.IsFalse(string.IsNullOrEmpty(jsonObject["updatedAt"].ToString()),
                "UpdatedAt should not be empty");

            _output.Pass("PATCH response has valid update schema");
        }
    }
}
