#include <Hadouken/Scripting/Modules/FileSystemModule.hpp>

#include <fstream>
#include <sstream>
#include <Poco/File.h>
#include <Poco/Path.h>
#include <Poco/Util/Application.h>

#include "../duktape.h"

using namespace Hadouken::Scripting::Modules;
using namespace Poco::Util;

duk_ret_t FileSystemModule::initialize(duk_context* ctx)
{
    duk_function_list_entry functions[] =
    {
        { "getFiles", FileSystemModule::getFiles, 1 },
        { "readFile", FileSystemModule::readFile, 1 },
        { NULL, NULL, 0 }
    };

    duk_put_function_list(ctx, 0 /* exports */, functions);
    return 0;
}

duk_ret_t FileSystemModule::getFiles(duk_context* ctx)
{
    const char* rawInputPath = duk_require_string(ctx, 0);
    std::string inputPath(rawInputPath);

    Poco::Path fp(inputPath);

    if (fp.isRelative())
    {
        Application& app = Application::instance();
        std::string scriptPath = app.config().getString("scripting.path");

        fp.makeAbsolute(scriptPath);
    }

    Poco::File p(fp);

    if (!p.exists())
    {
        return 0;
    }

    std::vector<Poco::File> files;
    p.list(files);
    std::vector<Poco::File>::iterator it = files.begin();

    int arrayIndex = duk_push_array(ctx);
    int i = 0;

    for (; it != files.end(); ++it)
    {
        duk_push_string(ctx, it->path().c_str());
        duk_put_prop_index(ctx, arrayIndex, i);

        ++i;
    }

    return 1;
}

duk_ret_t FileSystemModule::readFile(duk_context* ctx)
{
    const char* rawPath = duk_require_string(ctx, 0);
    Poco::Path fp(rawPath);

    if (fp.isRelative())
    {
        Application& app = Application::instance();
        std::string scriptPath = app.config().getString("scripting.path");

        fp.makeAbsolute(scriptPath);
    }

    Poco::File p(fp);

    if (p.exists())
    {
        std::ifstream reader(p.path(), std::ios::binary);
        std::stringstream ss;

        std::copy(
            std::istreambuf_iterator<char>(reader),
            std::istreambuf_iterator<char>(),
            std::ostreambuf_iterator<char>(ss));

        duk_push_string(ctx, ss.str().c_str());
        return 1;
    }

    return 0;
}
