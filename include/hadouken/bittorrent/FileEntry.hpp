#ifndef HADOUKEN_BITTORRENT_FILEENTRY_HPP
#define HADOUKEN_BITTORRENT_FILEENTRY_HPP

#include <libtorrent/file_storage.hpp>
#include <string>

namespace Hadouken
{
    namespace BitTorrent
    {
        struct FileEntry
        {
            FileEntry(const libtorrent::file_entry& entry);

            std::string getPath() const;

            std::string getSymlinkPath() const;

            size_t getOffset() const;

            size_t getSize() const;


        private:
            const libtorrent::file_entry entry_;
        };
    }
}

#endif
