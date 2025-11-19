using Reqnroll;
using RestSharp;
using TafRestSharpWireMock.Tests.Utils.Api;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TafRestSharpWireMock.Tests.StepDefinitions
{
    [Binding]
    public class DeleteSteps
    {
        private readonly ScenarioContext _context;
        private readonly IReqnrollOutput _output;
        private readonly ApiClient _apiClient;

        public DeleteSteps(ScenarioContext context, IReqnrollOutput output)
        {
            _context = context;
            _output = output;
            _apiClient = new ApiClient();
        }

        [When(@"I DELETE user with id (.*) from endpoint '(.*)'")]
        public async Task WhenIDeleteUserById(int userId, string endpoint)
        {
            endpoint = endpoint.Replace("{id}", userId.ToString());
            var response = await _apiClient.DeleteAsync(endpoint);
            _context["LastDeleteResponse"] = response;
            _output.Info($"DELETE request sent to: {endpoint}");
        }

        [Then(@"the DELETE response should have status (.*)")]
        public void ThenDeleteResponseStatus(int expectedStatus)
        {
            var response = (RestResponse)_context["LastDeleteResponse"];
            Assert.AreEqual(expectedStatus, (int)response.StatusCode);
            _output.Pass($"DELETE response status validated: {expectedStatus}");
        }

        [Given(@"I GET user with id (.*) from endpoint '(.*)'")]
        public async Task GivenIGetUserById(int userId, string endpoint)
        {
            endpoint = endpoint.Replace("{id}", userId.ToString());
            var response = await _apiClient.GetAsync(endpoint);
            _context["LastGetResponse"] = response;
            _output.Info($"GET request sent to: {endpoint}");
        }
    }
}
