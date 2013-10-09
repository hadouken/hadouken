using System;
using Hadouken.Framework.SemVer;
using Newtonsoft.Json;

namespace Hadouken.Framework.Rpc
{
    public class SemanticVersionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (SemanticVersion);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType != JsonToken.String)
                throw new Exception(
                    String.Format("Unexpected token or value when parsing version. Token: {0}, Value: {1}",
                                  reader.TokenType, reader.Value));
            try
            {
                var semver = new SemanticVersion((string) reader.Value);
                return semver;
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Error parsing SemanticVersion string {0}", reader.Value), e);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else if (value is SemanticVersion)
            {
                writer.WriteValue(value.ToString());
            }
            else
            {
                throw new Exception("Expected SemanticVersion object value.");
            }
        }
    }
}