using System;
using Autofac;
using Autofac.Core;
using Hadouken.Framework;
using Hadouken.Framework.Events;

namespace Hadouken.Plugins.HttpJsonRpc
{
    public class PluginBootstrapper : DefaultBootstrapper
    {
        public override void RegisterDependencies(ContainerBuilder builder)
        {
            var listenUri = String.Format("http://{0}:{1}/jsonrpc/", Configuration.HostBinding, Configuration.Port);
            
            builder
                .RegisterType<HttpJsonRpcServer>()
                .As<IHttpJsonRpcServer>()
                .WithParameter("listenUri", listenUri)
                .OnActivating(SetCredentials);
        }

        private void SetCredentials(IActivatingEventArgs<IHttpJsonRpcServer> obj)
        {
            obj.Instance.SetCredentials(Configuration.UserName, Configuration.Password);
        }
    }
}
