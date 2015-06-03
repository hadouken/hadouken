﻿using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Common.IO;
using Hadouken.Common.Messaging;
using Hadouken.Extensions.AutoAdd.Data;
using Hadouken.Extensions.AutoAdd.Data.Models;

namespace Hadouken.Extensions.AutoAdd {
    [Component]
    public class FolderScanner : IFolderScanner {
        private readonly IAutoAddRepository _autoAddRepository;
        private readonly IFileSystem _fileSystem;
        private readonly IKeyValueStore _keyValueStore;
        private readonly IMessageBus _messageBus;

        public FolderScanner(IFileSystem fileSystem,
            IKeyValueStore keyValueStore,
            IAutoAddRepository autoAddRepository,
            IMessageBus messageBus) {
            if (fileSystem == null) {
                throw new ArgumentNullException("fileSystem");
            }
            if (keyValueStore == null) {
                throw new ArgumentNullException("keyValueStore");
            }
            if (autoAddRepository == null) {
                throw new ArgumentNullException("autoAddRepository");
            }
            if (messageBus == null) {
                throw new ArgumentNullException("messageBus");
            }
            this._fileSystem = fileSystem;
            this._keyValueStore = keyValueStore;
            this._autoAddRepository = autoAddRepository;
            this._messageBus = messageBus;
        }

        public void Scan(Folder folder) {
            if (folder == null) {
                throw new ArgumentNullException("folder");
            }

            var dir = this._fileSystem.GetDirectory(folder.Path);
            if (!dir.Exists) {
                return;
            }

            var filter = this._keyValueStore.Get<string>("autoadd.filter");
            var files = dir.GetFiles(filter, folder.RecursiveSearch ? SearchScope.Recursive : SearchScope.Current);

            foreach (var file in from file in files let fileName = file.Path.GetFilename().FullPath where string.IsNullOrEmpty(folder.Pattern) || Regex.IsMatch(fileName, folder.Pattern) where this._autoAddRepository.GetHistoryByPath(file.Path.FullPath) == null select file) {
                this.AddFile(file, folder.Label);

                if (folder.RemoveSourceFile) {
                    file.Delete();
                }
            }
        }

        private void AddFile(IFile file, string label) {
            using (var stream = file.OpenRead()) {
                using (var ms = new MemoryStream()) {
                    stream.CopyTo(ms);
                    this._messageBus.Publish(new AddTorrentMessage(ms.ToArray()) {Label = label});
                }
            }

            // Add file to history
            var hist = new History {Path = file.Path.FullPath};
            this._autoAddRepository.CreateHistory(hist);
        }
    }
}