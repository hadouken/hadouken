#ifdef __APPLE__

#include <boost/filesystem.hpp>
#include <hadouken/platform.hpp>

#include <cstdlib>
#include <mach-o/dyld.h>

using namespace hadouken;
namespace fs = boost::filesystem;

void platform::init() {}

fs::path platform::data_path() 
{
    // TODO: find folders using URLsForDirectory

    char *home_path = getenv("HOME");

    if(home_path != nullptr) 
    {
        std::string p(home_path);
        p += "/Downloads";
        return fs::path(p);
    }

    return "";
}

fs::path platform::application_path() 
{
    char path[2048];
    uint32_t size = sizeof(path);

    if (_NSGetExecutablePath(path, &size) == 0) 
    {
        return fs::path(path).parent_path();
    }

    return "";
}

fs::path platform::get_current_directory() 
{ 
    return fs::initial_path(); 
}

int platform::launch_process(const std::string& executable,
                             const std::vector<std::string>& args) 
{
    return 0;
}

#endif
