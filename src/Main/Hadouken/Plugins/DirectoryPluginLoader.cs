using System;
using System.IO;
using System.Net;
using Hadouken.Configuration;
using Hadouken.Framework;
using Hadouken.Framework.Rpc;
using Hadouken.IO;
using Hadouken.Plugins.Metadata;

namespace Hadouken.Plugins
{
    public class DirectoryPluginLoader : IPluginLoader
    {
        private readonly IConfiguration _configuration;
        private readonly IFileSystem _fileSystem;
        private readonly IJsonRpcClient _rpcClient;

        public DirectoryPluginLoader(IConfiguration configuration, IFileSystem fileSystem, IJsonRpcClient rpcClient)
        {
            _configuration = configuration;
            _fileSystem = fileSystem;
            _rpcClient = rpcClient;
        }

        public bool CanLoad(string path)
        {
            return _fileSystem.IsDirectory(path);
        }

        public IPluginManager Load(string path)
        {
            var manifestPath = Path.Combine(path, "manifest.json");

            if (!_fileSystem.FileExists(manifestPath))
                return null;

            using (var manifestJson = _fileSystem.OpenRead(manifestPath))
            {
                IManifest manifest;

                if (Manifest.TryParse(manifestJson, out manifest))
                {
                    var pluginDataPath = Path.Combine(_configuration.ApplicationDataPath, manifest.Name);

                    if (!_fileSystem.DirectoryExists(pluginDataPath))
                        _fileSystem.CreateDirectory(pluginDataPath);

                    var bootConfig = new BootConfig
                    {
                        DataPath = pluginDataPath,
                        HostBinding = _configuration.Http.HostBinding,
                        Port = _configuration.Http.Port,
                        UserName = _configuration.Http.Authentication.UserName,
                        Password = _configuration.Http.Authentication.Password,
                        RpcGatewayUri = _configuration.Rpc.GatewayUri,
                        RpcPluginUri = String.Format(_configuration.Rpc.PluginUri, manifest.Name),
                        HttpVirtualPath = "/plugins/" + manifest.Name
                    };

                    return new PluginManager(path, manifest, _fileSystem, bootConfig, _rpcClient);
                }
            }

            return null;
        }
    }
}
