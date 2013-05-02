using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common;
using Hadouken.Common.Plugins;
using Hadouken.Common.Messaging;
using Hadouken.Common.Data;

using HdknPlugins.AutoAdd.Timers;
using HdknPlugins.AutoAdd.Data.Models;
using Hadouken.Common.IO;
using Hadouken.Common.BitTorrent;
using Migrator.Providers.SQLite;
using NLog;
using System.Text.RegularExpressions;

namespace HdknPlugins.AutoAdd
{
    public class AutoAddPlugin : Plugin
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IEnvironment _environment;
        private readonly IDataRepository _dataRepository;
        private readonly IFileSystem _fileSystem;
        private readonly ITimer _timer;

        public AutoAddPlugin(IEnvironment environment,
                             IMessageBus messageBus,
                             IDataRepository dataRepository,
                             IFileSystem fileSystem,
                             ITimerFactory timerFactory)
            : base(messageBus)
        {
            _environment = environment;
            _dataRepository = dataRepository;
            _fileSystem = fileSystem;
            _timer = timerFactory.CreateTimer();
        }

        public override void Load()
        {
            Logger.Trace("Load()");

            var m =
                new Migrator.Migrator(
                    new SQLiteTransformationProvider(new SQLiteDialect(), _environment.ConnectionString),
                    this.GetType().Assembly, false);

            Logger.Debug("Updating all migrations in current assembly");

            m.MigrateToLastVersion();

            _timer.SetCallback(5000, CheckFolders);
            _timer.Start();
        }

        private void CheckFolders()
        {
            var folders = _dataRepository.List<Folder>();

            foreach (var folder in folders)
            {
                CheckFolder(folder);
            }
        }

        internal void CheckFolder(Folder folder)
        {
            var files = _fileSystem.GetFiles(folder.Path, "*.torrent");

            foreach (var file in files)
            {
                string fileName = System.IO.Path.GetFileName(file);

                if (String.IsNullOrEmpty(fileName))
                    continue;

                if (!String.IsNullOrEmpty(folder.IncludeFilter) || !String.IsNullOrEmpty(folder.ExcludeFilter))
                {
                    bool include =
                        !(!String.IsNullOrEmpty(folder.IncludeFilter) && !Regex.IsMatch(fileName, folder.IncludeFilter));

                    bool exclude =
                        !(!String.IsNullOrEmpty(folder.ExcludeFilter) && !Regex.IsMatch(fileName, folder.ExcludeFilter));

                    if (include && exclude)
                    {
                        AddFile(file, folder.Label, folder.AutoStart);
                    }
                }
                else
                {
                    AddFile(file, folder.Label, folder.AutoStart);
                }
            }
        }

        internal void AddFile(string file, string label, bool autoStart)
        {
            var data = _fileSystem.ReadAllBytes(file);

            var msg = new AddTorrentMessage
                {
                    AutoStart = autoStart,
                    Data = data,
                    Label = label
                };

            MessageBus.Publish(msg);

            _fileSystem.DeleteFile(file);
        }

        public override void Unload()
        {
            Logger.Trace("Unload()");

            _timer.Stop();
        }
    }
}
