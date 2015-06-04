#ifndef HADOUKEN_PLATFORM_HPP
#define HADOUKEN_PLATFORM_HPP

/*
General header file for platform-specific functions.
*/

#include <boost/filesystem.hpp>
#include <string>
#include <vector>

namespace hadouken
{
    class platform
    {
    public:
        static void init();

#ifdef WIN32
        static void install_service();

        static void uninstall_service();
#endif

        static boost::filesystem::path data_path();

        static boost::filesystem::path application_path();

        static int launch_process(std::string executable, std::vector<std::string> args);
    };
}

#endif
