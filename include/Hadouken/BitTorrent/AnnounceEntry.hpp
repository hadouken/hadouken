#ifndef HADOUKEN_BITTORRENT_ANNOUNCEENTRY_HPP
#define HADOUKEN_BITTORRENT_ANNOUNCEENTRY_HPP

#include <Hadouken/Config.hpp>
#include <memory>
#include <string>

namespace libtorrent
{
    struct announce_entry;
}

namespace Hadouken
{
    namespace BitTorrent
    {
        struct AnnounceEntry
        {
            explicit AnnounceEntry(libtorrent::announce_entry& entry);
            ~AnnounceEntry();

            HDKN_EXPORT std::string getMessage() const;

            HDKN_EXPORT std::string getUrl() const;

            HDKN_EXPORT bool isUpdating() const;

            HDKN_EXPORT void reset();

        private:
            std::shared_ptr<libtorrent::announce_entry> announce_;
        };
    }
}

#endif
