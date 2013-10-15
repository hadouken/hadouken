using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Hadouken.Framework.Http;
using Hadouken.Framework.Http.Media;

namespace Hadouken.Framework.DI
{
    public class HttpFileServerModule : Module
    {
        private readonly string _listenUri;
        private readonly string _baseDirectory;

        public HttpFileServerModule(string listenUri, string baseDirectory)
        {
            _listenUri = listenUri;
            _baseDirectory = baseDirectory;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MediaTypeFactory>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<HttpFileServer>()
                .WithParameter("listenUri", _listenUri)
                .WithParameter("baseDirectory", _baseDirectory)
                .AsImplementedInterfaces()
                .OnActivated(SetCredentials)
                .SingleInstance();
        }

        private void SetCredentials(IActivatedEventArgs<HttpFileServer> args)
        {
            var config = args.Context.Resolve<IBootConfig>();
            args.Instance.SetCredentials(config.UserName, config.Password);
        }
    }
}
