using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TafRestSharpWireMock.Tests.Utils.Validation
{
    public static class FieldValidator
    {
        /// <summary>
        /// Validates a field in a JSON response against expected contents
        /// Supports special values: NotEmpty, Empty, Null, NotNull, Any
        /// </summary>
        public static void ValidateField(JToken responseJson, string fieldName, string expectedContents)
        {
            if (string.IsNullOrWhiteSpace(fieldName) || fieldName.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                return; // Skip validation
            }

            var field = GetField(responseJson, fieldName);

            // Handle special expected values
            switch (expectedContents)
            {
                case "NotEmpty":
                    ValidateNotEmpty(field, fieldName);
                    break;

                case "Empty":
                    ValidateEmpty(field, fieldName);
                    break;

                case "Null":
                    ValidateNull(field, fieldName);
                    break;

                case "NotNull":
                    ValidateNotNull(field, fieldName);
                    break;

                case "Any":
                    // Any value is acceptable, just check field exists
                    Assert.IsNotNull(field, $"Field '{fieldName}' should exist");
                    break;

                case "none":
                    // Skip validation
                    break;

                default:
                    // Exact value match
                    ValidateExactValue(field, fieldName, expectedContents);
                    break;
            }
        }

        /// <summary>
        /// Gets a field from JSON, supporting nested paths with dot notation
        /// Example: "data.user.name" or "users[0].email"
        /// </summary>
        public static JToken GetField(JToken json, string fieldPath)
        {
            if (json == null)
            {
                return null;
            }

            var parts = fieldPath.Split('.');
            JToken current = json;

            foreach (var part in parts)
            {
                if (current == null)
                {
                    return null;
                }

                // Handle array indexing: users[0]
                if (part.Contains("[") && part.Contains("]"))
                {
                    var arrayName = part.Substring(0, part.IndexOf("["));
                    var indexStr = part.Substring(part.IndexOf("[") + 1, part.IndexOf("]") - part.IndexOf("[") - 1);

                    if (int.TryParse(indexStr, out int index))
                    {
                        current = current[arrayName];
                        if (current != null && current.Type == JTokenType.Array)
                        {
                            var array = (JArray)current;
                            if (index < array.Count)
                            {
                                current = array[index];
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    current = current[part];
                }
            }

            return current;
        }

        private static void ValidateNotEmpty(JToken field, string fieldName)
        {
            Assert.IsNotNull(field, $"Field '{fieldName}' should not be null");

            if (field.Type == JTokenType.String)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(field.ToString()),
                    $"Field '{fieldName}' should not be empty");
            }
            else if (field.Type == JTokenType.Array)
            {
                var array = (JArray)field;
                Assert.IsTrue(array.Count > 0,
                    $"Array field '{fieldName}' should not be empty");
            }
            else if (field.Type == JTokenType.Object)
            {
                var obj = (JObject)field;
                Assert.IsTrue(obj.Properties().Any(),
                    $"Object field '{fieldName}' should not be empty");
            }
        }

        private static void ValidateEmpty(JToken field, string fieldName)
        {
            if (field == null)
            {
                return; // Null is considered empty
            }

            if (field.Type == JTokenType.String)
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(field.ToString()),
                    $"Field '{fieldName}' should be empty");
            }
            else if (field.Type == JTokenType.Array)
            {
                var array = (JArray)field;
                Assert.AreEqual(0, array.Count,
                    $"Array field '{fieldName}' should be empty");
            }
            else if (field.Type == JTokenType.Object)
            {
                var obj = (JObject)field;
                Assert.IsFalse(obj.Properties().Any(),
                    $"Object field '{fieldName}' should be empty");
            }
        }

        private static void ValidateNull(JToken field, string fieldName)
        {
            Assert.IsTrue(field == null || field.Type == JTokenType.Null,
                $"Field '{fieldName}' should be null");
        }

        private static void ValidateNotNull(JToken field, string fieldName)
        {
            Assert.IsNotNull(field, $"Field '{fieldName}' should not be null");
            Assert.IsFalse(field.Type == JTokenType.Null,
                $"Field '{fieldName}' should not be null");
        }

        private static void ValidateExactValue(JToken field, string fieldName, string expectedValue)
        {
            Assert.IsNotNull(field, $"Field '{fieldName}' should exist");

            string actualValue = field.ToString();

            Assert.AreEqual(expectedValue, actualValue,
                $"Field '{fieldName}' should have value '{expectedValue}', but was '{actualValue}'");
        }

        /// <summary>
        /// Validates that a response contains expected key-value text
        /// Searches the entire JSON response for the key and validates its value
        /// </summary>
        public static void ValidateResponseContainsKeyValue(JToken responseJson, string expectedKey, string expectedValue)
        {
            if (string.IsNullOrWhiteSpace(expectedKey) || expectedKey.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                return; // Skip validation
            }

            if (string.IsNullOrWhiteSpace(expectedValue) || expectedValue.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                return; // Skip validation
            }

            var found = FindKeyInJson(responseJson, expectedKey, expectedValue);

            Assert.IsTrue(found,
                $"Response should contain key '{expectedKey}' with value '{expectedValue}'");
        }

        private static bool FindKeyInJson(JToken token, string key, string expectedValue)
        {
            if (token == null)
            {
                return false;
            }

            if (token.Type == JTokenType.Object)
            {
                var obj = (JObject)token;

                // Check if this object has the key
                if (obj.ContainsKey(key))
                {
                    var value = obj[key].ToString();
                    if (value == expectedValue)
                    {
                        return true;
                    }
                }

                // Recursively check nested objects
                foreach (var property in obj.Properties())
                {
                    if (FindKeyInJson(property.Value, key, expectedValue))
                    {
                        return true;
                    }
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                var array = (JArray)token;
                foreach (var item in array)
                {
                    if (FindKeyInJson(item, key, expectedValue))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Validates response contains text (anywhere in the JSON)
        /// </summary>
        public static void ValidateResponseContainsText(string responseContent, string expectedText)
        {
            if (string.IsNullOrWhiteSpace(expectedText) || expectedText.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Assert.IsTrue(responseContent.Contains(expectedText),
                $"Response should contain text '{expectedText}'");
        }
    }
}
