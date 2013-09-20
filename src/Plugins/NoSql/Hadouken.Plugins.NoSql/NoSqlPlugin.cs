using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc.Hosting;

namespace Hadouken.Plugins.NoSql
{
    public class NoSqlPlugin : Plugin
    {
        private readonly IJsonRpcServer _jsonRpcServer;

        public NoSqlPlugin(IJsonRpcServer jsonRpcServer)
        {
            _jsonRpcServer = jsonRpcServer;
        }

        public override void Load()
        {
            _jsonRpcServer.Open();
        }

        public override void Unload()
        {
            _jsonRpcServer.Close();
        }
    }
}
