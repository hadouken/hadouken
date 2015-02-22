#include <Hadouken/Http/JsonRpcRequestHandler.hpp>

#include <Poco/JSON/ParseHandler.h>
#include <Poco/JSON/Parser.h>
#include <Poco/Net/HTTPServerResponse.h>
#include <Poco/Net/HTTPServerRequest.h>

using namespace Hadouken::Http;
using namespace Hadouken::Http::JsonRpc;
using namespace Poco::JSON;
using namespace Poco::Net;

JsonRpcRequestHandler::JsonRpcRequestHandler(std::map<std::string, Hadouken::Http::JsonRpc::RpcMethod*>& methods)
    : methods_(methods)
{
}

void JsonRpcRequestHandler::handleRequest(HTTPServerRequest& request, HTTPServerResponse& response)
{
    /*
    Parse a JSONRPC request. A request looks like this,

    {
      "jsonrpc": "2.0",
      "id": 123,
      "method": "some.method",
      "params": []
    }

    Currently, only positional parameters are supported. The specification also
    mentions named parameters however this is not implemented yet.

    The response should be

    {
      "jsonrpc": "2.0",
      "id": 123,
      "result": <result>
    }
    */

    // Set common parameters, like content type.
    response.setContentType("application/json");

    Parser parser;
    Poco::Dynamic::Var result = parser.parse(request.stream());

    Object::Ptr requestObject = result.extract<Object::Ptr>();
    Poco::DynamicStruct::Ptr responseObject = new Poco::DynamicStruct();

    // Validate "jsonrpc"
    if (!requestObject->has("jsonrpc"))
    {
        // Invalid JSONRPC request. Return error.
        return;
    }

    if (!requestObject->has("id"))
    {
        // Invalid JSONRPC request. Return error.
        return;
    }

    if (!requestObject->isArray("params"))
    {
        // We currently do not support named parameters.
        return;
    }

    // Set common properties on response object
    responseObject->insert("jsonrpc", "2.0");
    responseObject->insert("id", requestObject->get("id"));

    std::string method = requestObject->getValue<std::string>("method");

    if (methods_.count(method) > 0)
    {
        Array::Ptr params = requestObject->getArray("params");
        RpcMethod* rpcMethod = methods_.at(method);

        // Execute and write result.
        Poco::Dynamic::Var::Ptr result = rpcMethod->execute(params);
        responseObject->insert("result", *result);

        response.send() << responseObject->toString();
    }
    else
    {
        response.send() << "Not OK!";
    }
}