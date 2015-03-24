#include "jsengineextension.hpp"
#include "modules/bittorrent.hpp"
#include "modules/config.hpp"
#include "modules/fs.hpp"
#include "timer.hpp"

#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Delegate.h>
#include <Poco/Clock.h>
#include <Poco/ScopedLock.h>
#include <Poco/Util/Application.h>

using namespace Hadouken::BitTorrent;
using namespace JsEngine;
using namespace JsEngine::Modules;
using namespace Poco::Util;

duk_ret_t requireNative(duk_context* ctx)
{
    // arg0: id
    // arg1: require
    // arg2: exports
    // arg3: module

    const char* mod = duk_require_string(ctx, 0);
    std::string moduleName(mod);

    if (moduleName == "bittorrent")
    {
        duk_push_c_function(ctx, BitTorrent::init, 1);
        duk_dup(ctx, 2);
        duk_call(ctx, 1);

        return 1;
    }
    else if (moduleName == "config")
    {
        duk_push_c_function(ctx, dukopen_config, 1);
        duk_dup(ctx, 2);
        duk_call(ctx, 1);

        return 1;
    }
    else if (moduleName == "fs")
    {
        duk_push_c_function(ctx, FileSystem::init, 1);
        duk_dup(ctx, 2);
        duk_call(ctx, 1);

        return 1;
    }

    return 0;
}

JsEngineExtension::JsEngineExtension()
    : logger_(Poco::Logger::get("jsengine.jsengineextension")),
      run_adapter_(*this, &JsEngineExtension::run)
{
}

void JsEngineExtension::load(AbstractConfiguration& config)
{
    Application& app = Application::instance();
    Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();

    // Hook up event handlers
    sess.onTorrentAdded += Poco::delegate(this, &JsEngineExtension::onTorrentAdded);

    is_running_ = true;
    run_thread_.start(run_adapter_);
}

void JsEngineExtension::unload()
{
    Application& app = Application::instance();
    Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();

    // Tear down event handlers
    sess.onTorrentAdded -= Poco::delegate(this, &JsEngineExtension::onTorrentAdded);

    is_running_ = false;
    run_thread_.join();
}

void JsEngineExtension::run()
{
    duk_context* ctx = duk_create_heap_default();

    if (!ctx)
    {
        logger_.error("Could not create Duktape heap.");
        return;
    }

    initContext(ctx);
    Timer::init(ctx);

    runScript(ctx);

    uint32_t msgTO = 0x7FFFFFFF;
    Poco::Clock timerClock;

    while (is_running_)
    {
        Timer::run(ctx, timerClock, &msgTO);
        fireEvents(ctx);

        uint32_t sleep = std::min((uint32_t)100, msgTO);
        Poco::Thread::sleep(sleep);
    }

    duk_destroy_heap(ctx);
}

void JsEngineExtension::initContext(duk_context* ctx)
{
    // Add functions
    duk_push_c_function(ctx, requireNative, DUK_VARARGS);
    duk_put_global_string(ctx, "requireNative");
}

void JsEngineExtension::runScript(duk_context* ctx)
{
    Application& app = Application::instance();
    AbstractConfiguration& config = app.config();

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

    if (duk_peval_file(ctx, scriptFile.c_str()) != 0)
    {
        std::string error(duk_safe_to_string(ctx, -1));
        logger_.error("Could not evaluate script file: %s", error);
    }

    // Ignore result
    duk_pop(ctx);
}

void JsEngineExtension::fireEvents(duk_context* ctx)
{
    Poco::Mutex::ScopedLock lock(event_mutex_);

    if (event_data_.empty()) return;

    if (duk_get_global_string(ctx, "EventEmitter"))
    {
        duk_idx_t evIdx = duk_get_top_index(ctx);
        duk_get_prop_string(ctx, evIdx, "emit");

        if (duk_is_callable(ctx, -1))
        {
            duk_dup(ctx, evIdx);

            for (int i = 0; i < event_data_.size(); i++)
            {
                event_t data = event_data_.front();

                duk_push_string(ctx, data.first.c_str());

                if (duk_pcall_method(ctx, 1) != DUK_EXEC_SUCCESS)
                {
                    logger_.error("Could not fire events.");
                }

                event_data_.pop();
                duk_pop(ctx);
            }
        }
    }
}

void JsEngineExtension::onTorrentAdded(const void* sender, TorrentHandle& handle)
{
    Poco::Mutex::ScopedLock lock(event_mutex_);

    std::string eventName = "torrent.added";
    event_data_.push(std::make_pair(eventName, &handle));
}