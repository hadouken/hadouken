#include "jsengineextension.hpp"

using namespace JsEngine;
using namespace Poco::Util;

static const duk_number_list_entry my_module_consts[] = {
    { "FLAG_FOO", (double)(1 << 0) },
    { NULL, 0.0 }
};

duk_ret_t dukopen_bittorrent(duk_context* ctx)
{
    int n = duk_get_top(ctx);
    int type = duk_get_type(ctx, 1);
    duk_push_object(ctx);

    duk_put_number_list(ctx, -1, my_module_consts);
    
    return 1;
}

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
        duk_push_c_function(ctx, dukopen_bittorrent, 0);
        duk_call(ctx, 0);

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

    if (!ctx_) {
        logger_.error("Could not create Duktape heap.");
        return;
    }

    if (!config.has("extensions.jsengine.script")) {
        logger_.error("No script file defined for JsEngine.");
        return;
    }

    std::string script = config.getString("extensions.jsengine.script");

    // Add functions
    duk_push_global_object(ctx_);
    duk_push_c_function(ctx_, requireNative, DUK_VARARGS);
    duk_put_prop_string(ctx_, -2, "requireNative");
    duk_pop(ctx_);

    if (duk_peval_file(ctx_, script.c_str()) != 0) {
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
