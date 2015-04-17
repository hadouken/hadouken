#include <Hadouken/Http/JsonRpcRequestHandler.hpp>

#include <Hadouken/Scripting/ScriptingSubsystem.hpp>

#include <Poco/JSON/ParseHandler.h>
#include <Poco/JSON/Parser.h>
#include <Poco/Net/SecureStreamSocket.h>
#include <Poco/Net/HTTPBasicCredentials.h>
#include <Poco/Net/HTTPServerResponse.h>
#include <Poco/Net/HTTPServerRequest.h>
#include <Poco/Net/HTTPServerRequestImpl.h>
#include <Poco/Net/X509Certificate.h>
#include <Poco/Util/Application.h>

using namespace Hadouken::Http;
using namespace Hadouken::Scripting;
using namespace Poco::JSON;
using namespace Poco::Net;
using namespace Poco::Util;

JsonRpcRequestHandler::JsonRpcRequestHandler(const Poco::Util::AbstractConfiguration& config)
    : config_(config)
{
}

void JsonRpcRequestHandler::handleRequest(HTTPServerRequest& request, HTTPServerResponse& response)
{
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

    ScriptingSubsystem& scripting = Application::instance().getSubsystem<ScriptingSubsystem>();
    response.send() << scripting.rpc(result.toString());
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
