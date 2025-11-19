using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace TafRestSharpWireMock.Tests.Utils.Configuration
{
    public class ConfigurationManager
    {
        private static ConfigurationManager _instance;
        private static readonly object _lock = new object();
        private readonly IConfiguration _configuration;

        private ConfigurationManager()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public static ConfigurationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ConfigurationManager();
                        }
                    }
                }
                return _instance;
            }
        }

        public string CurrentEnvironment => _configuration["env"] ?? "dev";

        public bool UseWireMock => bool.Parse(_configuration["UseWireMock"] ?? "false");

        public int WireMockPort => int.Parse(_configuration["WireMock:Port"] ?? "9091");

        public string UserApiBaseUrl => _configuration["UserApi:BaseUrl"];

        public int UserApiTimeout => int.Parse(_configuration["UserApi:TimeoutInMs"] ?? "8000");

        public string UserApiRequestFilePath => _configuration["UserApi:RequestFilePath"];

        public string UserApiResponseFilePath => _configuration["UserApi:ResponseFilePath"];

        public string GetApiKey()
        {
            return _configuration[$"{CurrentEnvironment}:ApiKey"];
        }

        public string GetConfigValue(string key)
        {
            return _configuration[key];
        }

        public T GetConfigSection<T>(string sectionName) where T : new()
        {
            var section = new T();
            _configuration.GetSection(sectionName).Bind(section);
            return section;
        }
    }
}
