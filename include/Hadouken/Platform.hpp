#ifndef HADOUKEN_PLATFORM_HPP
#define HADOUKEN_PLATFORM_HPP

/*
General header file for platform-specific functions.
*/

#include <Hadouken/Config.hpp>
#include <Poco/Path.h>
#include <string>

namespace Hadouken
{
    class Platform
    {
    public:
        static HDKN_EXPORT Poco::Path getApplicationDataPath();

        static HDKN_EXPORT Poco::Path getApplicationPath();
    };
}

#endif
