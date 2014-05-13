using System.Net;
using System.Security;
using System.Security.Permissions;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata.Permissions
{
    public class SocketsPermissionParser : IPermissionParser
    {
        public IPermission Parse(JToken token)
        {
            return null;
        }

        public IPermission GetUnrestricted()
        {
            return new SocketPermission(PermissionState.Unrestricted);            
        }
    }
}
