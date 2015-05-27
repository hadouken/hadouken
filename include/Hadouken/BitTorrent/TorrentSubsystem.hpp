#ifndef HADOUKEN_BITTORRENT_TORRENT_SUBSYSTEM_HPP
#define HADOUKEN_BITTORRENT_TORRENT_SUBSYSTEM_HPP

#include <Hadouken/Config.hpp>
#include <Poco/Util/Application.h>
#include <Poco/Util/Subsystem.h>

namespace libtorrent
{
    class session;
}

using namespace Poco::Util;

namespace Hadouken
{
    namespace BitTorrent
    {
        class Session;

        class TorrentSubsystem : public Subsystem
        {
        public:
            HDKN_EXPORT libtorrent::session& getSession();

        protected:
            HDKN_EXPORT void initialize(Application& app);

            HDKN_EXPORT void uninitialize();

            HDKN_EXPORT const char* name() const;

        private:
            libtorrent::session* session_;
        };
    }
}

#endif