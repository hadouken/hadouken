#include <hadouken/scripting/script_host.hpp>

#include <hadouken/application.hpp>
#include <hadouken/scripting/modules/bittorrent/alert_wrapper.hpp>
#include <hadouken/scripting/modules/bencoding_module.hpp>
#include <hadouken/scripting/modules/bittorrent_module.hpp>
#include <hadouken/scripting/modules/core_module.hpp>
#include <hadouken/scripting/modules/file_system_module.hpp>
#include <hadouken/scripting/modules/http_module.hpp>
#include <hadouken/scripting/modules/logger_module.hpp>
#include <hadouken/scripting/modules/process_module.hpp>

#include <chrono>
#include <boost/filesystem.hpp>
#include <boost/log/trivial.hpp>
#include <boost/property_tree/json_parser.hpp>
#include <libtorrent/session.hpp>

#include "duktape.h"
#include "modules/common.hpp"

using namespace hadouken::scripting;
namespace fs = boost::filesystem;

script_host::script_host(hadouken::application& app)
    : app_(app)
{
    ctx_ = duk_create_heap_default();
}

script_host::~script_host()
{
    duk_destroy_heap(ctx_);
}

void script_host::load(fs::path scriptPath)
{
    std::lock_guard<std::mutex> lock(ctx_mutex_);

    fs::path script(scriptPath);
    script /= "hadouken.js";

    if (!fs::exists(script))
    {
        BOOST_LOG_TRIVIAL(fatal) << "Scripting subsystem cannot find script: " << script;
        return;
    }

    // Set up requireNative
    duk_push_c_function(ctx_, &script_host::require_native, DUK_VARARGS);
    duk_put_global_string(ctx_, "requireNative");

    duk_push_string(ctx_, script.parent_path().string().c_str());
    duk_put_global_string(ctx_, "__ROOT__");

    duk_push_pointer(ctx_, this);
    duk_put_global_string(ctx_, "\xff" "host");

    duk_idx_t idx = duk_push_object(ctx_);

    if (duk_peval_file(ctx_, script.string().c_str()) != 0)
    {
        std::string error(duk_safe_to_string(ctx_, -1));
        BOOST_LOG_TRIVIAL(fatal) << "Could not evaluate script file: " << error;;
    }

    // Top of the stack should contain a callable function
    if (!duk_is_callable(ctx_, -1))
    {
        BOOST_LOG_TRIVIAL(fatal) << "Script did not return a callable function.";
    }
    else
    {
        duk_dup(ctx_, -1);
        duk_dup(ctx_, idx);

        if (duk_pcall_method(ctx_, 1) != DUK_EXEC_SUCCESS)
        {
            std::string error(duk_safe_to_string(ctx_, -1));
            BOOST_LOG_TRIVIAL(fatal) << "Error when executing script: " << error;
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
                BOOST_LOG_TRIVIAL(fatal) << "Type 'load' is not callable.";
                return;
            }

            duk_dup(ctx_, -1);

            if (duk_pcall_method(ctx_, 0) != DUK_EXEC_SUCCESS)
            {
                std::string error(duk_safe_to_string(ctx_, -1));
                BOOST_LOG_TRIVIAL(fatal) << "Error when calling 'load': " << error;
            }

            duk_pop(ctx_);
        }

        duk_pop(ctx_);
    }

    duk_pop(ctx_);
    
    is_running_ = true;
    ticker_ = std::thread(std::bind(&script_host::tick, this));
}

void script_host::unload()
{
    is_running_ = false;
    ticker_.join();

    std::lock_guard<std::mutex> lock(ctx_mutex_);

    // Call unload
    duk_push_global_stash(ctx_);

    if (duk_get_prop_string(ctx_, -1, "hdkn"))
    {
        if (duk_get_prop_string(ctx_, -1, "unload"))
        {
            if (!duk_is_callable(ctx_, -1))
            {
                BOOST_LOG_TRIVIAL(fatal) << "Type 'unload' is not callable.";
                return;
            }

            duk_dup(ctx_, -1);

            if (duk_pcall_method(ctx_, 0) != DUK_EXEC_SUCCESS)
            {
                std::string error(duk_safe_to_string(ctx_, -1));
                BOOST_LOG_TRIVIAL(fatal) << "Error when calling 'unload': " << error;
            }

            duk_pop(ctx_);
        }

        duk_pop(ctx_);
    }

    duk_pop(ctx_);
}

void script_host::emit(std::string name, libtorrent::alert* alert)
{
    std::lock_guard<std::mutex> lock(ctx_mutex_);

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
                BOOST_LOG_TRIVIAL(fatal) << "Type 'emit' is not callable.";
                return;
            }

            duk_dup(ctx_, -1);
            duk_push_string(ctx_, name.c_str());

            if (alert)
            {
                modules::bittorrent::alert_wrapper::construct(ctx_, alert);
            }
            else
            {
                duk_push_undefined(ctx_);
            }

            if (duk_pcall_method(ctx_, 2) != DUK_EXEC_SUCCESS)
            {
                std::string error(duk_safe_to_string(ctx_, -1));
                BOOST_LOG_TRIVIAL(error) << "Error when emitting alert '" << name << "': " << error;
            }

            duk_pop(ctx_);
        }

        duk_pop(ctx_);
    }

    duk_pop(ctx_);
}

