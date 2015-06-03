#include <Hadouken/Scripting/ScriptingSubsystem.hpp>

#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/AlertWrapper.hpp>
#include <Hadouken/Scripting/Modules/BEncodingModule.hpp>
#include <Hadouken/Scripting/Modules/BitTorrentModule.hpp>
#include <Hadouken/Scripting/Modules/ConfigModule.hpp>
#include <Hadouken/Scripting/Modules/CoreModule.hpp>
#include <Hadouken/Scripting/Modules/FileSystemModule.hpp>
#include <Hadouken/Scripting/Modules/HttpModule.hpp>
#include <Hadouken/Scripting/Modules/LoggerModule.hpp>
#include <Hadouken/Scripting/Modules/ProcessModule.hpp>
#include <libtorrent/session.hpp>
#include <Poco/File.h>

#include <chrono>

#include "duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting;
using namespace Poco::Util;

ScriptingSubsystem::ScriptingSubsystem()
    : logger_(Poco::Logger::get("hadouken.scripting")),
    ctx_(NULL)
{
}

void ScriptingSubsystem::initialize(Application& app)
{
    std::lock_guard<std::mutex> lock(contextMutex_);

    AbstractConfiguration& config = app.config();

    std::string path = getScriptPath();
    std::string script = getScript();
    Poco::Path scriptPath(path ,script);

    if (!Poco::File(scriptPath).exists())
    {
        logger_.fatal("Scripting subsystem cannot find script '%s'.", scriptPath.toString());
        return;
    }

    ctx_ = duk_create_heap_default();

    // Set up requireNative
    duk_push_c_function(ctx_, &ScriptingSubsystem::requireNative, DUK_VARARGS);
    duk_put_global_string(ctx_, "requireNative");

    duk_idx_t idx = duk_push_object(ctx_);

    if (duk_peval_file(ctx_, scriptPath.toString().c_str()) != 0)
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

    // Call load
    duk_push_global_stash(ctx_);

    if (duk_get_prop_string(ctx_, -1, "hdkn"))
    {
        if (duk_get_prop_string(ctx_, -1, "load"))
        {
            if (!duk_is_callable(ctx_, -1))
            {
                logger_.error("Type 'load' is not callable.");
                return;
            }

            duk_dup(ctx_, -1);

            if (duk_pcall_method(ctx_, 0) != DUK_EXEC_SUCCESS)
            {
                std::string error(duk_safe_to_string(ctx_, -1));
                logger_.error("Error when calling 'load': %s", error);
            }

            duk_pop(ctx_);
        }

        duk_pop(ctx_);
    }

    duk_pop(ctx_);
    
    isRunning_ = true;
    ticker_ = std::thread(std::bind(&ScriptingSubsystem::tick, this));
    reader_ = std::thread(std::bind(&ScriptingSubsystem::read, this));
}

void ScriptingSubsystem::uninitialize()
{
    isRunning_ = false;

    reader_.join();
    ticker_.join();

    std::lock_guard<std::mutex> lock(contextMutex_);

    // Call unload
    duk_push_global_stash(ctx_);

    if (duk_get_prop_string(ctx_, -1, "hdkn"))
    {
        if (duk_get_prop_string(ctx_, -1, "unload"))
        {
            if (!duk_is_callable(ctx_, -1))
            {
                logger_.error("Type 'unload' is not callable.");
                return;
            }

            duk_dup(ctx_, -1);

            if (duk_pcall_method(ctx_, 0) != DUK_EXEC_SUCCESS)
            {
                std::string error(duk_safe_to_string(ctx_, -1));
                logger_.error("Error when calling 'unload': %s", error);
            }

            duk_pop(ctx_);
        }

        duk_pop(ctx_);
    }

    duk_pop(ctx_);

    if (ctx_)
    {
        duk_destroy_heap(ctx_);
        ctx_ = NULL;
    }
}

void ScriptingSubsystem::emit(std::string name, libtorrent::alert* alert)
{
    std::lock_guard<std::mutex> lock(contextMutex_);

    if (!ctx_)
    {
        return;
    }

    duk_push_global_stash(ctx_);

    if (duk_get_prop_string(ctx_, -1, "hdkn"))
    {
        if (duk_get_prop_string(ctx_, -1, "emit"))
        {
            if (!duk_is_callable(ctx_, -1))
            {
                logger_.error("Type 'emit' is not callable.");
                return;
            }

            duk_dup(ctx_, -1);
            duk_push_string(ctx_, name.c_str());

            if (alert)
            {
                Hadouken::Scripting::Modules::BitTorrent::AlertWrapper::construct(ctx_, alert);
            }
            else
            {
                duk_push_undefined(ctx_);
            }

            if (duk_pcall_method(ctx_, 2) != DUK_EXEC_SUCCESS)
            {
                std::string error(duk_safe_to_string(ctx_, -1));
                logger_.error("Error when emitting alert '%s': %s", name, error);
            }

            duk_pop(ctx_);
        }

        duk_pop(ctx_);
    }

    duk_pop(ctx_);
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

std::string ScriptingSubsystem::getScriptPath()
{
    Application& app = Application::instance();
    Poco::Path scriptPath = app.config().getString("scripting.path", "./js");

    if (scriptPath.isRelative())
    {
        scriptPath.makeAbsolute();
    }

    return scriptPath.toString();
}

std::string ScriptingSubsystem::getScript()
{
    Application& app = Application::instance();
    return app.config().getString("scripting.script", "hadouken.js");
}

void ScriptingSubsystem::tick()
{
    while (isRunning_)
    {
        emit("tick", nullptr);
        std::this_thread::sleep_for(std::chrono::seconds(1));
    }
}

void ScriptingSubsystem::read()
{
    libtorrent::session& sess = Application::instance().getSubsystem<TorrentSubsystem>().getSession();

    while (isRunning_)
    {
        const libtorrent::alert* found_alert = sess.wait_for_alert(libtorrent::seconds(1));
        if (!found_alert) continue;

        std::deque<libtorrent::alert*> alerts;
        sess.pop_alerts(&alerts);

        for (auto &alert : alerts)
        {
            emit(alert->what(), alert);
        }
    }
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
    else if (strcmp("config", moduleName) == 0)
    {
        duk_push_c_function(ctx, &Modules::ConfigModule::initialize, 1);
        duk_dup(ctx, 2);
        duk_call(ctx, 1);

        return 1;
    }
    else if (strcmp("http", moduleName) == 0)
    {
        duk_push_c_function(ctx, &Modules::HttpModule::initialize, 1);
        duk_dup(ctx, 2);
        duk_call(ctx, 1);

        return 1;
    }
    else if (strcmp("process", moduleName) == 0)
    {
        duk_push_c_function(ctx, &Modules::ProcessModule::initialize, 1);
        duk_dup(ctx, 2);
        duk_call(ctx, 1);

        return 1;
    }
    else if (strcmp("benc", moduleName) == 0)
    {
        duk_push_c_function(ctx, &Modules::BEncodingModule::initialize, 1);
        duk_dup(ctx, 2);
        duk_call(ctx, 1);

        return 1;
    }

    return 0;
}
