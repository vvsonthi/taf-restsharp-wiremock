using Reqnroll;
using RestSharp;
using TafRestSharpWireMock.Tests.Utils.Api;
using TafRestSharpWireMock.Tests.Models;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace TafRestSharpWireMock.Tests.StepDefinitions
{
    [Binding]
    public class GetSteps
    {
        private readonly ScenarioContext _context;
        private readonly IReqnrollOutput _output;
        private readonly ApiClient _apiClient;

        public GetSteps(ScenarioContext context, IReqnrollOutput output)
        {
            _context = context;
            _output = output;
            _apiClient = new ApiClient();
        }

        [When(@"I GET user with id (.*) from endpoint '(.*)'")]
        public async Task WhenIGetUserById(int userId, string endpoint)
        {
            endpoint = endpoint.Replace("{id}", userId.ToString());
            var response = await _apiClient.GetAsync(endpoint);
            _context["LastGetResponse"] = response;
            _output.Info($"GET request sent to: {endpoint}");
        }

        [When(@"I GET all users from endpoint '(.*)'")]
        public async Task WhenIGetAllUsers(string endpoint)
        {
            var response = await _apiClient.GetAsync(endpoint);
            _context["LastGetResponse"] = response;
            _output.Info($"GET request sent to: {endpoint}");
        }

        [When(@"I GET users from endpoint '(.*)' with page (.*)")]
        public async Task WhenIGetUsersWithPagination(string endpoint, int page)
        {
            var response = await _apiClient.GetAsync($"{endpoint}?page={page}");
            _context["LastGetResponse"] = response;
            _output.Info($"GET request sent to: {endpoint}?page={page}");
        }

        [Then(@"the GET response should have status (.*)")]
        public void ThenGetResponseStatus(int expectedStatus)
        {
            var response = (RestResponse)_context["LastGetResponse"];
            Assert.AreEqual(expectedStatus, (int)response.StatusCode);
            _output.Pass($"GET response status validated: {expectedStatus}");
        }

        [Then(@"the GET response should contain user with id (.*)")]
        public void ThenGetResponseContainsUserId(int expectedUserId)
        {
            var response = (RestResponse)_context["LastGetResponse"];
            var userData = JsonConvert.DeserializeObject<UserData>(response.Content);

            Assert.IsNotNull(userData);
            Assert.IsNotNull(userData.Data);
            Assert.AreEqual(expectedUserId, userData.Data.Id);
            _output.Pass($"GET response contains user with id: {expectedUserId}");
        }

        [Then(@"the GET response should contain a list of users")]
        public void ThenGetResponseContainsUserList()
        {
            var response = (RestResponse)_context["LastGetResponse"];
            var userList = JsonConvert.DeserializeObject<UserListData>(response.Content);

            Assert.IsNotNull(userList);
            Assert.IsNotNull(userList.Data);
            Assert.IsTrue(userList.Data.Length > 0);
            _output.Pass($"GET response contains {userList.Data.Length} users");
        }

        [Then(@"the GET response should have page number (.*)")]
        public void ThenGetResponseHasPageNumber(int expectedPage)
        {
            var response = (RestResponse)_context["LastGetResponse"];
            var userList = JsonConvert.DeserializeObject<UserListData>(response.Content);

            Assert.IsNotNull(userList);
            Assert.AreEqual(expectedPage, userList.Page);
            _output.Pass($"GET response has page number: {expectedPage}");
        }

        [Then(@"the GET response should have valid user schema")]
        public void ThenGetResponseHasValidUserSchema()
        {
            var response = (RestResponse)_context["LastGetResponse"];
            var userData = JsonConvert.DeserializeObject<UserData>(response.Content);

            Assert.IsNotNull(userData, "UserData should not be null");
            Assert.IsNotNull(userData.Data, "User data should not be null");
            Assert.IsTrue(userData.Data.Id > 0, "User ID should be greater than 0");
            Assert.IsFalse(string.IsNullOrEmpty(userData.Data.Email), "Email should not be empty");
            Assert.IsFalse(string.IsNullOrEmpty(userData.Data.First_Name), "First name should not be empty");
            Assert.IsFalse(string.IsNullOrEmpty(userData.Data.Last_Name), "Last name should not be empty");
            Assert.IsFalse(string.IsNullOrEmpty(userData.Data.Avatar), "Avatar should not be empty");

            _output.Pass("GET response has valid user schema");
        }

        [Then(@"the GET response should have valid user list schema")]
        public void ThenGetResponseHasValidUserListSchema()
        {
            var response = (RestResponse)_context["LastGetResponse"];
            var userList = JsonConvert.DeserializeObject<UserListData>(response.Content);

            Assert.IsNotNull(userList, "UserListData should not be null");
            Assert.IsTrue(userList.Page > 0, "Page should be greater than 0");
            Assert.IsTrue(userList.Per_Page > 0, "Per_Page should be greater than 0");
            Assert.IsTrue(userList.Total > 0, "Total should be greater than 0");
            Assert.IsTrue(userList.Total_Pages > 0, "Total_Pages should be greater than 0");
            Assert.IsNotNull(userList.Data, "Data array should not be null");
            Assert.IsTrue(userList.Data.Length > 0, "Data array should contain users");

            // Validate first user in the list
            var firstUser = userList.Data[0];
            Assert.IsTrue(firstUser.Id > 0, "First user ID should be greater than 0");
            Assert.IsFalse(string.IsNullOrEmpty(firstUser.Email), "First user email should not be empty");

            _output.Pass("GET response has valid user list schema");
        }
    }
}
