#ifndef HDKN_BT_TORRENT_HANDLE_HPP
#define HDKN_BT_TORRENT_HANDLE_HPP

#include <hadouken/config.hpp>
#include <string>

namespace libtorrent
{
    struct torrent_handle;
}

namespace hadouken
{
    namespace bittorrent
    {
        class HDKN_API torrent_handle
        {
        public:
            torrent_handle(const libtorrent::torrent_handle& handle);

            void move_storage(const std::string& save_path, int flags = 0) const;

            std::string name() const;

        private:
            const libtorrent::torrent_handle& handle_;
        };
    }
}

#endif
