#include <Hadouken/BitTorrent/FileEntry.hpp>

#include <libtorrent/file_storage.hpp>

using namespace Hadouken::BitTorrent;

FileEntry::FileEntry(const libtorrent::file_entry& entry)
    : entry_(entry)
{
}

std::string FileEntry::getPath() const
{
    return entry_.path;
}

std::string FileEntry::getSymlinkPath() const
{
    return entry_.symlink_path;
}

uint64_t FileEntry::getOffset() const
{
    return entry_.offset;
}

uint64_t FileEntry::getSize() const
{
    return entry_.size;
}
