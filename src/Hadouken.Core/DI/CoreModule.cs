using Autofac;
using Hadouken.Common.JsonRpc;
using Hadouken.Core.BitTorrent;
using Hadouken.Core.BitTorrent.Data;
using Hadouken.Core.BitTorrent.Handlers;
using Hadouken.Core.Data;
using Hadouken.Core.Handlers;
using Hadouken.Core.Http;
using Hadouken.Core.JsonRpc;
using Hadouken.Core.Security;
using Hadouken.Core.Services;
using Nancy.Bootstrapper;
using Ragnar;

namespace Hadouken.Core.DI
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // BitTorrent stuff
            builder.RegisterType<Session>().As<ISession>().SingleInstance().ExternallyOwned();
            builder.RegisterType<SessionHandler>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<TorrentInfoRepository>().As<ITorrentInfoRepository>().SingleInstance();
            builder.RegisterType<TorrentMetadataRepository>().As<ITorrentMetadataRepository>().SingleInstance();

            // BitTorrent message handlers
            builder.RegisterType<AddUrlHandler>().AsImplementedInterfaces();
            builder.RegisterType<AddTorrentHandler>().AsImplementedInterfaces();
            builder.RegisterType<ChangeTorrentLabelHandler>().AsImplementedInterfaces();
            builder.RegisterType<MoveTorrentHandler>().AsImplementedInterfaces();
            builder.RegisterType<PauseTorrentHandler>().AsImplementedInterfaces();
            builder.RegisterType<RemoveTorrentHandler>().AsImplementedInterfaces();
            builder.RegisterType<ResumeTorrentHandler>().AsImplementedInterfaces();
            builder.RegisterType<SessionSettingsChangedHandler>().AsImplementedInterfaces();

            // Notification things
            builder.RegisterType<NotifierEngine>().As<INotifierEngine>().SingleInstance();

            // JSONRPC host
            builder.RegisterType<RequestHandler>().As<IRequestHandler>().SingleInstance();
            builder.RegisterType<JsonRpcRequestParser>().As<IJsonRpcRequestParser>().SingleInstance();
            builder.RegisterType<MethodCacheBuilder>().As<IMethodCacheBuilder>();
            builder.RegisterType<ByNameResolver>().As<IParameterResolver>();
            builder.RegisterType<ByPositionResolver>().As<IParameterResolver>();
            builder.RegisterType<NullResolver>().As<IParameterResolver>();

            // JSONRPC services
            builder.RegisterType<BitTorrentService>().As<IJsonRpcService>();
            builder.RegisterType<ConfigurationService>().As<IJsonRpcService>();
            builder.RegisterType<LoggingService>().As<IJsonRpcService>();
            builder.RegisterType<NotificationService>().As<IJsonRpcService>();
            builder.RegisterType<UserServices>().As<IJsonRpcService>();

            // Message handlers
            builder.RegisterType<NotifyTorrentAddedHandler>().AsImplementedInterfaces();
            builder.RegisterType<NotifyTorrentCompletedHandler>().AsImplementedInterfaces();

            // HTTP
            builder.RegisterType<HttpServer>().As<IHttpServer>().SingleInstance();
            builder.RegisterType<CustomNancyBootstrapper>().As<INancyBootstrapper>().SingleInstance();

            // Repositories
            builder.RegisterType<NotifierRepository>().As<INotifierRepository>().SingleInstance();
            builder.RegisterType<UserRepository>().As<IUserRepository>().SingleInstance();

            // Security
            builder.RegisterType<UserManager>().As<IUserManager>().SingleInstance();

            // The main service
            builder.RegisterType<HadoukenService>().As<IHadoukenService>().SingleInstance();
        }
    }
}
