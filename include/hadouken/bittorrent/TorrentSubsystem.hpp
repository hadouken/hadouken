#ifndef HADOUKEN_BITTORRENT_TORRENT_SUBSYSTEM_HPP
#define HADOUKEN_BITTORRENT_TORRENT_SUBSYSTEM_HPP

#include <Poco/Util/Application.h>
#include <Poco/Util/Subsystem.h>

using namespace Poco::Util;

namespace Hadouken
{
    namespace BitTorrent
    {
        class Session;

        class TorrentSubsystem : public Subsystem
        {
        public:
            Session& getSession();

        protected:
            void initialize(Application& app);

            void uninitialize();

            const char* name() const;

        private:
            Session* sess_;
        };
    }
}

#endif