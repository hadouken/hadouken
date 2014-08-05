using System;
using System.Collections.Generic;
using Hadouken.Common.JsonRpc;
using Hadouken.Extensions.AutoAdd.Data;
using Hadouken.Extensions.AutoAdd.Data.Models;

namespace Hadouken.Extensions.AutoAdd.Services
{
    public sealed class AutoAddService : IJsonRpcService
    {
        private readonly IAutoAddRepository _repository;

        public AutoAddService(IAutoAddRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            _repository = repository;
        }

        [JsonRpcMethod("autoadd.folders.create")]
        public Folder CreateFolder(Folder folder)
        {
            _repository.CreateFolder(folder);
            return folder;
        }

        [JsonRpcMethod("autoadd.folders.getAll")]
        public IEnumerable<Folder> GetFolders()
        {
            return _repository.GetFolders();
        }
    }
}
