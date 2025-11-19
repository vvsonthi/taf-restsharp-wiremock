using Reqnroll;
using RestSharp;
using TafRestSharpWireMock.Tests.Utils;
using TafRestSharpWireMock.Tests.Utils.Api;
using TafRestSharpWireMock.Tests.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace TafRestSharpWireMock.Tests.StepDefinitions
{
    [Binding]
    public class DataDrivenSteps
    {
        private readonly ScenarioContext _context;
        private readonly IReqnrollOutput _output;
        private readonly ApiClient _apiClient;

        public DataDrivenSteps(ScenarioContext context, IReqnrollOutput output)
        {
            _context = context;
            _output = output;
            _apiClient = new ApiClient();
        }

        [When(@"I GET all predefined users from TestDataConstants")]
        public async Task WhenIGetAllPredefinedUsers()
        {
            var responses = new List<RestResponse>();

            foreach (var userId in TestDataConstants.UserIds)
            {
                var response = await _apiClient.GetAsync($"/api/users/{userId}");
                responses.Add(response);
                _output.Info($"GET request sent for user ID: {userId}");
            }

            _context["UserResponses"] = responses;
        }

        [Then(@"all GET requests should be successful")]
        public void ThenAllGetRequestsSuccessful()
        {
            var responses = (List<RestResponse>)_context["UserResponses"];

            foreach (var response in responses)
            {
                Assert.AreEqual(200, (int)response.StatusCode,
                    "All GET requests should return 200 OK");
            }

            _output.Pass($"All {responses.Count} GET requests were successful");
        }

        [When(@"I POST all predefined users from TestDataConstants to endpoint '(.*)'")]
        public async Task WhenIPostAllPredefinedUsers(string endpoint)
        {
            var responses = new List<RestResponse>();
            int rows = TestDataConstants.UsersToCreate.GetLength(0);

            for (int i = 0; i < rows; i++)
            {
                var name = TestDataConstants.UsersToCreate[i, 0];
                var job = TestDataConstants.UsersToCreate[i, 1];

                var payload = new CreateUserRequest
                {
                    Name = name,
                    Job = job
                };

                var response = await _apiClient.PostAsync(endpoint, payload);
                responses.Add(response);
                _output.Info($"POST request sent for user: {name} - {job}");
            }

            _context["CreateUserResponses"] = responses;
        }

        [Then(@"all POST requests should return status (.*)")]
        public void ThenAllPostRequestsReturnStatus(int expectedStatus)
        {
            var responses = (List<RestResponse>)_context["CreateUserResponses"];

            foreach (var response in responses)
            {
                Assert.AreEqual(expectedStatus, (int)response.StatusCode,
                    $"All POST requests should return {expectedStatus}");
            }

            _output.Pass($"All {responses.Count} POST requests returned status {expectedStatus}");
        }

        [Then(@"all created users should match the predefined data")]
        public void ThenAllCreatedUsersMatchPredefinedData()
        {
            var responses = (List<RestResponse>)_context["CreateUserResponses"];
            int rows = TestDataConstants.UsersToCreate.GetLength(0);

            Assert.AreEqual(rows, responses.Count,
                "Number of responses should match number of predefined users");

            for (int i = 0; i < rows; i++)
            {
                var expectedName = TestDataConstants.UsersToCreate[i, 0];
                var expectedJob = TestDataConstants.UsersToCreate[i, 1];

                var response = responses[i];
                var createResponse = JsonConvert.DeserializeObject<CreateUserResponse>(response.Content);

                Assert.AreEqual(expectedName, createResponse.Name,
                    $"User name should match: {expectedName}");
                Assert.AreEqual(expectedJob, createResponse.Job,
                    $"User job should match: {expectedJob}");
            }

            _output.Pass("All created users match the predefined data");
        }

        [When(@"I create user with name '(.*)' and job '(.*)' from constants")]
        public async Task WhenICreateUserFromConstants(string name, string job)
        {
            // Verify the data exists in constants
            int rows = TestDataConstants.UsersToCreate.GetLength(0);
            bool found = false;

            for (int i = 0; i < rows; i++)
            {
                if (TestDataConstants.UsersToCreate[i, 0] == name &&
                    TestDataConstants.UsersToCreate[i, 1] == job)
                {
                    found = true;
                    break;
                }
            }

            Assert.IsTrue(found, $"User {name} with job {job} should exist in TestDataConstants");

            var payload = new CreateUserRequest
            {
                Name = name,
                Job = job
            };

            var response = await _apiClient.PostAsync("/api/users", payload);

            if (!_context.ContainsKey("ConstantUserResponses"))
            {
                _context["ConstantUserResponses"] = new List<RestResponse>();
            }

            var responses = (List<RestResponse>)_context["ConstantUserResponses"];
            responses.Add(response);

            _output.Info($"Created user from constants: {name} - {job}");
        }

        [Then(@"both users should be created successfully")]
        public void ThenBothUsersCreatedSuccessfully()
        {
            var responses = (List<RestResponse>)_context["ConstantUserResponses"];

            Assert.AreEqual(2, responses.Count, "Should have 2 user creation responses");

            foreach (var response in responses)
            {
                Assert.AreEqual(201, (int)response.StatusCode,
                    "All users should be created with status 201");
            }

            _output.Pass("Both users were created successfully");
        }
    }
}
