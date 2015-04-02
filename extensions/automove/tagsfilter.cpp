#include "tagsfilter.hpp"

#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/BitTorrent/TorrentStatus.hpp>

using namespace AutoMove;
using namespace Hadouken::BitTorrent;

bool TagsFilter::isMatch(std::shared_ptr<TorrentHandle>& handle)
{
    for (std::string tag : tags_)
    {
        if (!handle->hasTag(tag))
        {
            return false;
        }
    }

    return true;
}
