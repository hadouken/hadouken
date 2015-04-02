#ifndef PUSHBULLET_PUSHBULLETEXTENSION_HPP
#define PUSHBULLET_PUSHBULLETEXTENSION_HPP

#include <Hadouken/Extensions/Extension.hpp>

#include <Poco/Logger.h>
#include <Poco/URI.h>
#include <Poco/Util/AbstractConfiguration.h>

#include <memory>

namespace Hadouken
{
    namespace BitTorrent
    {
        struct TorrentHandle;
    }
}

namespace Pushbullet
{
    class PushbulletExtension : public Hadouken::Extensions::Extension
    {
    public:
        PushbulletExtension();

        void load(Poco::Util::AbstractConfiguration& config);

        void unload();

    private:
        bool isEventEnabled(std::string eventName);
        void onTorrentAdded(const void* sender, std::shared_ptr<Hadouken::BitTorrent::TorrentHandle>& handle);
        void onTorrentFinished(const void* sender, std::shared_ptr<Hadouken::BitTorrent::TorrentHandle>& handle);

        void push(std::string title, std::string body);

        const Poco::URI pushesUrl_;
        Poco::Logger& logger_;
        std::string authToken_;
        std::vector<std::string> events_;
    };
}

#endif
