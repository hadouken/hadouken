using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hadouken.Framework.Rpc
{
    public class ErrorResponse : Response
    {
        public ErrorResponse(RpcErrorObject errorObject)
        {
            Error = errorObject;
        }

        [JsonProperty("error", Required = Required.Always)]
        public RpcErrorObject Error { get; set; }
    }

    public abstract class RpcErrorObject
    {
        [JsonProperty("code", Required = Required.Always)]
        public int ErrorCode { get; set; }

        [JsonProperty("message", Required = Required.AllowNull)]
        public string Message { get; set; }
    }

    public class MethodDoesNotExistError : RpcErrorObject
    {
        public MethodDoesNotExistError()
        {
            ErrorCode = -32601;
            Message = "The method does not exist / is not available.";
        }
    }

    public class ParseError : RpcErrorObject
    {
        public ParseError()
        {
            ErrorCode = -32700;
            Message =
                "Invalid JSON was received by the server. An error occurred on the server while parsing the JSON text.";
        }
    }
}
