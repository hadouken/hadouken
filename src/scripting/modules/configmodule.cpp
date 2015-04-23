#include <Hadouken/Scripting/Modules/ConfigModule.hpp>

#include <Poco/Util/Application.h>
#include <string>

#include "common.hpp"
#include "../duktape.h"

using namespace Hadouken::Scripting::Modules;
using namespace Poco::Util;

duk_ret_t ConfigModule::initialize(duk_context* ctx)
{
    duk_function_list_entry functions[] =
    {
        { "getBoolean",  getBoolean,  1 },
        { "getNumber",   getNumber,   1 },
        { "getString",   getString,   1 },
        { "has",         has,         1 },
        //{ "setString", setString, 2 },
        { NULL, NULL, 0 }
    };

    Application& app = Application::instance();
    Common::setPointer<AbstractConfiguration>(ctx, 0, &app.config());

    duk_put_function_list(ctx, 0 /* exports */, functions);
    return 0;
}

duk_ret_t ConfigModule::getBoolean(duk_context* ctx)
{
    std::string key(duk_require_string(ctx, 0));
    AbstractConfiguration* config = Common::getPointer<AbstractConfiguration>(ctx);

    duk_push_boolean(ctx, config->getBool(key));

    return 1;
}

duk_ret_t ConfigModule::getNumber(duk_context* ctx)
{
    std::string key(duk_require_string(ctx, 0));
    AbstractConfiguration* config = Common::getPointer<AbstractConfiguration>(ctx);

    duk_push_number(ctx, static_cast<duk_double_t>(config->getInt64(key)));

    return 1;
}

duk_ret_t ConfigModule::getString(duk_context* ctx)
{
    std::string key(duk_require_string(ctx, 0));
    AbstractConfiguration* config = Common::getPointer<AbstractConfiguration>(ctx);

    duk_push_string(ctx, config->getString(key).c_str());

    return 1;
}

duk_ret_t ConfigModule::has(duk_context* ctx)
{
    std::string key(duk_require_string(ctx, 0));
    AbstractConfiguration* config = Common::getPointer<AbstractConfiguration>(ctx);

    duk_push_boolean(ctx, config->has(key));

    return 1;
}

duk_ret_t ConfigModule::setString(duk_context* ctx)
{
    std::string key(duk_require_string(ctx, 0));
    std::string value(duk_require_string(ctx, 1));

    AbstractConfiguration* config = Common::getPointer<AbstractConfiguration>(ctx);
    config->setString(key, value);

    return 0;
}
