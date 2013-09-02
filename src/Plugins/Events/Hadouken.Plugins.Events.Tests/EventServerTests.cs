using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using NUnit.Framework;

namespace Hadouken.Plugins.Events.Tests
{
    public class EventServerTests
    {
        [Test]
        public void Start_WithValidUrl_OpensServer()
        {
            var eventServer = new EventServer("http://localhost:4567/");
            
            Assert.DoesNotThrow(() =>
            {
                eventServer.Start();
                eventServer.Stop(); 
            });
        }

        [Test]
        public async void Start_WithValidUrl_AcceptsSignalRClients()
        {
            var resetEvent = new ManualResetEvent(false);
            var eventServer = new EventServer("http://localhost:4567/");
            eventServer.Start();

            var hubConnection = new HubConnection("http://localhost:4567/");
            var proxyCaller = hubConnection.CreateHubProxy("events");
            var proxyListener = hubConnection.CreateHubProxy("events");

            proxyListener.On("eventPublished", () => resetEvent.Set());

            await hubConnection.Start();

            await proxyCaller.Invoke("publish", "test", new {data = "Foo"});

            resetEvent.WaitOne();
            eventServer.Stop();
        }
    }
}
