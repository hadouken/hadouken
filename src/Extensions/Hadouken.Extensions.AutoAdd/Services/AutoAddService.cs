using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.JsonRpc;
using Hadouken.Extensions.AutoAdd.Data;
using Hadouken.Extensions.AutoAdd.Data.Models;

namespace Hadouken.Extensions.AutoAdd.Services
{
    public sealed class AutoAddService : IJsonRpcService
    {
        private readonly IAutoAddRepository _autoAddRepository;

        public AutoAddService(IAutoAddRepository autoAddRepository)
        {
            if (autoAddRepository == null) throw new ArgumentNullException("autoAddRepository");
            _autoAddRepository = autoAddRepository;
        }

        [JsonRpcMethod("autoadd.folders.create")]
        public Folder CreateFolder(Folder folder)
        {
            if (folder == null) throw new ArgumentNullException("folder");

            _autoAddRepository.CreateFolder(folder);
            return folder;
        }

        [JsonRpcMethod("autoadd.folders.delete")]
        public void DeleteFolder(int folderId)
        {
            _autoAddRepository.DeleteFolder(folderId);
        }

        [JsonRpcMethod("autoadd.folders.getAll")]
        public IEnumerable<Folder> GetFolders()
        {
            return _autoAddRepository.GetFolders() ?? Enumerable.Empty<Folder>();
        }

        [JsonRpcMethod("autoadd.folders.update")]
        public void UpdateFolder(Folder folder)
        {
            if (folder == null) throw new ArgumentNullException("folder");
            _autoAddRepository.UpdateFolder(folder);
        }
    }
}
