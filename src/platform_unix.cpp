#ifdef __unix__

#include <boost/filesystem.hpp>
#include <hadouken/platform.hpp>
#include <limits.h>
#include <unistd.h>

using namespace hadouken;
namespace fs = boost::filesystem;

void platform::init()
{
}

fs::path platform::data_path()
{
    // TODO: do something useful.
    return "/etc/hadouken/";
}

fs::path platform::application_path()
{
    char result[PATH_MAX];
    ssize_t count = readlink("/proc/self/exe", result, PATH_MAX);
    std::string p(result, (count > 0) ? count : 0);

    return fs::path(p).parent_path();
}

fs::path platform::get_current_directory()
{
    return fs::initial_path();
}

int platform::launch_process(const std::string& executable, const std::vector<std::string>& args)
{
}

#endif
