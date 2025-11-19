using Reqnroll;
using RestSharp;
using TafRestSharpWireMock.Tests.Utils.Json;
using TafRestSharpWireMock.Tests.Utils.Api;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TafRestSharpWireMock.Tests.StepDefinitions
{
    [Binding]
    public class PostSteps
    {
        private readonly ScenarioContext _context;
        private readonly IReqnrollOutput _output;
        private readonly ApiClient _apiClient;

        public PostSteps(ScenarioContext context, IReqnrollOutput output)
        {
            _context = context;
            _output = output;
            _apiClient = new ApiClient();
        }

        [When(@"I POST the payload from file '(.*)' to endpoint '(.*)'")]
        public async Task WhenIPostPayloadFromFile(string fileName, string endpoint)
        {
            string filePath = Path.Combine("Data", fileName);
            string payload = JsonFileLoader.LoadJsonFromFile(filePath);

            var response = await _apiClient.PostAsync(endpoint, payload);
            _context["LastPostResponse"] = response;
            _output.Info("POST request sent with payload: " + fileName);
        }

        [Then(@"the POST response should have status (.*)")]
        public void ThenPostResponseStatus(int expectedStatus)
        {
            var response = (RestResponse)_context["LastPostResponse"];
            Assert.AreEqual(expectedStatus, (int)response.StatusCode);
            _output.Pass("POST response status validated successfully");
        }

        [Then(@"the POST response should contain '(.*)'")]
        public void ThenPostResponseContains(string expectedValue)
        {
            var response = (RestResponse)_context["LastPostResponse"];
            Assert.IsTrue(response.Content.Contains(expectedValue));
            _output.Pass("POST response contains expected value: " + expectedValue);
        }
    }
}