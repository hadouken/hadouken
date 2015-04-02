#ifndef AUTOMOVE_AUTOMOVEEXTENSION_HPP
#define AUTOMOVE_AUTOMOVEEXTENSION_HPP

#include <Hadouken/Extensions/Extension.hpp>

#include "rule.hpp"
#include <Poco/Logger.h>
#include <Poco/Util/AbstractConfiguration.h>

#include <memory>
#include <vector>

namespace Hadouken
{
    namespace BitTorrent
    {
        struct TorrentHandle;
    }
}

namespace AutoMove
{
    class AutoMoveExtension : public Hadouken::Extensions::Extension
    {
    public:
        AutoMoveExtension();

        void load(Poco::Util::AbstractConfiguration& config);

        void unload();

    private:
        void onTorrentCompleted(const void* sender, std::shared_ptr<Hadouken::BitTorrent::TorrentHandle>& handle);

        Poco::Logger& logger_;
        std::vector<Rule> rules_;
    };
}

#endif
