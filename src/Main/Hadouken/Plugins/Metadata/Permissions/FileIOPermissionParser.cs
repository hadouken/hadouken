using System.Linq;
using System.Security;
using System.Security.Permissions;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata.Permissions
{
    public class FileIOPermissionParser : IPermissionParser
    {
        public IPermission Parse(JToken token)
        {
            var accessList = token as JArray;

            if (accessList == null)
            {
                return null;
            }

            var result = new FileIOPermission(PermissionState.None);

            foreach (var item in accessList)
            {
                var obj = item as JObject;
                if (obj == null)
                {
                    continue;
                }

                var access = obj["access"].ToObject<string[]>();
                var paths = obj["paths"].ToObject<string[]>();

                var fileAccess = FileIOPermissionAccess.NoAccess;
                if (access.Contains("read")) fileAccess |= FileIOPermissionAccess.Read;
                if (access.Contains("write")) fileAccess |= FileIOPermissionAccess.Write;
                if (access.Contains("discover")) fileAccess |= FileIOPermissionAccess.PathDiscovery;
                if (access.Contains("append")) fileAccess |= FileIOPermissionAccess.Append;

                result.AddPathList(fileAccess, paths);
            }

            return result;
        }

        public IPermission GetUnrestricted()
        {
            return new FileIOPermission(PermissionState.Unrestricted);
        }
    }
}
