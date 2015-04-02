#include "patternfilter.hpp"

#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/BitTorrent/TorrentStatus.hpp>

using namespace AutoMove;
using namespace Hadouken::BitTorrent;

bool PatternFilter::isMatch(std::shared_ptr<TorrentHandle>& handle)
{
    std::string fieldValue = getFieldValue(field_, handle);

    if (fieldValue.empty())
    {
        return false;
    }

    return std::regex_match(fieldValue, pattern_);
}

std::string PatternFilter::getFieldValue(std::string fieldName, std::shared_ptr<TorrentHandle>& handle)
{
    TorrentStatus status = handle->getStatus();

    if (fieldName == "name")
    {
        return status.getName();
    }
    else if (fieldName == "savePath")
    {
        return status.getSavePath();
    }

    return std::string();
}