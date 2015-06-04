#ifndef HADOUKEN_VERSION_HPP
#define HADOUKEN_VERSION_HPP

#include <string>

namespace hadouken
{
    struct version
    {
        static std::string GIT_BRANCH();
        static std::string GIT_COMMIT_HASH();
        static std::string VERSION();
    };
}

#endif
