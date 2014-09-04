using System;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Logging;
using Hadouken.Common.Timers;
using Hadouken.Extensions.AutoAdd.Data;

namespace Hadouken.Extensions.AutoAdd
{
    [Extension("plugin.autoadd",
        Name = "AutoAdd",
        Description = "Monitors folders and adds any torrent files it finds.",
        ResourceNamespace = "Hadouken.Extensions.AutoAdd.Resources",
        Scripts = new[]
        {
            "js/app.js",
            "js/controllers/settingsController.js",
            "js/controllers/upsertFolderController.js"
        }
    )]
    public class AutoAddPlugin : IPlugin
    {
        private readonly ILogger _logger;
        private readonly IAutoAddRepository _repository;
        private readonly IFolderScanner _folderScanner;
        private readonly ITimer _timer;

        public AutoAddPlugin(ILogger logger,
            ITimerFactory timerFactory,
            IAutoAddRepository repository,
            IFolderScanner folderScanner)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (timerFactory == null) throw new ArgumentNullException("timerFactory");
            if (repository == null) throw new ArgumentNullException("repository");
            if (folderScanner == null) throw new ArgumentNullException("folderScanner");

            _logger = logger;
            _repository = repository;
            _folderScanner = folderScanner;
            _timer = timerFactory.Create(5000, CheckFolders);
        }

        public void Load()
        {
            _timer.Start();
        }

        public void Unload()
        {
            _timer.Stop();
        }

        private void CheckFolders()
        {
            var folders = _repository.GetFolders();

            foreach (var folder in folders)
            {
                try
                {
                    _folderScanner.Scan(folder);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error when checking folder {Path}.", folder.Path);
                }
            }
        }
    }
}
