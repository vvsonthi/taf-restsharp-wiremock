using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TafRestSharpWireMock.Tests.Utils.Headers
{
    public class HeaderLoader
    {
        private static Dictionary<string, Dictionary<string, string>> _cachedHeaders =
            new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Loads headers from a file for a specific test case number
        /// File format: TC001: Content-Type=application/json&Accept=application/json
        /// </summary>
        public static Dictionary<string, string> LoadHeaders(string filePath, string tcNumber)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Headers file not found: {filePath}");
            }

            // Check cache first
            if (_cachedHeaders.ContainsKey(filePath))
            {
                if (_cachedHeaders[filePath].ContainsKey(tcNumber))
                {
                    return new Dictionary<string, string>(_cachedHeaders[filePath][tcNumber]);
                }
            }
            else
            {
                _cachedHeaders[filePath] = new Dictionary<string, string>();
            }

            // Read and parse file
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                {
                    continue;
                }

                var parts = line.Split(new[] { ':' }, 2);
                if (parts.Length == 2)
                {
                    var tc = parts[0].Trim();
                    var headersString = parts[1].Trim();

                    if (!_cachedHeaders[filePath].ContainsKey(tc))
                    {
                        _cachedHeaders[filePath][tc] = headersString;
                    }
                }
            }

            // Return the headers for the requested TC
            if (_cachedHeaders[filePath].ContainsKey(tcNumber))
            {
                return ParseHeaderString(_cachedHeaders[filePath][tcNumber]);
            }

            throw new KeyNotFoundException($"Test case '{tcNumber}' not found in file: {filePath}");
        }

        /// <summary>
        /// Parses header string from inline format (key=value&key2=value2 or key=value,key2=value2)
        /// </summary>
        public static Dictionary<string, string> ParseHeaderString(string headerString)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(headerString) ||
                headerString.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                return result;
            }

            // Support both & and , as separators
            var separators = new[] { '&', ',' };
            var pairs = headerString.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in pairs)
            {
                var keyValue = pair.Split('=');
                if (keyValue.Length == 2)
                {
                    result[keyValue[0].Trim()] = keyValue[1].Trim();
                }
                else if (keyValue.Length == 1 && !string.IsNullOrWhiteSpace(keyValue[0]))
                {
                    result[keyValue[0].Trim()] = string.Empty;
                }
            }

            return result;
        }

        /// <summary>
        /// Loads headers from inline string or file based on input
        /// If input looks like a file (ends with .txt), loads from file
        /// Otherwise parses as inline header string
        /// </summary>
        public static Dictionary<string, string> LoadHeadersAuto(string input, string tcNumber = null)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                return new Dictionary<string, string>();
            }

            // Check if it looks like a file path
            if (input.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(tcNumber))
                {
                    throw new ArgumentException("TC number is required when loading headers from file");
                }

                string filePath = Path.Combine("Data", "Headers", input);
                return LoadHeaders(filePath, tcNumber);
            }

            // Parse as inline header string
            return ParseHeaderString(input);
        }

        /// <summary>
        /// Validates that expected headers are present in actual headers (case-insensitive)
        /// </summary>
        public static bool ValidateExpectedHeaders(Dictionary<string, string> actualHeaders, string expectedHeadersList)
        {
            if (string.IsNullOrWhiteSpace(expectedHeadersList) ||
                expectedHeadersList.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var expectedHeaders = expectedHeadersList
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(h => h.Trim().ToLower())
                .ToList();

            var actualHeaderKeys = actualHeaders.Keys.Select(k => k.ToLower()).ToList();

            foreach (var expected in expectedHeaders)
            {
                if (!actualHeaderKeys.Contains(expected))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets missing expected headers from actual headers
        /// </summary>
        public static List<string> GetMissingHeaders(Dictionary<string, string> actualHeaders, string expectedHeadersList)
        {
            var missing = new List<string>();

            if (string.IsNullOrWhiteSpace(expectedHeadersList) ||
                expectedHeadersList.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                return missing;
            }

            var expectedHeaders = expectedHeadersList
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(h => h.Trim().ToLower())
                .ToList();

            var actualHeaderKeys = actualHeaders.Keys.Select(k => k.ToLower()).ToList();

            foreach (var expected in expectedHeaders)
            {
                if (!actualHeaderKeys.Contains(expected))
                {
                    missing.Add(expected);
                }
            }

            return missing;
        }

        /// <summary>
        /// Clears the cache
        /// </summary>
        public static void ClearCache()
        {
            _cachedHeaders.Clear();
        }
    }
}
