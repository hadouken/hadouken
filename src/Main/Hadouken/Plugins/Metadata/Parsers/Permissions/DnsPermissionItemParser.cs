using System.Net;
using System.Security;
using System.Security.Permissions;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata.Parsers.Permissions
{
    public class DnsPermissionItemParser : IPermissionItemParser
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
