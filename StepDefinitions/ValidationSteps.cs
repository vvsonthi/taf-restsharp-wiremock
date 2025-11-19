using Reqnroll;
using RestSharp;
using TafRestSharpWireMock.Tests.Utils.Validation;
using TafRestSharpWireMock.Tests.Utils.Headers;
using TafRestSharpWireMock.Tests.Utils.ApiServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.IO;
using System.Linq;

namespace TafRestSharpWireMock.Tests.StepDefinitions
{
    [Binding]
    public class ValidationSteps
    {
        private readonly ScenarioContext _context;
        private readonly IReqnrollOutput _output;
        private readonly ApiServiceManager _serviceManager;

        public ValidationSteps(ScenarioContext context, IReqnrollOutput output)
        {
            _context = context;
            _output = output;
            _serviceManager = new ApiServiceManager(context);
        }

        private RestResponse GetLastResponse()
        {
            if (_context.ContainsKey("LastResponse"))
                return (RestResponse)_context["LastResponse"];
            if (_context.ContainsKey("LastGetResponse"))
                return (RestResponse)_context["LastGetResponse"];
            if (_context.ContainsKey("LastPostResponse"))
                return (RestResponse)_context["LastPostResponse"];
            if (_context.ContainsKey("LastPutResponse"))
                return (RestResponse)_context["LastPutResponse"];
            if (_context.ContainsKey("LastPatchResponse"))
                return (RestResponse)_context["LastPatchResponse"];
            if (_context.ContainsKey("LastDeleteResponse"))
                return (RestResponse)_context["LastDeleteResponse"];

            throw new Exception("No response found in context");
        }

        // ========== 1. Field Exact Match Verification ==========

        [Then(@"I verify the ""(.*)"" from the response to be ""(.*)""")]
        public void ThenIVerifyFieldToBe(string jsonKey, string jsonKeyValue)
        {
            if (jsonKey.Equals("none", StringComparison.OrdinalIgnoreCase) ||
                jsonKey.Equals("n/a", StringComparison.OrdinalIgnoreCase))
            {
                _output.Info("Skipping field validation (none specified)");
                return;
            }

            var response = GetLastResponse();
            var json = JToken.Parse(response.Content);

            FieldValidator.ValidateField(json, jsonKey, jsonKeyValue);

            _output.Pass($"Field '{jsonKey}' validated: {jsonKeyValue}");
        }

        // ========== 2. Field Contains Verification ==========

        [Then(@"I verify the ""(.*)"" from the response to contain ""(.*)""")]
        public void ThenIVerifyFieldToContain(string jsonKey, string jsonKeyValue)
        {
            if (jsonKey.Equals("none", StringComparison.OrdinalIgnoreCase) ||
                jsonKey.Equals("n/a", StringComparison.OrdinalIgnoreCase))
            {
                _output.Info("Skipping field contains validation (none specified)");
                return;
            }

            var response = GetLastResponse();
            var json = JToken.Parse(response.Content);

            var field = FieldValidator.GetField(json, jsonKey);
            Assert.IsNotNull(field, $"Field '{jsonKey}' should exist");

            string actualValue = field.ToString();
            Assert.IsTrue(actualValue.Contains(jsonKeyValue),
                $"Field '{jsonKey}' should contain '{jsonKeyValue}', but was '{actualValue}'");

            _output.Pass($"Field '{jsonKey}' contains: {jsonKeyValue}");
        }

        // ========== 3. Headers Presence Verification ==========

        [Then(@"I verify the headers ""(.*)"" to be present")]
        public void ThenIVerifyHeadersPresent(string expectedHeaderKeyNamesOnly)
        {
            if (expectedHeaderKeyNamesOnly.Equals("none", StringComparison.OrdinalIgnoreCase) ||
                expectedHeaderKeyNamesOnly.Equals("n/a", StringComparison.OrdinalIgnoreCase))
            {
                _output.Info("Skipping header presence validation (none specified)");
                return;
            }

            var response = GetLastResponse();
            var actualHeaders = response.Headers.ToDictionary(h => h.Name, h => h.Value?.ToString() ?? string.Empty);

            // Split expected header names by comma
            var expectedHeaderNames = expectedHeaderKeyNamesOnly.Split(',')
                .Select(h => h.Trim())
                .Where(h => !string.IsNullOrWhiteSpace(h))
                .ToList();

            var missingHeaders = expectedHeaderNames
                .Where(headerName => !actualHeaders.ContainsKey(headerName))
                .ToList();

            Assert.AreEqual(0, missingHeaders.Count,
                $"Missing expected headers: {string.Join(", ", missingHeaders)}");

            _output.Pass($"All expected headers present: {expectedHeaderKeyNamesOnly}");
        }

        // ========== 4. Specific Header Value Verification ==========

        [Then(@"I verify the header value for the key ""(.*)"" to be ""(.*)""")]
        public void ThenIVerifyHeaderValueForKey(string expectedHeaderKeyName, string expectedHeaderKeyValue)
        {
            if (expectedHeaderKeyName.Equals("none", StringComparison.OrdinalIgnoreCase) ||
                expectedHeaderKeyName.Equals("n/a", StringComparison.OrdinalIgnoreCase))
            {
                _output.Info("Skipping header value validation (none specified)");
                return;
            }

            var response = GetLastResponse();
            var header = response.Headers.FirstOrDefault(h =>
                h.Name.Equals(expectedHeaderKeyName, StringComparison.OrdinalIgnoreCase));

            Assert.IsNotNull(header, $"Header '{expectedHeaderKeyName}' not found in response");

            string actualValue = header.Value?.ToString() ?? string.Empty;
            Assert.AreEqual(expectedHeaderKeyValue, actualValue,
                $"Header '{expectedHeaderKeyName}' should be '{expectedHeaderKeyValue}', but was '{actualValue}'");

            _output.Pass($"Header '{expectedHeaderKeyName}' validated: {expectedHeaderKeyValue}");
        }

