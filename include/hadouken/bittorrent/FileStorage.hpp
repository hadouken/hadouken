#ifndef HADOUKEN_BITTORRENT_FILESTORAGE_HPP
#define HADOUKEN_BITTORRENT_FILESTORAGE_HPP

namespace libtorrent
{
    class file_storage;
}

namespace Hadouken
{
    namespace BitTorrent
    {
        struct FileEntry;

        class FileStorage
        {
        public:
            FileStorage(const libtorrent::file_storage& storage);

            FileEntry getEntryAt(int index) const;

            int getNumFiles() const;

            int getNumPieces() const;

            size_t getTotalSize() const;

        private:
            const libtorrent::file_storage& storage_;
        };
    }
}

#endif
