#include <hadouken/scripting/modules/logger_module.hpp>

#include <boost/log/trivial.hpp>
#include "../duktape.h"

using namespace hadouken::scripting::modules;

duk_ret_t logger_module::initialize(duk_context* ctx)
{
    duk_function_list_entry functions[] =
    {
        { "get", get, 1 },
        { NULL, NULL, 0 }
    };

    duk_put_function_list(ctx, 0 /* exports */, functions);
    return 0;
}

duk_ret_t logger_module::get(duk_context* ctx)
{
    std::string loggerName(duk_require_string(ctx, 0));

    duk_idx_t logIdx = duk_push_object(ctx);

    duk_function_list_entry functions[] =
    {
        { "trace", log_trace, DUK_VARARGS },
        { "debug", log_debug, DUK_VARARGS },
        { "info",  log_info,  DUK_VARARGS },
        { "warn",  log_warn,  DUK_VARARGS },
        { "error", log_error, DUK_VARARGS },
        { NULL, NULL, 0 }
    };

    duk_put_function_list(ctx, logIdx, functions);

    return 1;
}

duk_ret_t logger_module::log_trace(duk_context* ctx)
{
    BOOST_LOG_TRIVIAL(trace) << duk_require_string(ctx, 0);
    return 0;
}

duk_ret_t logger_module::log_debug(duk_context* ctx)
{
    BOOST_LOG_TRIVIAL(debug) << duk_require_string(ctx, 0);
    return 0;
}

duk_ret_t logger_module::log_info(duk_context* ctx)
{
    BOOST_LOG_TRIVIAL(info) << duk_require_string(ctx, 0);
    return 0;
}

duk_ret_t logger_module::log_warn(duk_context* ctx)
{
    BOOST_LOG_TRIVIAL(warning) << duk_require_string(ctx, 0);
    return 0;
}

duk_ret_t logger_module::log_error(duk_context* ctx)
{
    BOOST_LOG_TRIVIAL(error) << duk_require_string(ctx, 0);
    return 0;
}