        // ========== 5. Field Exists Verification ==========

        [Then(@"I verify the ""(.*)"" in the response ""exists""")]
        public void ThenIVerifyFieldExists(string jsonKey)
        {
            if (jsonKey.Equals("none", StringComparison.OrdinalIgnoreCase) ||
                jsonKey.Equals("n/a", StringComparison.OrdinalIgnoreCase))
            {
                _output.Info("Skipping field existence validation (none specified)");
                return;
            }

            var response = GetLastResponse();
            var json = JToken.Parse(response.Content);

            var field = FieldValidator.GetField(json, jsonKey);
            Assert.IsNotNull(field, $"Field '{jsonKey}' should exist in response");

            _output.Pass($"Field '{jsonKey}' exists in response");
        }

        // ========== 6. Field Does Not Exist Verification ==========

        [Then(@"I verify the ""(.*)"" in the response ""does not exist""")]
        public void ThenIVerifyFieldDoesNotExist(string jsonKey)
        {
            if (jsonKey.Equals("none", StringComparison.OrdinalIgnoreCase) ||
                jsonKey.Equals("n/a", StringComparison.OrdinalIgnoreCase))
            {
                _output.Info("Skipping field non-existence validation (none specified)");
                return;
            }

            var response = GetLastResponse();
            var json = JToken.Parse(response.Content);

            var field = FieldValidator.GetField(json, jsonKey);
            Assert.IsNull(field, $"Field '{jsonKey}' should not exist in response, but it does");

            _output.Pass($"Field '{jsonKey}' does not exist in response");
        }

        // ========== 7. Field Type Verification ==========

        [Then(@"I verify the ""(.*)"" type in the response to be a ""(.*)""")]
        public void ThenIVerifyFieldType(string jsonKey, string expectedType)
        {
            if (jsonKey.Equals("none", StringComparison.OrdinalIgnoreCase) ||
                jsonKey.Equals("n/a", StringComparison.OrdinalIgnoreCase))
            {
                _output.Info("Skipping field type validation (none specified)");
                return;
            }

            var response = GetLastResponse();
            var json = JToken.Parse(response.Content);

            var field = FieldValidator.GetField(json, jsonKey);
            Assert.IsNotNull(field, $"Field '{jsonKey}' should exist in response");

            // Normalize expected type to lowercase for comparison
            string normalizedExpectedType = expectedType.ToLower();
            JTokenType actualType = field.Type;

            bool typeMatches = normalizedExpectedType switch
            {
                "string" => actualType == JTokenType.String,
                "integer" or "int" => actualType == JTokenType.Integer,
                "number" or "float" or "double" => actualType == JTokenType.Float || actualType == JTokenType.Integer,
                "boolean" or "bool" => actualType == JTokenType.Boolean,
                "array" => actualType == JTokenType.Array,
                "object" => actualType == JTokenType.Object,
                "null" => actualType == JTokenType.Null,
                _ => throw new ArgumentException($"Unsupported type: {expectedType}. Supported types: string, integer, number, boolean, array, object, null")
            };

            Assert.IsTrue(typeMatches,
                $"Field '{jsonKey}' should be of type '{expectedType}', but was '{actualType}'");

            _output.Pass($"Field '{jsonKey}' is of type: {expectedType}");
        }

        // ========== 8. Schema Validation ==========

        [Then(@"the response returned should match the schema ""(.*)""")]
        public void ThenResponseShouldMatchSchema(string schemaFilename)
        {
            if (schemaFilename.Equals("none", StringComparison.OrdinalIgnoreCase) ||
                schemaFilename.Equals("n/a", StringComparison.OrdinalIgnoreCase))
            {
                _output.Info("Skipping schema validation (none specified)");
                return;
            }

            var response = GetLastResponse();
            var json = JToken.Parse(response.Content);

            // Get current API name to determine schema folder
            string apiName = _serviceManager.GetCurrentServiceName();

            // Build schema file path: Schemas/{ApiName}/{schemaFilename}
            string schemaFilePath = Path.Combine("Schemas", apiName, schemaFilename);

            if (!File.Exists(schemaFilePath))
            {
                throw new FileNotFoundException($"Schema file not found: {schemaFilePath}");
            }

            // Load schema from file
            string schemaJson = File.ReadAllText(schemaFilePath);
            var schema = JSchema.Parse(schemaJson);

            // Validate JSON against schema
            bool isValid = json.IsValid(schema, out IList<string> errorMessages);

            if (!isValid)
            {
                string errors = string.Join(Environment.NewLine, errorMessages);
                Assert.Fail($"Response does not match schema '{schemaFilename}':{Environment.NewLine}{errors}");
            }

            _output.Pass($"Response matches schema: {schemaFilename}");
        }

        // ========== Legacy Compatibility Steps ==========

        [Then(@"I verify that the field ""(.*)"" in the response is ""(.*)""")]
        public void ThenIVerifyThatFieldInResponseIs(string fieldName, string fieldContents)
        {
            ThenIVerifyFieldToBe(fieldName, fieldContents);
        }

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

        [Then(@"I receive a response with HTTP status code ""(.*)"" with status text ""(.*)""")]
        public void ThenIReceiveResponseWithStatus(string statusCode, string statusText)
        {
            var response = GetLastResponse();

            Assert.AreEqual(int.Parse(statusCode), (int)response.StatusCode,
                $"Expected status code {statusCode}, but got {(int)response.StatusCode}");

            _output.Pass($"Response status: {statusCode} {statusText}");
        }
    }
}
