using WireMock.Server;
using WireMock.Settings;

namespace TafRestSharpWireMock.Tests.Utils.WireMock
{
    public class WireMockFixture
    {
        public WireMockServer Server { get; private set; }

        public void Start(int port)
        {
            Server = WireMockServer.Start(new WireMockServerSettings
            {
                Port = port,
                ReadStaticMappings = true,
                WatchStaticMappings = true
            });
        }

        public void Stop()
        {
            Server?.Stop();
        }
    }
}