using System;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Logging;
using Hadouken.Common.Timers;
using Hadouken.Extensions.AutoAdd.Data;

namespace Hadouken.Extensions.AutoAdd {
    [Extension("plugin.autoadd",
        Name = "AutoAdd",
        Description = "Monitors folders and adds any torrent files it finds.",
        ResourceNamespace = "Hadouken.Extensions.AutoAdd.Resources",
        Scripts = new[] {
            "js/app.js",
            "js/controllers/settingsController.js",
            "js/controllers/upsertFolderController.js"
        }
        )]
    public class AutoAddPlugin : IPlugin {
        private readonly IFolderScanner _folderScanner;
        private readonly ILogger<AutoAddPlugin> _logger;
        private readonly IAutoAddRepository _repository;
        private readonly ITimer _timer;

        public AutoAddPlugin(ILogger<AutoAddPlugin> logger,
            ITimerFactory timerFactory,
            IAutoAddRepository repository,
            IFolderScanner folderScanner) {
            if (logger == null) {
                throw new ArgumentNullException("logger");
            }
            if (timerFactory == null) {
                throw new ArgumentNullException("timerFactory");
            }
            if (repository == null) {
                throw new ArgumentNullException("repository");
            }
            if (folderScanner == null) {
                throw new ArgumentNullException("folderScanner");
            }

            this._logger = logger;
            this._repository = repository;
            this._folderScanner = folderScanner;
            this._timer = timerFactory.Create(5000, this.CheckFolders);
        }

        public void Load() {
            this._timer.Start();
        }

        public void Unload() {
            this._timer.Stop();
        }

        internal void CheckFolders() {
            var folders = this._repository.GetFolders();

            foreach (var folder in folders) {
                try {
                    this._folderScanner.Scan(folder);
                }
                catch (Exception e) {
                    this._logger.Error(e, "Error when checking folder {Path}.", folder.Path);
                }
            }
        }
    }
}