// LICENSE: MIT
// Author: github.com/bryceg
// Modifications by github.com/vktr

using Autofac;
using Hadouken.Core.Security;
using Owin;

namespace Hadouken.Core.Http.WebSockets.Extensions {
    public static class OwinExtension {
        /// <summary>
        ///     Maps a static URI to a web socket consumer
        /// </summary>
        /// <typeparam name="T">Type of WebSocketHubConsumer</typeparam>
        /// <param name="app">Owin App</param>
        /// <param name="route">Static URI to map to the hub</param>
        /// <param name="lifetimeScope"></param>
        /// <param name="userManager"></param>
        public static IAppBuilder MapWebSocketRoute<T>(this IAppBuilder app, string route, ILifetimeScope lifetimeScope,
            IUserManager userManager)
            where T : WebSocketConnection {
            return app.Map(route, config => config.Use<WebSocketConnectionMiddleware<T>>(lifetimeScope, userManager));
        }
    }
}