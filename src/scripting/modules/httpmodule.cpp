#include <Hadouken/Scripting/Modules/HttpModule.hpp>

#include <Poco/Net/HTTPClientSession.h>
#include <Poco/Net/HTTPSClientSession.h>
#include <Poco/Net/HTTPRequest.h>
#include <Poco/Net/HTTPResponse.h>
#include <Poco/URI.h>

#include <sstream>
#include <string>
#include "../duktape.h"

using namespace Hadouken::Scripting::Modules;

duk_ret_t HttpModule::initialize(duk_context* ctx)
{
    duk_function_list_entry functions[] =
    {
        { "post", post, 3 },
        { NULL,   NULL, 0 }
    };

    duk_put_function_list(ctx, 0, functions);

    return 0;
}

duk_ret_t HttpModule::post(duk_context* ctx)
{
    using namespace Poco::Net;

    Poco::URI url(duk_require_string(ctx, 0));
    std::string body(duk_require_string(ctx, 1));

    HTTPClientSession* client;

    if (url.getScheme() == "https")
    {
        client = new HTTPSClientSession(url.getHost(), url.getPort());
    }
    else
    {
        client = new HTTPClientSession(url.getHost(), url.getPort());
    }

    HTTPRequest request(HTTPRequest::HTTP_POST, url.getPathAndQuery());

    if (duk_has_prop_string(ctx, 2, "headers"))
    {
        duk_get_prop_string(ctx, 2, "headers");
        duk_enum(ctx, -1, DUK_ENUM_OWN_PROPERTIES_ONLY);

        std::vector<std::string> keys;

        while (duk_next(ctx, -1, 0))
        {
            keys.push_back(duk_get_string(ctx, -1));
            duk_pop(ctx);
        }

        duk_pop(ctx);

        for (std::string key : keys)
        {
            duk_get_prop_string(ctx, -1, key.c_str());

            std::string val(duk_get_string(ctx, -1));
            request.add(key, val);

            duk_pop(ctx);
        }

        duk_pop(ctx);
    }

    request.setContentLength(body.size());
    client->sendRequest(request) << body;

    HTTPResponse response;
    std::istream& responseStream = client->receiveResponse(response);

    duk_idx_t respIdx = duk_push_object(ctx);

    duk_push_string(ctx, response.getReason().c_str());
    duk_put_prop_string(ctx, respIdx, "reason");
    
    duk_push_int(ctx, response.getStatus());
    duk_put_prop_string(ctx, respIdx, "status");

    delete client;

    return 1;
}
