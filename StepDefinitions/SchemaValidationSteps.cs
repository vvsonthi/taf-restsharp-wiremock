using Reqnroll;
using RestSharp;
using TafRestSharpWireMock.Tests.Utils.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace TafRestSharpWireMock.Tests.StepDefinitions
{
    [Binding]
    public class SchemaValidationSteps
    {
        private readonly ScenarioContext _context;
        private readonly IReqnrollOutput _output;

        public SchemaValidationSteps(ScenarioContext context, IReqnrollOutput output)
        {
            _context = context;
            _output = output;
        }

        private RestResponse GetLastResponse()
        {
            if (_context.ContainsKey("LastGetResponse"))
                return (RestResponse)_context["LastGetResponse"];
            if (_context.ContainsKey("LastPostResponse"))
                return (RestResponse)_context["LastPostResponse"];
            if (_context.ContainsKey("LastPutResponse"))
                return (RestResponse)_context["LastPutResponse"];
            if (_context.ContainsKey("LastPatchResponse"))
                return (RestResponse)_context["LastPatchResponse"];

            throw new System.Exception("No response found in context");
        }

        [Then(@"the response should have valid single user schema with all required fields")]
        public void ThenResponseHasValidSingleUserSchema()
        {
            var response = GetLastResponse();
            var jsonObject = JObject.Parse(response.Content);
            var userData = (JObject)jsonObject["data"];

            SchemaValidator.ValidateUserSchema(userData);
            _output.Pass("Response has valid single user schema");
        }

        [Then(@"user email should match email pattern")]
        public void ThenUserEmailMatchesPattern()
        {
            var response = GetLastResponse();
            var jsonObject = JObject.Parse(response.Content);
            var userData = (JObject)jsonObject["data"];

            SchemaValidator.ValidateFieldPattern(userData, "email", @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            _output.Pass("User email matches pattern");
        }

        [Then(@"user id should be within valid range")]
        public void ThenUserIdWithinValidRange()
        {
            var response = GetLastResponse();
            var jsonObject = JObject.Parse(response.Content);
            var userData = (JObject)jsonObject["data"];

            SchemaValidator.ValidateFieldRange(userData, "id", 1, 100);
            _output.Pass("User ID is within valid range");
        }

        [Then(@"the response should have valid user list schema with pagination")]
        public void ThenResponseHasValidUserListSchema()
        {
            var response = GetLastResponse();
            var jsonObject = JObject.Parse(response.Content);

            SchemaValidator.ValidateRequiredFields(jsonObject, "page", "per_page", "total", "total_pages", "data");
            _output.Pass("Response has valid user list schema with pagination");
        }

        [Then(@"all users in list should have valid schema")]
        public void ThenAllUsersHaveValidSchema()
        {
            var response = GetLastResponse();
            var jsonObject = JObject.Parse(response.Content);
            var dataArray = (JArray)jsonObject["data"];

            foreach (var user in dataArray)
            {
                SchemaValidator.ValidateUserSchema((JObject)user);
            }

            _output.Pass($"All {dataArray.Count} users have valid schema");
        }

        [Then(@"pagination fields should be within valid range")]
        public void ThenPaginationFieldsWithinValidRange()
        {
            var response = GetLastResponse();
            var jsonObject = JObject.Parse(response.Content);

            SchemaValidator.ValidateFieldRange(jsonObject, "page", 1, int.MaxValue);
            SchemaValidator.ValidateFieldRange(jsonObject, "per_page", 1, 100);
            SchemaValidator.ValidateFieldRange(jsonObject, "total", 1, int.MaxValue);
            SchemaValidator.ValidateFieldRange(jsonObject, "total_pages", 1, int.MaxValue);

            _output.Pass("Pagination fields are within valid range");
        }

        [Then(@"the response should have valid create user schema")]
        public void ThenResponseHasValidCreateUserSchema()
        {
            var response = GetLastResponse();
            var jsonObject = JObject.Parse(response.Content);

            SchemaValidator.ValidateCreateUserSchema(jsonObject);
            _output.Pass("Response has valid create user schema");
        }

        [Then(@"createdAt should be in ISO 8601 format")]
        public void ThenCreatedAtInISO8601Format()
        {
            var response = GetLastResponse();
            var jsonObject = JObject.Parse(response.Content);

            SchemaValidator.ValidateFieldPattern(jsonObject, "createdAt", @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}");
            _output.Pass("createdAt is in ISO 8601 format");
        }

        [Then(@"created user id should not be empty")]
        public void ThenCreatedUserIdNotEmpty()
        {
            var response = GetLastResponse();
            var jsonObject = JObject.Parse(response.Content);

            SchemaValidator.ValidateFieldNotEmpty(jsonObject, "id");
            _output.Pass("Created user ID is not empty");
        }

        [Then(@"the response should have valid update user schema")]
        public void ThenResponseHasValidUpdateUserSchema()
        {
            var response = GetLastResponse();
            var jsonObject = JObject.Parse(response.Content);

            SchemaValidator.ValidateUpdateUserSchema(jsonObject);
            _output.Pass("Response has valid update user schema");
        }

        [Then(@"updatedAt should be in ISO 8601 format")]
        public void ThenUpdatedAtInISO8601Format()
        {
            var response = GetLastResponse();
            var jsonObject = JObject.Parse(response.Content);

            SchemaValidator.ValidateFieldPattern(jsonObject, "updatedAt", @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}");
            _output.Pass("updatedAt is in ISO 8601 format");
        }

        [Then(@"the response should contain all required fields: (.*)")]
        public void ThenResponseContainsRequiredFields(string fieldList)
        {
            var response = GetLastResponse();
            var jsonObject = JObject.Parse(response.Content);
            var userData = (JObject)jsonObject["data"];

            var fields = fieldList.Split(',').Select(f => f.Trim()).ToArray();
            SchemaValidator.ValidateRequiredFields(userData, fields);

            _output.Pass($"Response contains all required fields: {fieldList}");
        }

        [Then(@"field '(.*)' should be of type integer")]
        public void ThenFieldIsInteger(string fieldName)
        {
            var response = GetLastResponse();
            var jsonObject = JObject.Parse(response.Content);
            var userData = (JObject)jsonObject["data"];

            SchemaValidator.ValidateField(userData, fieldName, typeof(int));
            _output.Pass($"Field '{fieldName}' is of type integer");
        }

        [Then(@"field '(.*)' should be of type string")]
        public void ThenFieldIsString(string fieldName)
        {
            var response = GetLastResponse();
            var jsonObject = JObject.Parse(response.Content);
            var userData = (JObject)jsonObject["data"];

            SchemaValidator.ValidateField(userData, fieldName, typeof(string));
            _output.Pass($"Field '{fieldName}' is of type string");
        }
    }
}
