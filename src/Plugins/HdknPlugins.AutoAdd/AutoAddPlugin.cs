using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Plugins;
using Hadouken.Data;
using Hadouken.BitTorrent;
using Hadouken.IO;
using HdknPlugins.AutoAdd.Timers;
using Hadouken.Configuration;
using HdknPlugins.AutoAdd.Data.Models;
using System.Text.RegularExpressions;
using System.IO;
using NLog;

namespace HdknPlugins.AutoAdd
{
    [Plugin("autoadd", "1.0", ResourceBase = "HdknPlugins.AutoAdd.UI")]
    public class AutoAddPlugin : IPlugin
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IKeyValueStore _keyValueStore;
        private readonly IDataRepository _dataRepository;
        private readonly IBitTorrentEngine _bitTorrentEngine;
        private readonly IFileSystem _fileSystem;
        private readonly ITimerFactory _timerFactory;

        private ITimer _timer;

        public AutoAddPlugin(IKeyValueStore keyValueStore, IDataRepository dataRepository, IBitTorrentEngine bitTorrentEngine, IFileSystem fileSystem, ITimerFactory timerFactory)
        {
            _keyValueStore = keyValueStore;
            _dataRepository = dataRepository;
            _bitTorrentEngine = bitTorrentEngine;
            _fileSystem = fileSystem;
            _timerFactory = timerFactory;
        }

        public void Load()
        {
            var interval = _keyValueStore.Get("plugins.autoadd.pollInterval", 3000);

            _timer = _timerFactory.CreateTimer();
            _timer.SetCallback(interval, CheckFolders);
        }

        internal void CheckFolders()
        {
            var folders = _dataRepository.List<WatchedFolder>();

            if(folders == null)
                return;

            foreach(var folder in folders)
            {
                CheckFolder(folder);
            }
        }

        internal void CheckFolder(WatchedFolder folder)
        {
            string[] files = _fileSystem.GetFiles(folder.Path, "*.torrent");

            foreach(var file in files)
            {
                string fileName = Path.GetFileName(file);

                if(String.IsNullOrEmpty(fileName))
                    continue;

                if(!String.IsNullOrEmpty(folder.IncludeFilter) || !String.IsNullOrEmpty(folder.ExcludeFilter))
                {
                    bool excludeAdd = !(!String.IsNullOrEmpty(folder.ExcludeFilter) && !Regex.IsMatch(fileName, folder.ExcludeFilter));
                    bool includeAdd = !(!String.IsNullOrEmpty(folder.IncludeFilter) && !Regex.IsMatch(fileName, folder.IncludeFilter));

                    if(excludeAdd && includeAdd)
                    {
                        AddFile(folder, file);
                    }
                }
                else
                {
                    AddFile(folder, file);
                }
            }
        }

        internal void AddFile(WatchedFolder folder, string file)
        {
            try
            {
                // No filter, just add
                byte[] data = _fileSystem.ReadAllBytes(file);
                var manager = _bitTorrentEngine.AddTorrent(data);

                if(manager == null)
                    return;

                if (folder.AutoStart)
                    manager.Start();

                _fileSystem.DeleteFile(file);
            }
            catch(FileNotFoundException fileNotFoundException)
            {
                Logger.ErrorException(String.Format("File {0} not found", file), fileNotFoundException);
            }
            catch(IOException ioException)
            {
                Logger.ErrorException(String.Format("I/O exception when reading file {0}", file), ioException);
            }
        }

        public void Unload()
        {
            _timer.Stop();
        }
    }
}
