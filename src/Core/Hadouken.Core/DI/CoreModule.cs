using Autofac;
using Hadouken.Common.BitTorrent;
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
            builder.Register<ISession>(c =>
            {
                var v = typeof (AssemblyInformation).Assembly.GetName().Version;
                var fingerprint = new Fingerprint("HA", v.Major, v.Minor, v.Build, v.Revision);
                var session = new Session(fingerprint);

                using (var settings = session.QuerySettings())
                {
                    settings.UserAgent = "Hadouken/" + v;
                    session.SetSettings(settings);
                }

                return session;
            }).SingleInstance().ExternallyOwned();

            builder.Register(c => c.Resolve<ISession>().Alerts).SingleInstance().ExternallyOwned();
            builder.RegisterType<AlertBus>().As<IAlertBus>().SingleInstance();
            builder.RegisterType<SessionHandler>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<TorrentEngine>().As<ITorrentEngine>().SingleInstance();
            builder.RegisterType<TorrentInfoRepository>().As<ITorrentInfoRepository>().SingleInstance();
            builder.RegisterType<TorrentManager>().As<ITorrentManager>().SingleInstance();
            builder.RegisterType<TorrentMetadataRepository>().As<ITorrentMetadataRepository>().SingleInstance();

            // BitTorrent message handlers
            builder.RegisterType<AddUrlHandler>().AsImplementedInterfaces();
            builder.RegisterType<AddTorrentHandler>().AsImplementedInterfaces();
            builder.RegisterType<ChangeFilePriorityHandler>().AsImplementedInterfaces();
            builder.RegisterType<ChangeTorrentLabelHandler>().AsImplementedInterfaces();
            builder.RegisterType<ChangeTorrentSettingsHandler>().AsImplementedInterfaces();
            builder.RegisterType<ClearTorrentErrorHandler>().AsImplementedInterfaces();
            builder.RegisterType<MoveTorrentHandler>().AsImplementedInterfaces();
            builder.RegisterType<PauseTorrentHandler>().AsImplementedInterfaces();
            builder.RegisterType<QueuePositionBottomHandler>().AsImplementedInterfaces();
            builder.RegisterType<QueuePositionDownHandler>().AsImplementedInterfaces();
            builder.RegisterType<QueuePositionTopHandler>().AsImplementedInterfaces();
            builder.RegisterType<QueuePositionUpHandler>().AsImplementedInterfaces();
            builder.RegisterType<RemoveTorrentHandler>().AsImplementedInterfaces();
            builder.RegisterType<RenameTorrentFileHandler>().AsImplementedInterfaces();
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
            builder.RegisterType<CoreService>().As<IJsonRpcService>();
            builder.RegisterType<ConfigurationService>().As<IJsonRpcService>();
            builder.RegisterType<FileSystemService>().As<IJsonRpcService>();
            builder.RegisterType<LoggingService>().As<IJsonRpcService>();
            builder.RegisterType<NotificationService>().As<IJsonRpcService>();
            builder.RegisterType<UserServices>().As<IJsonRpcService>();

            // Message handlers
            builder.RegisterType<NotifyTorrentAddedHandler>().AsImplementedInterfaces();
            builder.RegisterType<NotifyTorrentCompletedHandler>().AsImplementedInterfaces();

            // HTTP
            builder.RegisterType<EventStreamServer>().AsSelf();
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
