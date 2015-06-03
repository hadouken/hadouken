#include <Hadouken/Scripting/Modules/FileSystemModule.hpp>

#include <fstream>
#include <sstream>

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
        { "combine",         combine,         DUK_VARARGS },
        { "deleteFile",      deleteFile,      1 },
        { "directoryExists", directoryExists, 1 },
        { "fileExists",      fileExists,      1 },
        { "getFiles",        getFiles,        1 },
        { "isRelative",      isRelative,      1 },
        { "readBuffer",      readBuffer,      1 },
        { "readText",        readText,        1 },
        { "writeBuffer",     writeBuffer,     2 },
        { "writeText",       writeText,       2 },
        { NULL, NULL, 0 }
    };

    duk_put_function_list(ctx, 0 /* exports */, functions);
    return 0;
}

duk_ret_t FileSystemModule::combine(duk_context* ctx)
{
    int args = duk_get_top(ctx);

    Poco::Path path(duk_get_string(ctx, 0));
    
    for (int i = 1; i < duk_get_top(ctx); i++)
    {
        path.append(duk_get_string(ctx, i));
    }

    duk_push_string(ctx, path.toString().c_str());
    return 1;
}

duk_ret_t FileSystemModule::deleteFile(duk_context* ctx)
{
    std::string path(duk_require_string(ctx, 0));
    Poco::Path fp(path);
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

duk_ret_t FileSystemModule::directoryExists(duk_context* ctx)
{
    Poco::File f(duk_require_string(ctx, 0));
    duk_push_boolean(ctx, (f.exists() && f.isDirectory()));
    return 1;
}

duk_ret_t FileSystemModule::fileExists(duk_context* ctx)
{
    Poco::File f(duk_require_string(ctx, 0));
    duk_push_boolean(ctx, (f.exists() && f.isFile()));
    return 1;
}

duk_ret_t FileSystemModule::getFiles(duk_context* ctx)
{
    const char* rawInputPath = duk_require_string(ctx, 0);
    std::string inputPath(rawInputPath);

    Poco::Path fp(inputPath);
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

duk_ret_t FileSystemModule::isRelative(duk_context* ctx)
{
    duk_push_boolean(ctx, Poco::Path(duk_require_string(ctx, 0)).isRelative());
    return 1;
}

duk_ret_t FileSystemModule::readBuffer(duk_context* ctx)
{
    Poco::Path fp(duk_require_string(ctx, 0));
    Poco::File p(fp);

    if (p.exists())
    {
        std::FILE *fp = std::fopen(p.path().c_str(), "rb");

        if (fp)
        {
            std::fseek(fp, 0, SEEK_END);
            long size = std::ftell(fp);
            void* buffer = duk_push_buffer(ctx, size, false);

            std::rewind(fp);
            std::fread(buffer, 1, size, fp);
            std::fclose(fp);

            return 1;
        }
    }

    return 0;
}

duk_ret_t FileSystemModule::readText(duk_context* ctx)
{
    const char* rawPath = duk_require_string(ctx, 0);
    Poco::Path fp(rawPath);
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

duk_ret_t FileSystemModule::writeBuffer(duk_context* ctx)
{
    Poco::Path filePath(duk_require_string(ctx, 0));

    duk_size_t size;
    const char* buffer = static_cast<const char*>(duk_require_buffer(ctx, 1, &size));

    std::FILE *fp = std::fopen(filePath.toString().c_str(), "wb");

    if (fp)
    {
        size_t written = std::fwrite(buffer, sizeof(char), size, fp);
        std::fclose(fp);

        duk_push_number(ctx, written);
        return 1;
    }
}

duk_ret_t FileSystemModule::writeText(duk_context* ctx)
{
    Poco::Path filePath(duk_require_string(ctx, 0));

    duk_size_t size;
    const char* text = duk_require_lstring(ctx, 1, &size);

    std::FILE *fp = std::fopen(filePath.toString().c_str(), "wb");

    if (fp)
    {
        size_t written = std::fwrite(text, sizeof(char), size, fp);
        std::fclose(fp);

        duk_push_number(ctx, written);
        return 1;
    }
}
