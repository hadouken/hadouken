#include <Hadouken/Scripting/Modules/LoggerModule.hpp>

#include "../duktape.h"
#include <Poco/Logger.h>

using namespace Hadouken::Scripting::Modules;

const char* loggerField = "\xff" "logger";

duk_ret_t LoggerModule::initialize(duk_context* ctx)
{
    duk_function_list_entry functions[] =
    {
        { "get", LoggerModule::get, 1 },
        { NULL, NULL, 0 }
    };

    duk_put_function_list(ctx, 0 /* exports */, functions);
    return 0;
}

duk_ret_t LoggerModule::get(duk_context* ctx)
{
    std::string loggerName(duk_require_string(ctx, 0));
    Poco::Logger& logger = Poco::Logger::get("hadouken.scripting." + loggerName);

    duk_idx_t logIdx = duk_push_object(ctx);

    duk_function_list_entry functions[] =
    {
        { "trace", LoggerModule::log_trace, DUK_VARARGS },
        { "debug", LoggerModule::log_debug, DUK_VARARGS },
        { "info", LoggerModule::log_info, DUK_VARARGS },
        { "warn", LoggerModule::log_warn, DUK_VARARGS },
        { "error", LoggerModule::log_error, DUK_VARARGS },
        { NULL, NULL, 0 }
    };

    duk_put_function_list(ctx, logIdx, functions);

    duk_push_pointer(ctx, &logger);
    duk_put_prop_string(ctx, logIdx, loggerField);

    return 1;
}

duk_ret_t LoggerModule::log_trace(duk_context* ctx)
{
    Poco::Logger* logger = LoggerModule::getPointerFromThis<Poco::Logger>(ctx, loggerField);
    logger->trace(duk_require_string(ctx, 0));
    return 0;
}

duk_ret_t LoggerModule::log_debug(duk_context* ctx)
{
    Poco::Logger* logger = LoggerModule::getPointerFromThis<Poco::Logger>(ctx, loggerField);
    logger->debug(duk_require_string(ctx, 0));
    return 0;
}

duk_ret_t LoggerModule::log_info(duk_context* ctx)
{
    Poco::Logger* logger = LoggerModule::getPointerFromThis<Poco::Logger>(ctx, loggerField);
    logger->information(duk_require_string(ctx, 0));
    return 0;
}

duk_ret_t LoggerModule::log_warn(duk_context* ctx)
{
    Poco::Logger* logger = LoggerModule::getPointerFromThis<Poco::Logger>(ctx, loggerField);
    logger->warning(duk_require_string(ctx, 0));
    return 0;
}

duk_ret_t LoggerModule::log_error(duk_context* ctx)
{
    Poco::Logger* logger = LoggerModule::getPointerFromThis<Poco::Logger>(ctx, loggerField);
    logger->error(duk_require_string(ctx, 0));
    return 0;
}

template<class T>
T* LoggerModule::getPointerFromThis(duk_context* ctx, const char* fieldName)
{
    duk_push_this(ctx);

    T* res = 0;

    if (duk_get_prop_string(ctx, -1, fieldName))
    {
        res = static_cast<T*>(duk_get_pointer(ctx, -1));
        duk_pop(ctx);
    }

    duk_pop(ctx);
    return res;
}
