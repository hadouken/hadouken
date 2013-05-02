using System;
using System.Linq;
using Hadouken.Common;
using Hadouken.Configuration;
using System.Net;
using NLog;

namespace Hadouken.Impl
{
    [Component]
    public class HostEnvironment : IEnvironment
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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

        private string GetBinding()
        {
            Logger.Trace("Building Uri binding.");

            var binding = "http://+:{port}/";
            var port = _registryReader.ReadInt("webui.port", 8080);

            // Allow overriding from application configuration file.
            if (HdknConfig.ConfigManager.AllKeys.Contains("WebUI.Binding"))
                binding = HdknConfig.ConfigManager["WebUI.Binding"];

            if (HdknConfig.ConfigManager.AllKeys.Contains("WebUI.Port"))
                port = Convert.ToInt32(HdknConfig.ConfigManager["WebUI.Port"]);

            binding = binding.Replace("{port}", port.ToString());

            Logger.Debug("Built binding {0}", binding);

            return binding;
        }

        public string ConnectionString
        {
            get { return HdknConfig.ConnectionString.Replace("$Paths.Data$", HdknConfig.GetPath("Paths.Data")); }
        }

        public string HttpBinding { get; private set; }

        public NetworkCredential HttpCredentials { get; private set; }
    }
}
