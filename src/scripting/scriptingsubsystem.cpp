#include <Hadouken/Scripting/ScriptingSubsystem.hpp>

#include <Hadouken/Scripting/Modules/BitTorrentModule.hpp>
#include <Hadouken/Scripting/Modules/CoreModule.hpp>
#include <Hadouken/Scripting/Modules/FileSystemModule.hpp>
#include <Hadouken/Scripting/Modules/LoggerModule.hpp>

#include "duktape.h"

using namespace Hadouken::Scripting;
using namespace Poco::Util;

ScriptingSubsystem::ScriptingSubsystem()
    : logger_(Poco::Logger::get("hadouken.scripting"))
{
}

void ScriptingSubsystem::initialize(Application& app)
{
    AbstractConfiguration& config = app.config();

    std::string path = config.getString("scripting.path", "");
    std::string script = config.getString("scripting.script", "");

    if (path.empty() || script.empty())
    {
        logger_.fatal("Scripting subsystem missing path to JS files and a boot script. Please set 'scripting.path' and 'scripting.script'.");
        return;
    }

    ctx_ = duk_create_heap_default();

    // Set up requireNative
    duk_push_c_function(ctx_, &ScriptingSubsystem::requireNative, DUK_VARARGS);
    duk_put_global_string(ctx_, "requireNative");

    duk_idx_t idx = duk_push_object(ctx_);

    std::string scriptFile = path + "/" + script;

    if (duk_peval_file(ctx_, scriptFile.c_str()) != 0)
    {
        std::string error(duk_safe_to_string(ctx_, -1));
        logger_.error("Could not evaluate script file: %s", error);
    }

    // Top of the stack should contain a callable function
    if (!duk_is_callable(ctx_, -1))
    {
        logger_.error("Script did not return a callable function.");
    }
    else
    {
        duk_dup(ctx_, -1);
        duk_dup(ctx_, idx);

        if (duk_pcall_method(ctx_, 1) != DUK_EXEC_SUCCESS)
        {
            std::string error(duk_safe_to_string(ctx_, -1));
            logger_.error("Error when executing script: %s", error);
        }
        
        duk_pop(ctx_);
    }

    // Put object in global stash
    duk_push_global_stash(ctx_);
    duk_dup(ctx_, idx);
    duk_put_prop_string(ctx_, -2, "hdkn");
    duk_pop(ctx_);

    // Ignore result
    duk_pop(ctx_);
}

void ScriptingSubsystem::uninitialize()
{
    duk_destroy_heap(ctx_);
}

std::string ScriptingSubsystem::rpc(std::string request)
{
    std::lock_guard<std::mutex> lock(contextMutex_);

    if (!ctx_)
    {
        logger_.fatal("Cannot handle RPC request. Scripting subsystem not configured.");
        return std::string();
    }

    std::string result;
    duk_push_global_stash(ctx_);
    
    if (duk_get_prop_string(ctx_, -1, "hdkn"))
    {
        if (duk_get_prop_string(ctx_, -1, "rpc"))
        {
            if (!duk_is_callable(ctx_, -1))
            {
                logger_.error("Type 'rpc' is not callable.");
                return std::string();
            }

            duk_dup(ctx_, -1);
            
            duk_push_string(ctx_, request.c_str());
            duk_json_decode(ctx_, -1);

            if (duk_pcall_method(ctx_, 1) != DUK_EXEC_SUCCESS)
            {
                std::string error(duk_safe_to_string(ctx_, -1));
                logger_.error("Error when handling RPC method: %s", error);
            }

            if (duk_get_type(ctx_, -1) == DUK_TYPE_OBJECT)
            {
                result = std::string(duk_json_encode(ctx_, -1));
            }

            duk_pop(ctx_);
        }

        duk_pop(ctx_);
    }

    duk_pop(ctx_);

    return result;
}

const char* ScriptingSubsystem::name() const
{
    return "Scripting";
}

duk_ret_t ScriptingSubsystem::requireNative(duk_context* ctx)
{
    const char* moduleName = duk_require_string(ctx, 0);

    if (strcmp("fs", moduleName) == 0)
    {
        duk_push_c_function(ctx, &Modules::FileSystemModule::initialize, 1);
        duk_dup(ctx, 2);
        duk_call(ctx, 1);

        return 1;
    }
    else if (strcmp("bittorrent", moduleName) == 0)
    {
        duk_push_c_function(ctx, &Modules::BitTorrentModule::initialize, 1);
        duk_dup(ctx, 2);
        duk_call(ctx, 1);

        return 1;
    }
    else if (strcmp("core", moduleName) == 0)
    {
        duk_push_c_function(ctx, &Modules::CoreModule::initialize, 1);
        duk_dup(ctx, 2);
        duk_call(ctx, 1);

        return 1;
    }
    else if (strcmp("logger", moduleName) == 0)
    {
        duk_push_c_function(ctx, &Modules::LoggerModule::initialize, 1);
        duk_dup(ctx, 2);
        duk_call(ctx, 1);

        return 1;
    }

    return 0;
}
