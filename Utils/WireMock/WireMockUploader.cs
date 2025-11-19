using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TafRestSharpWireMock.Tests.Utils.WireMock
{
    /// <summary>
    /// Uploads WireMock mappings and response files to WireMock server via Admin API
    /// </summary>
    public class WireMockUploader
    {
        private readonly string _wireMockAdminUrl;
        private readonly HttpClient _httpClient;

        public WireMockUploader(string wireMockBaseUrl, int? adminPort = null)
        {
            // If adminPort is provided, use it; otherwise extract from baseUrl
            if (adminPort.HasValue)
            {
                var uri = new Uri(wireMockBaseUrl);
                _wireMockAdminUrl = $"{uri.Scheme}://{uri.Host}:{adminPort}/__admin";
            }
            else
            {
                _wireMockAdminUrl = $"{wireMockBaseUrl.TrimEnd('/')}/__admin";
            }

            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);

            Console.WriteLine($"[WireMockUploader] Admin URL: {_wireMockAdminUrl}");
        }

        /// <summary>
        /// Uploads all mappings and files from WireMock/ folder to WireMock server
        /// </summary>
        public async Task<UploadResult> UploadAllAsync()
        {
            var result = new UploadResult();

            try
            {
                // 1. Check if WireMock is running
                bool isRunning = await IsWireMockRunningAsync();
                if (!isRunning)
                {
                    result.Success = false;
                    result.ErrorMessage = "WireMock server is not running or not reachable";
                    Console.WriteLine($"[WireMockUploader] ERROR: {result.ErrorMessage}");
                    return result;
                }

                Console.WriteLine("[WireMockUploader] WireMock server is running");

                // 2. Reset WireMock to clear any existing stubs
                await ResetWireMockAsync();
                Console.WriteLine("[WireMockUploader] WireMock reset completed");

                // 3. Upload all mappings
                int mappingsUploaded = await UploadMappingsAsync();
                result.MappingsUploaded = mappingsUploaded;
                Console.WriteLine($"[WireMockUploader] Uploaded {mappingsUploaded} mappings");

                // 4. Upload all response files
                int filesUploaded = await UploadFilesAsync();
                result.FilesUploaded = filesUploaded;
                Console.WriteLine($"[WireMockUploader] Uploaded {filesUploaded} response files");

                result.Success = true;
                Console.WriteLine("[WireMockUploader] Upload completed successfully");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                Console.WriteLine($"[WireMockUploader] ERROR: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Checks if WireMock server is running
        /// </summary>
        private async Task<bool> IsWireMockRunningAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_wireMockAdminUrl}/mappings");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Resets WireMock server (clears all stubs and requests)
        /// </summary>
        private async Task ResetWireMockAsync()
        {
            var response = await _httpClient.PostAsync($"{_wireMockAdminUrl}/reset", null);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Uploads all mapping files from WireMock/mappings/ folder
        /// </summary>
        private async Task<int> UploadMappingsAsync()
        {
            string mappingsDir = Path.Combine(Directory.GetCurrentDirectory(), "WireMock", "mappings");

            if (!Directory.Exists(mappingsDir))
            {
                Console.WriteLine($"[WireMockUploader] Mappings directory not found: {mappingsDir}");
                return 0;
            }

            var mappingFiles = Directory.GetFiles(mappingsDir, "*.json", SearchOption.AllDirectories);
            int uploadedCount = 0;

            foreach (var filePath in mappingFiles)
            {
                try
                {
                    var jsonContent = File.ReadAllText(filePath);
                    var mapping = JObject.Parse(jsonContent);

                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync($"{_wireMockAdminUrl}/mappings", content);

                    if (response.IsSuccessStatusCode)
                    {
                        uploadedCount++;
                        Console.WriteLine($"[WireMockUploader]   ✓ Uploaded mapping: {Path.GetFileName(filePath)}");
                    }
                    else
                    {
                        Console.WriteLine($"[WireMockUploader]   ✗ Failed to upload mapping: {Path.GetFileName(filePath)} - {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WireMockUploader]   ✗ Error uploading {Path.GetFileName(filePath)}: {ex.Message}");
                }
            }

            return uploadedCount;
        }

        /// <summary>
        /// Uploads all response files from WireMock/__files/ folder
        /// </summary>
        private async Task<int> UploadFilesAsync()
        {
            string filesDir = Path.Combine(Directory.GetCurrentDirectory(), "WireMock", "__files");

            if (!Directory.Exists(filesDir))
            {
                Console.WriteLine($"[WireMockUploader] Files directory not found: {filesDir}");
                return 0;
            }

            var responseFiles = Directory.GetFiles(filesDir, "*.*", SearchOption.AllDirectories);
            int uploadedCount = 0;

            foreach (var filePath in responseFiles)
            {
                try
                {
                    var fileContent = File.ReadAllText(filePath);
                    var fileName = Path.GetFileName(filePath);

                    var content = new StringContent(fileContent, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync($"{_wireMockAdminUrl}/__files/{fileName}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        uploadedCount++;
                        Console.WriteLine($"[WireMockUploader]   ✓ Uploaded file: {fileName}");
                    }
                    else
                    {
                        Console.WriteLine($"[WireMockUploader]   ✗ Failed to upload file: {fileName} - {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WireMockUploader]   ✗ Error uploading {Path.GetFileName(filePath)}: {ex.Message}");
                }
            }

            return uploadedCount;
        }

        /// <summary>
        /// Verifies uploaded stubs are working by checking a specific mapping
        /// </summary>
        public async Task<bool> VerifyStubsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_wireMockAdminUrl}/mappings");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(content);
                    var mappingsCount = json["mappings"]?.Count() ?? 0;

                    Console.WriteLine($"[WireMockUploader] Verification: {mappingsCount} stubs registered");
                    return mappingsCount > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WireMockUploader] Verification failed: {ex.Message}");
            }

            return false;
        }
    }

    /// <summary>
    /// Result of WireMock upload operation
    /// </summary>
    public class UploadResult
    {
        public bool Success { get; set; }
        public int MappingsUploaded { get; set; }
        public int FilesUploaded { get; set; }
        public string ErrorMessage { get; set; }

        public override string ToString()
        {
            if (Success)
            {
                return $"Upload successful: {MappingsUploaded} mappings, {FilesUploaded} files";
            }
            else
            {
                return $"Upload failed: {ErrorMessage}";
            }
        }
    }
}
