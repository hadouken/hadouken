#ifndef AUTOMOVE_AUTOMOVEEXTENSION_HPP
#define AUTOMOVE_AUTOMOVEEXTENSION_HPP

#include <Hadouken/Extensions/Extension.hpp>

#include "rule.hpp"
#include <Poco/Logger.h>
#include <Poco/Util/AbstractConfiguration.h>
#include <vector>

namespace Hadouken
{
    namespace BitTorrent
    {
        class TorrentHandle;
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
        std::string getFieldValue(Hadouken::BitTorrent::TorrentHandle& handle, std::string fieldName);

        void onTorrentCompleted(const void* sender, Hadouken::BitTorrent::TorrentHandle& handle);

        Poco::Logger& logger_;
        std::vector<Rule> rules_;
    };
}

#endif
