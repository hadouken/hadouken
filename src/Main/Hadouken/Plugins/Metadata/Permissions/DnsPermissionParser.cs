using System.Net;
using System.Security;
using System.Security.Permissions;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata.Permissions
{
    public class DnsPermissionParser : IPermissionParser
    {
        public IPermission Parse(JToken token)
        {
            return new DnsPermission(PermissionState.Unrestricted);
        }

        public IPermission GetUnrestricted()
        {
            return new DnsPermission(PermissionState.Unrestricted);
        }
    }
}
