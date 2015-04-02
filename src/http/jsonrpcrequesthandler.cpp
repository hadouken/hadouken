#include <Hadouken/Http/JsonRpcRequestHandler.hpp>

#include <Poco/JSON/ParseHandler.h>
#include <Poco/JSON/Parser.h>
#include <Poco/Net/SecureStreamSocket.h>
#include <Poco/Net/HTTPBasicCredentials.h>
#include <Poco/Net/HTTPServerResponse.h>
#include <Poco/Net/HTTPServerRequest.h>
#include <Poco/Net/HTTPServerRequestImpl.h>
#include <Poco/Net/X509Certificate.h>

#include <openssl/x509.h>

using namespace Hadouken::Http;
using namespace Hadouken::Http::JsonRpc;
using namespace Poco::JSON;
using namespace Poco::Net;

Poco::DynamicStruct::Ptr createErrorResponse(int code, std::string message, std::string data)
{
    Poco::DynamicStruct::Ptr error = new Poco::DynamicStruct();
    error->insert("code", code);
    error->insert("message", message);
    error->insert("data", data);

    return error;
}

JsonRpcRequestHandler::JsonRpcRequestHandler(const Poco::Util::AbstractConfiguration& config, std::map<std::string, std::shared_ptr<RpcMethod>>& methods)
    : config_(config),
      methods_(methods)
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

    response.add("Access-Control-Allow-Origin", "*");
    response.add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, Authorization");
    response.setContentType("application/json");
    
    if (request.getMethod() == "OPTIONS") {
        response.send() << "{ \"status\": \"OK\" }";
        return;
    }

    if (!isValidRequest(request))
    {
        response.setStatusAndReason(HTTPServerResponse::HTTP_UNAUTHORIZED, "Unauthorized");

        Poco::DynamicStruct::Ptr unauthObj = new Poco::DynamicStruct();
        unauthObj->insert("error", "Unauthorized request.");

        response.send() << unauthObj->toString();
        return;
    }

    Parser parser;
    Poco::Dynamic::Var result = parser.parse(request.stream());

    Object::Ptr requestObject = result.extract<Object::Ptr>();
    Poco::DynamicStruct::Ptr responseObject = new Poco::DynamicStruct();

    // Validate "jsonrpc"
    if (!requestObject->has("jsonrpc"))
    {
        responseObject->insert("error", *createErrorResponse(-32600,
           "Invalid Request",
           "Missing \"jsonrpc\" field in request object."));

        response.send() << responseObject->toString();
        return;
    }

    if (!requestObject->has("id"))
    {
        // Invalid JSONRPC request. Return error.
        responseObject->insert("error", *createErrorResponse(-32600,
            "Invalid Request",
            "Missing \"id\" field in request object."));

        response.send() << responseObject->toString();
        return;
    }

    if (!requestObject->isArray("params"))
    {
        // We currently do not support named parameters.
        responseObject->insert("error", *createErrorResponse(-32600,
            "Invalid Request",
            "The \"params\" field must be an array."));

        response.send() << responseObject->toString();
        return;
    }

    // Set common properties on response object
    responseObject->insert("jsonrpc", "2.0");
    responseObject->insert("id", requestObject->get("id"));

    std::string method = requestObject->getValue<std::string>("method");

    if (methods_.count(method) > 0)
    {
        Array::Ptr params = requestObject->getArray("params");
        std::shared_ptr<RpcMethod> rpcMethod = methods_.at(method);

        // Execute and write result.
        Poco::Dynamic::Var::Ptr result = rpcMethod->execute(params);

        if (result.isNull())
        {
            responseObject->insert("result", Poco::Dynamic::Var());
        }
        else
        {
            responseObject->insert("result", *result);
        }

        response.send() << responseObject->toString();
    }
    else
    {
        response.send() << "Not OK!";
    }
}

bool JsonRpcRequestHandler::isValidRequest(HTTPServerRequest& request) const
{
    // Check if auth type is set to None.
    if (!config_.has("http.auth.type")
        || Poco::icompare(config_.getString("http.auth.type"), "none") == 0)
    {
        return true;
    }

    // If we get here, auth is enabled. Check credentials.
    if (!request.hasCredentials())
    {
        return false;
    }

    std::string scheme;
    std::string authInfo;
    request.getCredentials(scheme, authInfo);

    std::string authType = config_.getString("http.auth.type");

    if (Poco::icompare(authType, "Basic") == 0)
    {
        std::string userName = config_.getString("http.auth.basic.userName");
        std::string password = config_.getString("http.auth.basic.password");

        Poco::Net::HTTPBasicCredentials credentials(request);

        if (Poco::icompare(userName, credentials.getUsername()) == 0
            && password.compare(credentials.getPassword()) == 0)
        {
            return true;
        }
    }
    else if (Poco::icompare(authType, "Token") == 0)
    {
        std::string token = config_.getString("http.auth.token");

        if (token.compare(authInfo) == 0)
        {
            return true;
        }
    }
    else if (Poco::icompare(authType, "Certificate") == 0)
    {
        // For this to be a valid authType, we need to run SSL.
        if (!config_.hasProperty("http.ssl.enabled")
            && !config_.getBool("http.ssl.enabled"))
        {
            // Log error
            return false;
        }

        SecureStreamSocket secureSocket = static_cast<HTTPServerRequestImpl&>(request).socket();

        if (!secureSocket.havePeerCertificate())
        {
            return false;
        }

        // TODO: Validate peer certificate
    }

    return false;
}
