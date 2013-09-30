using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hadouken.Framework.Rpc
{
    public sealed class JsonRpcResponse
    {
        public JsonRpcResponse()
        {
            Protocol = "2.0";
        }

        [JsonProperty("id", Required = Required.Always)]
        public object Id { get; set; }

        [JsonProperty("jsonrpc", Required = Required.Always)]
        public string Protocol { get; set; }

        [JsonProperty("result", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public object Result { get; set; }

        [JsonProperty("error", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public RpcError Error { get; set; }
    }

    public abstract class RpcError
    {
        protected RpcError(int errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
        }

        [JsonProperty("code")]
        public int ErrorCode { get; private set; }

        [JsonProperty("message")]
        public string Message { get; private set; }

        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public string Data { get; set; }
    }

    public sealed class RpcParseError : RpcError
    {
        public RpcParseError()
            : base(-32700, "Invalid JSON was received by the server. An error occurred on the server while parsing the JSON text.") {}
    }

    public sealed class InvalidRequestError : RpcError
    {
        public InvalidRequestError()
            : base(-32600, "The JSON sent is not a valid Request object.") {}
    }

    public sealed class MethodNotFoundError : RpcError
    {
        public MethodNotFoundError()
            : base(-32601, "The method does not exist / is not available.") {}
    }

    public sealed class InvalidParamsError : RpcError
    {
        public InvalidParamsError()
            : base(-32602, "Invalid method parameter(s).") {}
    }

    public sealed class InternalRpcError : RpcError
    {
        public InternalRpcError()
            : base(-32603, "Internal JSON-RPC error.") {}
    }
}
