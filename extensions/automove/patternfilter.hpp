#ifndef AUTOMOVE_PATTERNFILTER_HPP
#define AUTOMOVE_PATTERNFILTER_HPP

#include "filter.hpp"

#include <regex>
#include <string>

namespace AutoMove
{
    struct PatternFilter : public Filter
    {
    public:
        PatternFilter(std::regex pattern, std::string field)
        {
            pattern_ = pattern;
            field_ = field;
        }

        bool isMatch(Hadouken::BitTorrent::TorrentHandle& handle);

    private:
        std::string getFieldValue(std::string fieldName, Hadouken::BitTorrent::TorrentHandle& handle);

        std::regex pattern_;
        std::string field_;
    };
}

#endif
