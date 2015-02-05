#ifndef HDKN_BT_TORRENT_HANDLE_HPP
#define HDKN_BT_TORRENT_HANDLE_HPP

#ifdef WIN32
    #define HDKN_API __declspec(dllexport)
#else
    #define HDKN_API
#endif

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
