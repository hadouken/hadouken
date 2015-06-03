using Hadouken.Tools.Posh.Commands;
using Hadouken.Tools.Posh.IO;
using Hadouken.Tools.Posh.Net;

namespace Hadouken.Tools.Posh {
    internal static class ServiceLocator {
        private static readonly TinyIoCContainer Container;

        static ServiceLocator() {
            Container = RegisterComponents();
        }

        public static T Get<T>() where T : class {
            return Container.Resolve<T>();
        }

        private static TinyIoCContainer RegisterComponents() {
            var container = new TinyIoCContainer();

            // Cmdlets
            container.Register<IAddTorrentCommand, AddTorrentCommand>();
            container.Register<IGetTorrentCommand, GetTorrentCommand>();

            // IO
            container.Register<IFileSystem, FileSystem>();

            // Net
            container.Register<IJsonRpcClient, JsonRpcClient>();
            container.Register<IHttpClient, HttpClientWrapper>();

            return container;
        }
    }
}