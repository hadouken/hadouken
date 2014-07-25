using System;
using System.Collections.Generic;
using Hadouken.Common.Text;

namespace Hadouken.Core.JsonRpc
{
    public class JsonRpcRequestParser : IJsonRpcRequestParser
    {
        private readonly IJsonSerializer _serializer;

        public JsonRpcRequestParser(IJsonSerializer serializer)
        {
            if (serializer == null) throw new ArgumentNullException("serializer");
            _serializer = serializer;
        }

        public JsonRpcRequest Parse(string json)
        {
            var request = _serializer.DeserializeObject<IDictionary<string, object>>(json);

            // Validate protocol version
            if (!request.ContainsKey("jsonrpc")
                || request["jsonrpc"].GetType() != typeof (string)
                || request["jsonrpc"].ToString() != "2.0")
            {
                throw new InvalidRequestException("Invalid protocol version.");
            }

            // Validate method
            if (!request.ContainsKey("method")
                || request["method"].GetType() != typeof (string))
            {
                throw new InvalidRequestException("Invalid method name.");
            }

            // Validate id
            if (!(request.ContainsKey("id")
                  && request["id"] != null)
                || !(request["id"] is int
                     || request["id"] is long
                     || request["id"] is string))
            {
                throw new InvalidRequestException("Invalid id.");
            }

            var rpcRequest =  new JsonRpcRequest
            {
                Id = request["id"],
                MethodName = request["method"].ToString(),
                ProtocolVersion = request["jsonrpc"].ToString()
            };

            if (request.ContainsKey("params"))
            {
                rpcRequest.Parameters = request["params"];
            }

            return rpcRequest;
        }
    }
}