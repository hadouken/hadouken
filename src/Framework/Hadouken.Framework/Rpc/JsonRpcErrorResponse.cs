using Newtonsoft.Json;
using System;

namespace Hadouken.Framework.Rpc
{
    public class JsonRpcErrorResponse : JsonRpcResponse
    {
        [JsonProperty("error", Required = Required.Always)]
        public JsonRpcErrorContainer Error { get; set; }

        public static JsonRpcErrorResponse ParseError(object requestId)
        {
            return new JsonRpcErrorResponse() {Id = requestId, Error = new JsonRpcErrorContainer(-32700, "Invalid JSON was received by the server. An error occurred on the server while parsing the JSON text.", null) };
        }

        public static JsonRpcErrorResponse InvalidRequest(object requestId)
        {
            return new JsonRpcErrorResponse() {Id = requestId, Error = new JsonRpcErrorContainer(-32600, "The JSON sent is not a valid Request object.", null) };
        }

        public static JsonRpcErrorResponse MethodNotFound(object requestId, string methodName)
        {
            return new JsonRpcErrorResponse() {Id = requestId, Error = new JsonRpcErrorContainer(-32601, "The method does not exist / is not available.", methodName) };
        }

        public static JsonRpcErrorResponse InvalidParams(object requestId)
        {
            return new JsonRpcErrorResponse() {Id = requestId, Error = new JsonRpcErrorContainer(-32602, "Invalid method parameter(s).", null) };
        }

        public static JsonRpcErrorResponse InternalRpcError(object requestId, Exception exception = null)
        {
            return new JsonRpcErrorResponse() {Id = requestId, Error = new JsonRpcErrorContainer(-32603, "Internal JSON-RPC error.", exception != null ? exception.ToString() : null) };
        }
    }

    public sealed class JsonRpcErrorContainer
    {
        public JsonRpcErrorContainer() {}

        public JsonRpcErrorContainer(int errorCode, string message, object data)
        {
            ErrorCode = errorCode;
            Message = message;
            Data = data;
        }

        [JsonProperty("code")]
        public int ErrorCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }
    }
}
