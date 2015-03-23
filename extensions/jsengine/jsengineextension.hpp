#ifndef JSENGINE_JSENGINEEXTENSION_HPP
#define JSENGINE_JSENGINEEXTENSION_HPP

#include <Hadouken/Extensions/Extension.hpp>
#include <Poco/Logger.h>
#include <Poco/RunnableAdapter.h>
#include <Poco/Thread.h>
#include <Poco/Util/AbstractConfiguration.h>

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

        bool is_running_;
        Poco::RunnableAdapter<JsEngineExtension> run_adapter_;
        Poco::Thread run_thread_;
        Poco::Logger& logger_;
    };
}

#endif
