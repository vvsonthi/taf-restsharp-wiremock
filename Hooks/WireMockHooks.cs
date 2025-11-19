using System;
using System.Threading.Tasks;
using Reqnroll;
using TafRestSharpWireMock.Tests.Utils.Configuration;
using TafRestSharpWireMock.Tests.Utils.WireMock;

namespace TafRestSharpWireMock.Tests.Hooks
{
    [Binding]
    public class WireMockHooks
    {
        private static bool _wireMockUploaded = false;

        /// <summary>
        /// Runs before the test run starts
        /// Uploads WireMock mappings and files if UseWireMock is enabled
        /// </summary>
        [BeforeTestRun(Order = 1)]
        public static void SetupWireMock()
        {
            Console.WriteLine("=======================================================");
            Console.WriteLine("[WireMockHooks] BeforeTestRun - Starting WireMock Setup");
            Console.WriteLine("=======================================================");

            try
            {
                var config = ConfigurationManager.Instance;
                bool useWireMock = config.GetConfigValue<bool>("UseWireMock");

                if (!useWireMock)
                {
                    Console.WriteLine("[WireMockHooks] UseWireMock=false, skipping WireMock setup");
                    return;
                }

                Console.WriteLine("[WireMockHooks] UseWireMock=true, proceeding with WireMock setup");

                // Get WireMock configuration
                int wireMockPort = config.GetConfigValue<int>("WireMock:Port");
                int? adminPort = null;

                try
                {
                    adminPort = config.GetConfigValue<int>("WireMock:AdminPort");
                }
                catch
                {
                    // AdminPort not configured, will use same port
                }

                string wireMockBaseUrl = $"http://localhost:{wireMockPort}";
                Console.WriteLine($"[WireMockHooks] WireMock Base URL: {wireMockBaseUrl}");
                Console.WriteLine($"[WireMockHooks] WireMock Admin Port: {adminPort?.ToString() ?? "Same as base port"}");

                // Upload mappings and files
                var uploader = new WireMockUploader(wireMockBaseUrl, adminPort);
                var result = UploadWireMockFilesAsync(uploader).GetAwaiter().GetResult();

                if (result.Success)
                {
                    Console.WriteLine("=======================================================");
                    Console.WriteLine("[WireMockHooks] WireMock Setup Complete!");
                    Console.WriteLine($"[WireMockHooks] ✓ {result.MappingsUploaded} mappings uploaded");
                    Console.WriteLine($"[WireMockHooks] ✓ {result.FilesUploaded} response files uploaded");
                    Console.WriteLine("=======================================================");
                    _wireMockUploaded = true;
                }
                else
                {
                    Console.WriteLine("=======================================================");
                    Console.WriteLine("[WireMockHooks] WireMock Setup FAILED!");
                    Console.WriteLine($"[WireMockHooks] Error: {result.ErrorMessage}");
                    Console.WriteLine("=======================================================");

                    // Decide whether to fail tests or continue
                    bool failOnWireMockError = config.GetConfigValue<bool>("WireMock:FailOnUploadError", false);
                    if (failOnWireMockError)
                    {
                        throw new Exception($"WireMock setup failed: {result.ErrorMessage}");
                    }
                    else
                    {
                        Console.WriteLine("[WireMockHooks] WARNING: Continuing tests despite WireMock setup failure");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("=======================================================");
                Console.WriteLine("[WireMockHooks] EXCEPTION during WireMock setup");
                Console.WriteLine($"[WireMockHooks] {ex.Message}");
                Console.WriteLine($"[WireMockHooks] {ex.StackTrace}");
                Console.WriteLine("=======================================================");
                throw;
            }
        }

        /// <summary>
        /// Async helper to upload WireMock files
        /// </summary>
        private static async Task<UploadResult> UploadWireMockFilesAsync(WireMockUploader uploader)
        {
            var result = await uploader.UploadAllAsync();

            // Verify stubs are loaded
            if (result.Success)
            {
                bool verified = await uploader.VerifyStubsAsync();
                if (!verified)
                {
                    Console.WriteLine("[WireMockHooks] WARNING: Stubs uploaded but verification failed");
                }
            }

            return result;
        }

        /// <summary>
        /// Runs after the test run completes
        /// </summary>
        [AfterTestRun(Order = 1)]
        public static void TearDownWireMock()
        {
            Console.WriteLine("=======================================================");
            Console.WriteLine("[WireMockHooks] AfterTestRun - WireMock Teardown");
            Console.WriteLine("=======================================================");

            if (_wireMockUploaded)
            {
                Console.WriteLine("[WireMockHooks] WireMock was used during this test run");
                Console.WriteLine("[WireMockHooks] Note: WireMock server is still running (not stopped by framework)");
                Console.WriteLine("[WireMockHooks] To stop WireMock, manually stop the server or container");
            }
            else
            {
                Console.WriteLine("[WireMockHooks] WireMock was not used during this test run");
            }

            Console.WriteLine("=======================================================");
        }

        /// <summary>
        /// Helper method to check if WireMock is properly set up
        /// Can be called from feature files if needed
        /// </summary>
        public static bool IsWireMockReady()
        {
            return _wireMockUploaded;
        }
    }
}
