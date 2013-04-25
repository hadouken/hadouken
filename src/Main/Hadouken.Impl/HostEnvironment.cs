using System;
using System.Linq;
using Hadouken.Common;
using Hadouken.Configuration;
using System.Net;

namespace Hadouken.Impl
{
    [Component]
    public class HostEnvironment : IEnvironment
    {
        private readonly IRegistryReader _registryReader;

        public HostEnvironment(IRegistryReader registryReader)
        {
            _registryReader = registryReader;

            Load();
        }

        private void Load()
        {
            HttpBinding = GetBinding();
        }

        private Uri GetBinding()
        {
            var binding = _registryReader.ReadString("http.binding", "http://localhost:{port}/");
            var port = _registryReader.ReadInt("http.port", 8081);

            // Allow overriding from application configuration file.
            if (HdknConfig.ConfigManager.AllKeys.Contains("WebUI.Binding"))
                binding = HdknConfig.ConfigManager["WebUI.Binding"];

            if (HdknConfig.ConfigManager.AllKeys.Contains("WebUI.Port"))
                port = Convert.ToInt32(HdknConfig.ConfigManager["WebUI.Port"]);

            return new Uri(binding.Replace("{port}", port.ToString()));
        }

        public string ConnectionString
        {
            get { return HdknConfig.ConnectionString; }
        }

        public Uri HttpBinding { get; private set; }

        public NetworkCredential HttpCredentials { get; private set; }
    }
}
