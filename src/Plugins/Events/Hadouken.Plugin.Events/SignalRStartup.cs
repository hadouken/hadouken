using Microsoft.AspNet.SignalR;
using Owin;

namespace Hadouken.Plugins.Events
{
    public class SignalRStartup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var hubConfiguration = new HubConfiguration
            {
                EnableDetailedErrors = true
            };

            appBuilder.MapSignalR(hubConfiguration);
        }
    }
}
