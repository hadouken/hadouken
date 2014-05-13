using System.Net.Sockets;
using Hadouken.Fx;
using Hadouken.Fx.Configuration;
using Hadouken.Fx.Logging;
using Hadouken.Plugins.Sample.Configuration;

namespace Hadouken.Plugins.Sample
{
    public class SamplePlugin : Plugin
    {
        private readonly ILogger _logger;
        private readonly IConfigurationRepository<SampleConfig> _configRepository;

        public SamplePlugin(ILogger logger, IConfigurationRepository<SampleConfig> configRepository)
        {
            _logger = logger;
            _configRepository = configRepository;
        }

        public override void Load()
        {
            _logger.Info("Getting config.");

            var config = _configRepository.Get();

            if (config == null)
            {
                _logger.Info("Creating new config");

                config = new SampleConfig
                {
                    Bar = 1337,
                    Foo = "Leet"
                };

                _configRepository.Set(config);
            }

            _logger.Info("Foo: " + config.Foo);

            _logger.Info("Connecting to google.com:80");
            new TcpClient().Connect("173.194.40.244", 80);
            _logger.Info("Connected successfully");
        }
    }
}
