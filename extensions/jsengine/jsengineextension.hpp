#ifndef PUSHBULLET_PUSHBULLETEXTENSION_HPP
#define PUSHBULLET_PUSHBULLETEXTENSION_HPP

#include <Hadouken/Extensions/Extension.hpp>
#include <Poco/Logger.h>
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
        void onTorrentCompleted(const void* sender, Hadouken::BitTorrent::TorrentHandle& handle);

        Poco::Logger& logger_;
        duk_context* ctx_;
    };
}

#endif
