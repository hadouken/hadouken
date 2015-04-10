#ifndef PUSHOVER_PUSHOVEREXTENSION_HPP
#define PUSHOVER_PUSHOVEREXTENSION_HPP

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

namespace Pushover
{
    class PushoverExtension : public Hadouken::Extensions::Extension
    {
    public:
        PushoverExtension();

        void load(Poco::Util::AbstractConfiguration& config);

        void unload();

    private:
        bool isEventEnabled(std::string eventName);
        void onTorrentAdded(const void* sender, std::shared_ptr<Hadouken::BitTorrent::TorrentHandle>& handle);
        void onTorrentFinished(const void* sender, std::shared_ptr<Hadouken::BitTorrent::TorrentHandle>& handle);

        void push(std::string title, std::string body);

        const Poco::URI messagesUrl_;
        Poco::Logger& logger_;
        std::string token_;
        std::string user_;
        std::vector<std::string> events_;
    };
}

#endif