std::string script_host::rpc(std::string request)
{
    std::lock_guard<std::mutex> lock(ctx_mutex_);
    
    std::string result;
    duk_push_global_stash(ctx_);
    
    if (duk_get_prop_string(ctx_, -1, "hdkn"))
    {
        if (duk_get_prop_string(ctx_, -1, "rpc"))
        {
            if (!duk_is_callable(ctx_, -1))
            {
                BOOST_LOG_TRIVIAL(error) << "Type 'rpc' is not callable.";
                return std::string();
            }

            duk_dup(ctx_, -1);
            
            duk_push_string(ctx_, request.c_str());

            try
            {
                duk_json_decode(ctx_, -1);
            }
            catch (const std::exception& ex)
            {
                BOOST_LOG_TRIVIAL(error) << "Error when decoding JSON";
            }

            if (duk_pcall_method(ctx_, 1) != DUK_EXEC_SUCCESS)
            {
                std::string error(duk_safe_to_string(ctx_, -1));
                BOOST_LOG_TRIVIAL(error) << "Error when handling RPC method: " << error;
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

void script_host::define_global(std::string variable, std::string value)
{
    std::lock_guard<std::mutex> lock(ctx_mutex_);
    
    duk_push_string(ctx_, value.c_str());
    duk_put_global_string(ctx_, variable.c_str());
}

bool script_host::is_authenticated(std::string authHeader)
{
    std::lock_guard<std::mutex> lock(ctx_mutex_);

    bool result;
    duk_push_global_stash(ctx_);

    if (duk_get_prop_string(ctx_, -1, "hdkn"))
    {
        if (duk_get_prop_string(ctx_, -1, "authenticator"))
        {
            if (!duk_is_callable(ctx_, -1))
            {
                BOOST_LOG_TRIVIAL(error) << "Type 'authenticator' is not callable.";
                return false;
            }

            duk_dup(ctx_, -1);
            duk_push_string(ctx_, authHeader.c_str());
            
            if (duk_pcall_method(ctx_, 1) != DUK_EXEC_SUCCESS)
            {
                std::string error(duk_safe_to_string(ctx_, -1));
                BOOST_LOG_TRIVIAL(error) << "Error when authenticating request: " << error;
            }

            if (duk_get_type(ctx_, -1) == DUK_TYPE_BOOLEAN)
            {
                result = duk_get_boolean(ctx_, -1);
            }

            duk_pop(ctx_);
        }

        duk_pop(ctx_);
    }

    duk_pop(ctx_);

    return result;
}

void script_host::tick()
{
    while (is_running_)
    {
        emit("tick", nullptr);
        std::this_thread::sleep_for(std::chrono::seconds(1));
    }
}

duk_ret_t script_host::require_native(duk_context* ctx)
{
    duk_get_global_string(ctx, "\xff" "host");
    script_host* host = static_cast<script_host*>(duk_get_pointer(ctx, -1));
    duk_pop(ctx);

    const char* moduleName = duk_require_string(ctx, 0);

    if (strcmp("fs", moduleName) == 0)
    {
        duk_push_c_function(ctx, &modules::file_system_module::initialize, 1);
        duk_dup(ctx, 2);
        duk_call(ctx, 1);

        return 1;
    }
    else if (strcmp("bittorrent", moduleName) == 0)
    {
        return modules::bittorrent_module::initialize(ctx, host->app_.session());
    }
    else if (strcmp("core", moduleName) == 0)
    {
        duk_push_c_function(ctx, &modules::core_module::initialize, 1);
        duk_dup(ctx, 2);
        duk_call(ctx, 1);

        return 1;
    }
    else if (strcmp("logger", moduleName) == 0)
    {
        duk_push_c_function(ctx, &modules::logger_module::initialize, 1);
        duk_dup(ctx, 2);
        duk_call(ctx, 1);

        return 1;
    }
    else if (strcmp("http", moduleName) == 0)
    {
        duk_push_c_function(ctx, &modules::http_module::initialize, 1);
        duk_dup(ctx, 2);
        duk_call(ctx, 1);

        return 1;
    }
    else if (strcmp("process", moduleName) == 0)
    {
        duk_push_c_function(ctx, &modules::process_module::initialize, 1);
        duk_dup(ctx, 2);
        duk_call(ctx, 1);

        return 1;
    }
    else if (strcmp("benc", moduleName) == 0)
    {
        duk_push_c_function(ctx, &modules::bencoding_module::initialize, 1);
        duk_dup(ctx, 2);
        duk_call(ctx, 1);

        return 1;
    }

    return 0;
}
