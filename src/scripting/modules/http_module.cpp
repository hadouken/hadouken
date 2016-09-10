#include <hadouken/scripting/modules/http_module.hpp>

#include <boost/network/include/http/client.hpp>
#include <sstream>
#include <string>
#include "../duktape.h"

using namespace boost::network::http;
using namespace hadouken::scripting::modules;

typedef boost::network::http::basic_client<
    boost::network::http::tags::http_default_8bit_udp_resolve
    , 1
    , 1
>
client_t;

duk_ret_t http_module::initialize(duk_context* ctx)
{
    static duk_function_list_entry functions[] =
    {
        { "post", post, 3 },
        { NULL,   NULL, 0 }
    };

    duk_put_function_list(ctx, 0, functions);

    return 0;
}

duk_ret_t http_module::post(duk_context* ctx)
{
    client_t::request request_(duk_require_string(ctx, 0));
    std::string content(duk_require_string(ctx, 1));

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
            request_ << boost::network::header(key, val);

            duk_pop(ctx);
        }

        duk_pop(ctx);
    }

    client_t client_;
    client_t::response response_ = client_.post(request_, content);

    duk_idx_t respIdx = duk_push_object(ctx);

    duk_push_int(ctx, response_.status());
    duk_put_prop_string(ctx, respIdx, "status");

    duk_push_string(ctx, response_.status_message().c_str());
    duk_put_prop_string(ctx, respIdx, "reason");

    duk_push_string(ctx, response_.body().c_str());
    duk_put_prop_string(ctx, respIdx, "body");

    return 1;
}
