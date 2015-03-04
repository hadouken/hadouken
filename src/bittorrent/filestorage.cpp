#include <Hadouken/BitTorrent/FileStorage.hpp>

#include <Hadouken/BitTorrent/FileEntry.hpp>
#include <libtorrent/file_storage.hpp>

using namespace Hadouken::BitTorrent;

FileStorage::FileStorage(const libtorrent::file_storage& storage)
    : storage_(storage)
{
}

FileEntry FileStorage::getEntryAt(int index) const
{
    return FileEntry(storage_.at(index));
}

int FileStorage::getNumFiles() const
{
    return storage_.num_files();
}

int FileStorage::getNumPieces() const
{
    return storage_.num_pieces();
}

uint64_t FileStorage::getTotalSize() const
{
    return storage_.total_size();
}
