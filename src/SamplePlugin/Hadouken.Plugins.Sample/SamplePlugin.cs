using System;
using System.IO;
using System.IO.IsolatedStorage;
using Hadouken.Fx;
using Hadouken.Fx.Logging;

namespace Hadouken.Plugins.Sample
{
    public class SamplePlugin : Plugin
    {
        private readonly ILogger _logger;

        public SamplePlugin(ILogger logger)
        {
            _logger = logger;
        }

        public override void Load()
        {
            _logger.Info("SamplePlugin loading.");

            using (var storage = IsolatedStorageFile.GetUserStoreForDomain())
            using (var file = storage.OpenFile(Guid.NewGuid().ToString(), FileMode.OpenOrCreate))
            using (var writer = new StreamWriter(file))
            {
                writer.WriteLine(AppDomain.CurrentDomain.FriendlyName);
            }
        }
    }
}
