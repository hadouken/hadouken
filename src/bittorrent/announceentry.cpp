#include <Hadouken/BitTorrent/AnnounceEntry.hpp>

#include <libtorrent/torrent_info.hpp>

using namespace Hadouken::BitTorrent;

AnnounceEntry::AnnounceEntry(libtorrent::announce_entry& entry)
    : announce_(new libtorrent::announce_entry(entry))
{
}

AnnounceEntry::~AnnounceEntry()
{
}

std::string AnnounceEntry::getMessage() const
{
    return announce_->message;
}

uint8_t AnnounceEntry::getTier() const
{
    return announce_->tier;
}

std::string AnnounceEntry::getUrl() const
{
    return announce_->url;
}

bool AnnounceEntry::isUpdating() const
{
    return announce_->updating;
}

bool AnnounceEntry::isVerified() const
{
    return announce_->verified;
}

void AnnounceEntry::reset()
{
    announce_->reset();
}
