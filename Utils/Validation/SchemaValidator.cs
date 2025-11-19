using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TafRestSharpWireMock.Tests.Utils.Validation
{
    public static class SchemaValidator
    {
        public static void ValidateField(JObject jsonObject, string fieldName, Type expectedType, bool isRequired = true)
        {
            if (isRequired)
            {
                Assert.IsTrue(jsonObject.ContainsKey(fieldName),
                    $"Field '{fieldName}' should exist in the response");
            }

            if (!jsonObject.ContainsKey(fieldName))
            {
                return; // Skip validation if field is not required and doesn't exist
            }

            var field = jsonObject[fieldName];

            if (expectedType == typeof(string))
            {
                Assert.IsTrue(field.Type == JTokenType.String,
                    $"Field '{fieldName}' should be of type string");
            }
            else if (expectedType == typeof(int))
            {
                Assert.IsTrue(field.Type == JTokenType.Integer,
                    $"Field '{fieldName}' should be of type integer");
            }
            else if (expectedType == typeof(bool))
            {
                Assert.IsTrue(field.Type == JTokenType.Boolean,
                    $"Field '{fieldName}' should be of type boolean");
            }
            else if (expectedType == typeof(Array))
            {
                Assert.IsTrue(field.Type == JTokenType.Array,
                    $"Field '{fieldName}' should be of type array");
            }
            else if (expectedType == typeof(object))
            {
                Assert.IsTrue(field.Type == JTokenType.Object,
                    $"Field '{fieldName}' should be of type object");
            }
        }

        public static void ValidateFieldNotEmpty(JObject jsonObject, string fieldName)
        {
            Assert.IsTrue(jsonObject.ContainsKey(fieldName),
                $"Field '{fieldName}' should exist in the response");

            var field = jsonObject[fieldName];

            if (field.Type == JTokenType.String)
            {
                Assert.IsFalse(string.IsNullOrEmpty(field.ToString()),
                    $"Field '{fieldName}' should not be empty");
            }
            else if (field.Type == JTokenType.Array)
            {
                var array = (JArray)field;
                Assert.IsTrue(array.Count > 0,
                    $"Array field '{fieldName}' should not be empty");
            }
        }

        public static void ValidateFieldRange(JObject jsonObject, string fieldName, int min, int max)
        {
            Assert.IsTrue(jsonObject.ContainsKey(fieldName),
                $"Field '{fieldName}' should exist in the response");

            var field = jsonObject[fieldName];
            int value = field.Value<int>();

            Assert.IsTrue(value >= min && value <= max,
                $"Field '{fieldName}' should be between {min} and {max}, but was {value}");
        }

        public static void ValidateFieldPattern(JObject jsonObject, string fieldName, string pattern)
        {
            Assert.IsTrue(jsonObject.ContainsKey(fieldName),
                $"Field '{fieldName}' should exist in the response");

            var field = jsonObject[fieldName];
            string value = field.ToString();

            Assert.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(value, pattern),
                $"Field '{fieldName}' with value '{value}' should match pattern '{pattern}'");
        }

        public static void ValidateRequiredFields(JObject jsonObject, params string[] fieldNames)
        {
            foreach (var fieldName in fieldNames)
            {
                Assert.IsTrue(jsonObject.ContainsKey(fieldName),
                    $"Required field '{fieldName}' should exist in the response");
            }
        }

        public static void ValidateUserSchema(JObject userData)
        {
            ValidateRequiredFields(userData, "id", "email", "first_name", "last_name", "avatar");

            ValidateField(userData, "id", typeof(int));
            ValidateField(userData, "email", typeof(string));
            ValidateField(userData, "first_name", typeof(string));
            ValidateField(userData, "last_name", typeof(string));
            ValidateField(userData, "avatar", typeof(string));

            ValidateFieldNotEmpty(userData, "email");
            ValidateFieldNotEmpty(userData, "first_name");
            ValidateFieldNotEmpty(userData, "last_name");
            ValidateFieldNotEmpty(userData, "avatar");

            ValidateFieldRange(userData, "id", 1, int.MaxValue);

            // Validate email format
            ValidateFieldPattern(userData, "email", @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public static void ValidateUserListSchema(JObject userListData)
        {
            ValidateRequiredFields(userListData, "page", "per_page", "total", "total_pages", "data");

            ValidateField(userListData, "page", typeof(int));
            ValidateField(userListData, "per_page", typeof(int));
            ValidateField(userListData, "total", typeof(int));
            ValidateField(userListData, "total_pages", typeof(int));
            ValidateField(userListData, "data", typeof(Array));

            ValidateFieldRange(userListData, "page", 1, int.MaxValue);
            ValidateFieldRange(userListData, "per_page", 1, int.MaxValue);
            ValidateFieldNotEmpty(userListData, "data");

            // Validate each user in the data array
            var dataArray = (JArray)userListData["data"];
            foreach (var user in dataArray)
            {
                ValidateUserSchema((JObject)user);
            }
        }

        public static void ValidateCreateUserSchema(JObject createResponse)
        {
            ValidateRequiredFields(createResponse, "name", "job", "id", "createdAt");

            ValidateField(createResponse, "name", typeof(string));
            ValidateField(createResponse, "job", typeof(string));
            ValidateField(createResponse, "id", typeof(string));
            ValidateField(createResponse, "createdAt", typeof(string));

            ValidateFieldNotEmpty(createResponse, "name");
            ValidateFieldNotEmpty(createResponse, "job");
            ValidateFieldNotEmpty(createResponse, "id");
            ValidateFieldNotEmpty(createResponse, "createdAt");

            // Validate ISO 8601 date format
            ValidateFieldPattern(createResponse, "createdAt", @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}");
        }

        public static void ValidateUpdateUserSchema(JObject updateResponse)
        {
            ValidateRequiredFields(updateResponse, "updatedAt");

            ValidateField(updateResponse, "updatedAt", typeof(string));
            ValidateFieldNotEmpty(updateResponse, "updatedAt");

            // Validate ISO 8601 date format
            ValidateFieldPattern(updateResponse, "updatedAt", @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}");
        }
    }
}
