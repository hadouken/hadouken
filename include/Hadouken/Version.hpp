#ifndef HADOUKEN_VERSION_HPP
#define HADOUKEN_VERSION_HPP

#include <string>

namespace Hadouken
{
    struct Version
    {
        static std::string GIT_BRANCH();
        static std::string GIT_COMMIT_HASH();
        static std::string VERSION();
    };
}

#endif
