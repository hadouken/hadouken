using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Autofac;
using Hadouken.Common.Data;
using Hadouken.Common.Logging;
using Hadouken.Core.Http.WebSockets.Extensions;
using Hadouken.Core.Security;
using Microsoft.Owin.Builder;
using Microsoft.Owin.Hosting;
using Nancy.Bootstrapper;
using Nowin;
using Owin;

namespace Hadouken.Core.Http {
    public class HttpServer : IHttpServer {
// ReSharper disable once UnusedField.Compiler
// ReSharper disable once InconsistentNaming
        private static readonly StartOptions __startOptions;
        private readonly INancyBootstrapper _bootstrapper;
        private readonly IKeyValueStore _keyValueStore;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ILogger<HttpServer> _logger;
        private readonly IUserManager _userManager;
        private INowinServer _nowinServer;

        public HttpServer(ILogger<HttpServer> logger,
            INancyBootstrapper bootstrapper,
            IKeyValueStore keyValueStore,
            ILifetimeScope lifetimeScope,
            IUserManager userManager) {
            if (logger == null) {
                throw new ArgumentNullException("logger");
            }
            if (bootstrapper == null) {
                throw new ArgumentNullException("bootstrapper");
            }
            if (keyValueStore == null) {
                throw new ArgumentNullException("keyValueStore");
            }
            if (lifetimeScope == null) {
                throw new ArgumentNullException("lifetimeScope");
            }
            if (userManager == null) {
                throw new ArgumentNullException("userManager");
            }
            this._logger = logger;
            this._bootstrapper = bootstrapper;
            this._keyValueStore = keyValueStore;
            this._lifetimeScope = lifetimeScope;
            this._userManager = userManager;
        }

        public void Start() {
            var binding = this._keyValueStore.Get("http.binding", "127.0.0.1");
            var port = this._keyValueStore.Get("http.port", 7890);

            if (binding == "localhost") {
                binding = "127.0.0.1";
            }
            if (binding == "+") {
                binding = "0.0.0.0";
            }

            var address = IPAddress.Parse(binding);
            var endPoint = new IPEndPoint(address, port);

            var certificateFile = this._keyValueStore.Get<string>("http.x509.file");
            var certificatePassword = this._keyValueStore.Get<string>("http.x509.password");
            X509Certificate2 certificate = null;

            if (!string.IsNullOrEmpty(certificateFile)) {
                try {
                    certificate = string.IsNullOrEmpty(certificatePassword)
                        ? new X509Certificate2(certificateFile)
                        : new X509Certificate2(certificateFile, certificatePassword);
                }
                catch (Exception exception) {
                    this._logger.Warn(exception, "Failed to load X509 certificate.");
                }
            }

            try {
                var app = this.BuildOwinApp();
                var builder = ServerBuilder
                    .New()
                    .SetOwinApp(app.Build())
                    .SetOwinCapabilities((IDictionary<string, object>) app.Properties[OwinKeys.ServerCapabilitiesKey])
                    .SetEndPoint(endPoint);

                if (certificate != null) {
                    builder.SetCertificate(certificate);
                }

                this._nowinServer = builder.Build();

                Task.Run(() => this._nowinServer.Start());

                var protocol = certificate != null ? "https" : "http";
                this._logger.Info("HTTP server accepting connections on {Url}.",
                    string.Format("{0}://{1}", protocol, endPoint));
            }
            catch (Exception exception) {
                this._logger.Error(exception, "Failed to start HTTP server.");
            }
        }

        public void Stop() {
            this._nowinServer.Dispose();
        }

        private IAppBuilder BuildOwinApp() {
            var builder = new AppBuilder();
            OwinServerFactory.Initialize(builder.Properties);

            builder
                .MapWebSocketRoute<EventStreamServer>("/events", this._lifetimeScope, this._userManager)
                .UseNancy(nancyOptions => nancyOptions.Bootstrapper = this._bootstrapper);

            return builder;
        }
    }
}