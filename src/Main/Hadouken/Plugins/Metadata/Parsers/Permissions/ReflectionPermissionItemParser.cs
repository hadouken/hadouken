using System.Linq;
using System.Security;
using System.Security.Permissions;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata.Parsers.Permissions
{
    public class ReflectionPermissionItemParser : IPermissionItemParser
    {
        public IPermission Parse(JToken token)
        {
            var items = token as JArray;

            if (items == null)
            {
                return null;
            }

            var result = new ReflectionPermission(PermissionState.None);
            var flags = items.ToObject<string[]>();

            if (flags.Contains("memberAccess")) result.Flags |= ReflectionPermissionFlag.MemberAccess;
            if (flags.Contains("restrictedMemberAccess")) result.Flags |= ReflectionPermissionFlag.RestrictedMemberAccess;

            return result;
        }

        public IPermission GetUnrestricted()
        {
            return new ReflectionPermission(PermissionState.Unrestricted);
        }
    }
}
