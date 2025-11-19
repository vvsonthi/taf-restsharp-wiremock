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
    public class PutSteps
    {
        private readonly ScenarioContext _context;
        private readonly IReqnrollOutput _output;
        private readonly ApiClient _apiClient;

        public PutSteps(ScenarioContext context, IReqnrollOutput output)
        {
            _context = context;
            _output = output;
            _apiClient = new ApiClient();
        }

        [When(@"I PUT the following payload to endpoint '(.*)'")]
        public async Task WhenIPutPayload(string endpoint, Table table)
        {
            var payload = new Dictionary<string, string>();
            foreach (var row in table.Rows)
            {
                var key = row[0];
                var value = row[1];
                payload[key] = value;
            }

            var response = await _apiClient.PutAsync(endpoint, payload);
            _context["LastPutResponse"] = response;
            _output.Info($"PUT request sent to: {endpoint}");
        }

        [When(@"I PUT the payload from file '(.*)' to endpoint '(.*)'")]
        public async Task WhenIPutPayloadFromFile(string fileName, string endpoint)
        {
            string filePath = System.IO.Path.Combine("Data", fileName);
            string payload = JsonFileLoader.LoadJsonFromFile(filePath);

            var response = await _apiClient.PutAsync(endpoint, payload);
            _context["LastPutResponse"] = response;
            _output.Info($"PUT request sent with payload from: {fileName}");
        }

        [When(@"I PUT payload with name '(.*)' and job '(.*)' to endpoint '(.*)'")]
        public async Task WhenIPutPayloadWithNameAndJob(string name, string job, string endpoint)
        {
            var payload = new CreateUserRequest
            {
                Name = name,
                Job = job
            };

            var response = await _apiClient.PutAsync(endpoint, payload);
            _context["LastPutResponse"] = response;
            _output.Info($"PUT request sent to: {endpoint}");
        }

        [Then(@"the PUT response should have status (.*)")]
        public void ThenPutResponseStatus(int expectedStatus)
        {
            var response = (RestResponse)_context["LastPutResponse"];
            Assert.AreEqual(expectedStatus, (int)response.StatusCode);
            _output.Pass($"PUT response status validated: {expectedStatus}");
        }

        [Then(@"the PUT response should contain field '(.*)' with value '(.*)'")]
        public void ThenPutResponseContainsField(string fieldName, string expectedValue)
        {
            var response = (RestResponse)_context["LastPutResponse"];
            var jsonObject = JObject.Parse(response.Content);

            Assert.IsTrue(jsonObject.ContainsKey(fieldName), $"Response should contain field: {fieldName}");
            Assert.AreEqual(expectedValue, jsonObject[fieldName].ToString());
            _output.Pass($"PUT response contains {fieldName}: {expectedValue}");
        }

        [Then(@"the PUT response should have field '(.*)'")]
        public void ThenPutResponseHasField(string fieldName)
        {
            var response = (RestResponse)_context["LastPutResponse"];
            var jsonObject = JObject.Parse(response.Content);

            Assert.IsTrue(jsonObject.ContainsKey(fieldName), $"Response should contain field: {fieldName}");
            Assert.IsFalse(string.IsNullOrEmpty(jsonObject[fieldName].ToString()),
                $"Field {fieldName} should not be empty");
            _output.Pass($"PUT response has field: {fieldName}");
        }

        [Then(@"the PUT response should have valid update schema")]
        public void ThenPutResponseHasValidUpdateSchema()
        {
            var response = (RestResponse)_context["LastPutResponse"];
            var updateResponse = JsonConvert.DeserializeObject<UpdateUserResponse>(response.Content);

            Assert.IsNotNull(updateResponse, "UpdateUserResponse should not be null");
            Assert.IsFalse(string.IsNullOrEmpty(updateResponse.Name), "Name should not be empty");
            Assert.IsFalse(string.IsNullOrEmpty(updateResponse.Job), "Job should not be empty");
            Assert.IsFalse(string.IsNullOrEmpty(updateResponse.UpdatedAt), "UpdatedAt should not be empty");

            _output.Pass("PUT response has valid update schema");
        }
    }
}
