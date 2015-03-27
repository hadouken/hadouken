#ifndef AUTOMOVE_RULE_HPP
#define AUTOMOVE_RULE_HPP

#include <memory>
#include <regex>
#include <string>

#include "filter.hpp"

namespace AutoMove
{
    struct Rule
    {
        Rule(std::string p, Filter* filter)
            : filter(filter)
        {
            path = p;
        }

        std::string path;
        std::shared_ptr<Filter> filter;
    };
}

#endif
