using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TafRestSharpWireMock.Tests.Utils.QueryParams
{
    public class QueryParamLoader
    {
        private static Dictionary<string, Dictionary<string, string>> _cachedParams =
            new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Loads query parameters from a file for a specific test case number
        /// File format: TC1: name=jon&age=25
        /// </summary>
        /// <param name="filePath">Path to the query params file</param>
        /// <param name="tcNumber">Test case number (e.g., TC1, TC2)</param>
        /// <returns>Query string for the test case</returns>
        public static string LoadQueryParams(string filePath, string tcNumber)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Query params file not found: {filePath}");
            }

            var cacheKey = $"{filePath}_{tcNumber}";

            // Check cache first
            if (_cachedParams.ContainsKey(filePath))
            {
                if (_cachedParams[filePath].ContainsKey(tcNumber))
                {
                    return _cachedParams[filePath][tcNumber];
                }
            }
            else
            {
                _cachedParams[filePath] = new Dictionary<string, string>();
            }

            // Read and parse file
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                {
                    continue; // Skip empty lines and comments
                }

                // Parse format: TC1: name=jon&age=25
                var parts = line.Split(new[] { ':' }, 2);
                if (parts.Length == 2)
                {
                    var tc = parts[0].Trim();
                    var queryString = parts[1].Trim();

                    if (!_cachedParams[filePath].ContainsKey(tc))
                    {
                        _cachedParams[filePath][tc] = queryString;
                    }
                }
            }

            // Return the query params for the requested TC
            if (_cachedParams[filePath].ContainsKey(tcNumber))
            {
                return _cachedParams[filePath][tcNumber];
            }

            throw new KeyNotFoundException($"Test case '{tcNumber}' not found in file: {filePath}");
        }

        /// <summary>
        /// Loads query parameters and returns as a dictionary
        /// </summary>
        public static Dictionary<string, string> LoadQueryParamsAsDictionary(string filePath, string tcNumber)
        {
            var queryString = LoadQueryParams(filePath, tcNumber);
            return ParseQueryString(queryString);
        }

        /// <summary>
        /// Parses a query string into a dictionary
        /// Example: "name=jon&age=25" -> { "name": "jon", "age": "25" }
        /// </summary>
        public static Dictionary<string, string> ParseQueryString(string queryString)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(queryString))
            {
                return result;
            }

            var pairs = queryString.Split('&');
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split('=');
                if (keyValue.Length == 2)
                {
                    result[keyValue[0]] = keyValue[1];
                }
                else if (keyValue.Length == 1)
                {
                    result[keyValue[0]] = string.Empty;
                }
            }

            return result;
        }

        /// <summary>
        /// Loads all query params from a file into a dictionary
        /// </summary>
        public static Dictionary<string, string> LoadAllQueryParams(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Query params file not found: {filePath}");
            }

            var result = new Dictionary<string, string>();
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
                    var queryString = parts[1].Trim();
                    result[tc] = queryString;
                }
            }

            return result;
        }

        /// <summary>
        /// Clears the cache (useful for testing)
        /// </summary>
        public static void ClearCache()
        {
            _cachedParams.Clear();
        }

        /// <summary>
        /// Builds a complete URL with query parameters
        /// </summary>
        public static string BuildUrlWithQueryParams(string endpoint, string queryParams)
        {
            if (string.IsNullOrWhiteSpace(queryParams))
            {
                return endpoint;
            }

            var separator = endpoint.Contains("?") ? "&" : "?";
            return $"{endpoint}{separator}{queryParams}";
        }

        /// <summary>
        /// Builds a complete URL with query parameters from file
        /// </summary>
        public static string BuildUrlWithQueryParamsFromFile(string endpoint, string filePath, string tcNumber)
        {
            var queryParams = LoadQueryParams(filePath, tcNumber);
            return BuildUrlWithQueryParams(endpoint, queryParams);
        }

        /// <summary>
        /// Auto-detects and loads query parameters from inline string or file
        /// - If input ends with .txt, loads from file using tcNumber
        /// - If input is "none", returns empty dictionary
        /// - Otherwise, parses as inline query string (e.g., "page=1&status=active")
        /// </summary>
        /// <param name="input">File name, inline query string, or "none"</param>
        /// <param name="tcNumber">Test case number (required only for file loading)</param>
        /// <param name="apiName">API name for folder-based organization (optional)</param>
        /// <returns>Dictionary of query parameters</returns>
        public static Dictionary<string, string> LoadQueryParamsAuto(string input, string tcNumber = null, string apiName = null)
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
                    throw new ArgumentException("TC number is required when loading query params from file");
                }

                try
                {
                    // Build path with API folder if apiName is provided
                    string filePath;
                    if (!string.IsNullOrWhiteSpace(apiName))
                    {
                        // New structure: Data/QueryParams/{ApiName}/{file}
                        filePath = Path.Combine("Data", "QueryParams", apiName, input);
                    }
                    else
                    {
                        // Old structure for backward compatibility: Data/QueryParams/{file}
                        filePath = Path.Combine("Data", "QueryParams", input);
                    }

                    return LoadQueryParamsAsDictionary(filePath, tcNumber);
                }
                catch (FileNotFoundException)
                {
                    // File not found, return empty dictionary
                    return new Dictionary<string, string>();
                }
                catch (KeyNotFoundException)
                {
                    // TC not found in file, return empty dictionary
                    return new Dictionary<string, string>();
                }
            }

            // Parse as inline query string
            return ParseQueryString(input);
        }
    }
}
