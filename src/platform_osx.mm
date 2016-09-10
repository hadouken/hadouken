#ifdef __APPLE__

#include <boost/filesystem.hpp>
#include <hadouken/platform.hpp>

#include <cstdlib>
#include <mach-o/dyld.h>

#import <Foundation/Foundation.h>

using namespace hadouken;
namespace fs = boost::filesystem;

void platform::init() {}

fs::path platform::data_path()
{
    NSFileManager *fileManager = [NSFileManager defaultManager];
    NSURL *resultURL = [fileManager URLForDirectory:NSDownloadsDirectory inDomain:NSUserDomainMask appropriateForURL:nil create:NO error:nil];
    if (resultURL != nil)
    {
        return fs::path([resultURL fileSystemRepresentation]);
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
