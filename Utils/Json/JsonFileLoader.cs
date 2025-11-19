using System.IO;

namespace TafRestSharpWireMock.Tests.Utils.Json
{
    public static class JsonFileLoader
    {
        public static string LoadJsonFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("JSON file not found: " + filePath);
            return File.ReadAllText(filePath);
        }
    }
}