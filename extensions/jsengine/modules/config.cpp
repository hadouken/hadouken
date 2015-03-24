#include "config.hpp"

#include <Hadouken/Platform.hpp>
#include <Poco/File.h>
#include <Poco/Path.h>
#include <Poco/Util/Application.h>
#include <Poco/Util/JSONConfiguration.h>

using namespace Poco;
using namespace Poco::Util;

duk_ret_t config_get(duk_context* ctx)
{
    std::string key(duk_require_string(ctx, 0));

    if (key.empty())
    {
        duk_error(ctx, DUK_ERR_INTERNAL_ERROR, "Key must be specified.");
        return 0;
    }

    Application& app = Application::instance();

    if (!app.config().has(key))
    {
        duk_push_undefined(ctx);
    }
    else
    {
        std::string value = app.config().getRawString(key);
        duk_eval_string(ctx, value.c_str());
    }

    return 1;
}

static const duk_function_list_entry config_funcs[] = {
    { "get", config_get, 1 },
    { NULL }
};

namespace JsEngine
{
    duk_ret_t dukopen_config(duk_context* ctx)
    {
        duk_put_function_list(ctx, 0 /* export */, config_funcs);
        return 0;
    }
}
