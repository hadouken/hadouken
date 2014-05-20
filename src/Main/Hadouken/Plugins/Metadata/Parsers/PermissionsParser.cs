using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using Hadouken.Plugins.Metadata.Parsers.Permissions;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata.Parsers
{
    public sealed class PermissionsParser
    {
        private const string Unrestricted = "<unrestricted>";

        private static readonly IDictionary<string, Func<IPermissionItemParser>> PermissionParsers =
            new Dictionary<string, Func<IPermissionItemParser>>();

        static PermissionsParser()
        {
            PermissionParsers.Add("dns", () => new DnsPermissionItemParser());
            PermissionParsers.Add("fileio", () => new FileIoPermissionItemParser());
            PermissionParsers.Add("reflection", () => new ReflectionPermissionItemParser());
            PermissionParsers.Add("sockets", () => new SocketsPermissionItemParser());
            PermissionParsers.Add("web", () => new WebPermissionItemParser());
        }

        public PermissionSet Parse(JToken value)
        {
            if (value == null)
            {
                return new PermissionSet(PermissionState.None);
            }

            if (value.Type == JTokenType.String && value.Value<string>() == Unrestricted)
            {
                return new PermissionSet(PermissionState.Unrestricted);
            }

            var permissionsObject = value as JObject;

            if (permissionsObject == null)
            {
                return new PermissionSet(PermissionState.None);
            }

            var result = new PermissionSet(PermissionState.None);

            foreach (var item in permissionsObject)
            {
                Func<IPermissionItemParser> parserFactory;

                if (!PermissionParsers.TryGetValue(item.Key, out parserFactory))
                {
                    continue;
                }

                var parser = parserFactory();

                if (item.Value.Type == JTokenType.String && item.Value.Value<string>() == Unrestricted)
                {
                    result.AddPermission(parser.GetUnrestricted());
                }
                else
                {
                    result.AddPermission(parser.Parse(item.Value));
                }
            }

            return result;
        }
    }
}
