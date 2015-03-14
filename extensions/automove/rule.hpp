#ifndef AUTOMOVE_RULE_HPP
#define AUTOMOVE_RULE_HPP

#include <regex>
#include <string>

namespace AutoMove
{
    struct Rule
    {
        Rule(std::string field, std::regex pattern, std::string targetPath)
        {
            field_ = field;
            pattern_ = pattern;
            targetPath_ = targetPath;
        }

        std::string getField() const
        {
            return field_;
        }

        std::regex getPattern() const
        {
            return pattern_;
        }

        std::string getTargetPath() const
        {
            return targetPath_;
        }

    private:
        std::string field_;
        std::regex pattern_;
        std::string targetPath_;
    };
}

#endif
