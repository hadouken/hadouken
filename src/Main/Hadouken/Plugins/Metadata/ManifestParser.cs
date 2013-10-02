using System;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins.Metadata
{
    public static class ManifestParser
    {
        public static bool TryParse(Stream json, out Manifest manifest)
        {
            Exception exception;
            return TryParse(json, out manifest, out exception);
        }

        public static bool TryParse(Stream json, out Manifest manifest, out Exception exception)
        {
            manifest = null;
            exception = null;

            try
            {
                using (var streamReader = new StreamReader(json))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var obj = JToken.ReadFrom(jsonReader) as JObject;

                    if (obj == null)
                        return false;

                    JToken manifestVersionToken;

                    if (!obj.TryGetValue("manifest_version", out manifestVersionToken))
                        return false;

                    if (manifestVersionToken.Type != JTokenType.Integer)
                        return false;

                    var manifestVersion = manifestVersionToken.Value<int>();

                    // Get a manifest parser for this version of the manifest
                }
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }

            return false;
        }
    }
}
