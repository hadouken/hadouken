#ifndef AUTOMOVE_TAGSFILTER_HPP
#define AUTOMOVE_TAGSFILTER_HPP

#include "filter.hpp"

#include <string>
#include <vector>

namespace AutoMove
{
    struct TagsFilter : public Filter
    {
    public:
        TagsFilter(std::vector<std::string> tags)
        {
            tags_ = tags;
        }

        bool isMatch(Hadouken::BitTorrent::TorrentHandle& handle);

    private:
        std::vector<std::string> tags_;
    };
}

#endif
