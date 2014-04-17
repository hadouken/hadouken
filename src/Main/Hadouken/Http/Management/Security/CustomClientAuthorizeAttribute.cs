using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Hadouken.Http.Management.Security
{
    public class CustomClientAuthorizeAttribute : AuthorizeAttribute
    {
        public override bool AuthorizeHubConnection(HubDescriptor hubDescriptor, IRequest request)
        {
            var remoteIp = request.Environment["server.RemoteIpAddress"] as string;

            if (remoteIp == null)
            {
                return base.AuthorizeHubConnection(hubDescriptor, request);
            }

            return remoteIp == "127.0.0.1" || remoteIp == "::1";
        }
    }
}
