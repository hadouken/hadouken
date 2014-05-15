using System.Net;
using System.Security;
using System.Security.Permissions;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata.Parsers.Permissions
{
    public class SocketsPermissionItemParser : IPermissionItemParser
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
