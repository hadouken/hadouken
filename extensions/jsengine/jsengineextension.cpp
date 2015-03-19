#include "jsengineextension.hpp"
#include "mod_fs.hpp"

using namespace JsEngine;
using namespace Poco::Util;

duk_ret_t requireNative(duk_context* ctx)
{
    int argCount = duk_get_top(ctx);

    // arg0: id
    // arg1: require
    // arg2: exports
    // arg3: module

    const char* mod = duk_get_string(ctx, 0);
    std::string moduleName(mod);

    if (moduleName == "bittorrent")
    {
        return 0;
    }
    else if (moduleName == "fs")
    {
        duk_push_c_function(ctx, dukopen_fs, 1);
        duk_dup(ctx, 2);
        duk_call(ctx, 1);

        return 1;
    }

    return 0;
}

JsEngineExtension::JsEngineExtension()
    : logger_(Poco::Logger::get("jsengine.jsengineextension"))
{
}

void JsEngineExtension::load(AbstractConfiguration& config)
{
    ctx_ = duk_create_heap_default();

    if (!ctx_)
    {
        logger_.error("Could not create Duktape heap.");
        return;
    }

    if (!config.has("extensions.jsengine.path"))
    {
        logger_.error("No path defined for JsEngine.");
        return;
    }

    if (!config.has("extensions.jsengine.script"))
    {
        logger_.error("No script file defined for JsEngine.");
        return;
    }

    std::string path = config.getString("extensions.jsengine.path");
    std::string script = config.getString("extensions.jsengine.script");

    std::string scriptFile = path + "/" + script;

    // Add functions
    duk_push_global_object(ctx_);
    duk_push_c_function(ctx_, requireNative, DUK_VARARGS);
    duk_put_prop_string(ctx_, -2, "requireNative");

    if (duk_peval_file(ctx_, scriptFile.c_str()) != 0)
    {
        std::string error(duk_safe_to_string(ctx_, -1));
        logger_.error("Could not evaluate script file: %s", error);
        return;
    }

    // Ignore result
    duk_pop(ctx_);
}

void JsEngineExtension::unload()
{
    duk_destroy_heap(ctx_);
}
