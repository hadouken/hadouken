#ifndef JSENGINE_JSENGINEEXTENSION_HPP
#define JSENGINE_JSENGINEEXTENSION_HPP

#include <Hadouken/Extensions/Extension.hpp>
#include <Poco/Logger.h>
#include <Poco/Mutex.h>
#include <Poco/RunnableAdapter.h>
#include <Poco/Thread.h>
#include <Poco/Util/AbstractConfiguration.h>
#include <queue>

#include "duktape.h"

namespace Hadouken
{
    namespace BitTorrent
    {
        struct TorrentHandle;
    }
}

namespace JsEngine
{
    typedef std::pair<std::string, void*> event_t;

    class JsEngineExtension : public Hadouken::Extensions::Extension
    {
    public:
        JsEngineExtension();

        void load(Poco::Util::AbstractConfiguration& config);

        void unload();

    private:
        void run();
        void initContext(duk_context* ctx);
        void runScript(duk_context* ctx);
        void fireEvents(duk_context* ctx);

        void onTorrentAdded(const void* sender, Hadouken::BitTorrent::TorrentHandle& handle);
        void onTorrentFinished(const void* sender, Hadouken::BitTorrent::TorrentHandle& handle);

        bool is_running_;
        Poco::RunnableAdapter<JsEngineExtension> run_adapter_;
        Poco::Thread run_thread_;
        Poco::Logger& logger_;
        Poco::Mutex event_mutex_;
        std::queue<event_t> event_data_;
    };
}

#endif
