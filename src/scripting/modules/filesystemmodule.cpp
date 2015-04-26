#include <Hadouken/Scripting/Modules/FileSystemModule.hpp>

#include <fstream>
#include <sstream>
#include <Hadouken/Scripting/ScriptingSubsystem.hpp>
#include <Poco/File.h>
#include <Poco/Path.h>
#include <Poco/Util/Application.h>

#include "../duktape.h"

using namespace Hadouken::Scripting;
using namespace Hadouken::Scripting::Modules;
using namespace Poco::Util;

duk_ret_t FileSystemModule::initialize(duk_context* ctx)
{
    duk_function_list_entry functions[] =
    {
        { "deleteFile", deleteFile, 1 },
        { "getFiles",   getFiles,   1 },
        { "readFile",   readFile,   1 },
        { NULL, NULL, 0 }
    };

    duk_put_function_list(ctx, 0 /* exports */, functions);
    return 0;
}

duk_ret_t FileSystemModule::deleteFile(duk_context* ctx)
{
    std::string path(duk_require_string(ctx, 0));
    Poco::Path fp(path);

    if (fp.isRelative())
    {
        Application& app = Application::instance();
        std::string scriptPath = app.getSubsystem<ScriptingSubsystem>().getScriptPath();

        fp.makeAbsolute(scriptPath);
    }

    Poco::File p(fp);

    if (!p.exists())
    {
        duk_push_error_object(ctx, DUK_ERR_ERROR, "File not found: %s", fp.toString().c_str());
        return 1;
    }

    if (!p.isFile())
    {
        duk_push_error_object(ctx, DUK_ERR_ERROR, "Path is not a file: %s", fp.toString().c_str());
        return 1;
    }

    p.remove();
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
        std::string scriptPath = app.getSubsystem<ScriptingSubsystem>().getScriptPath();

        fp.makeAbsolute(scriptPath);
    }

    Poco::File p(fp);

    if (!p.exists())
    {
        return 0;
    }

    std::vector<Poco::File> files;
    p.list(files);

    int arrayIndex = duk_push_array(ctx);
    int i = 0;

    for (Poco::File file : files)
    {
        duk_push_string(ctx, file.path().c_str());
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
        std::string scriptPath = app.getSubsystem<ScriptingSubsystem>().getScriptPath();

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
