#include "mod_fs.hpp"

#include <Poco/File.h>
#include <Poco/Path.h>
#include <fstream>
#include <sstream>

duk_ret_t fs_getFiles(duk_context*);
duk_ret_t fs_readFile(duk_context*);

static const duk_function_list_entry fs_funcs[] = {
    { "getFiles", fs_getFiles, 1 },
    { "readFile", fs_readFile, 1 },
    { NULL, NULL, 0 }
};

duk_ret_t fs_getFiles(duk_context* ctx)
{
    int argCount = duk_get_top(ctx);
    std::string inputPath(duk_get_string(ctx, 0));
    
    Poco::File p(inputPath);
    std::vector<Poco::File> files;

    if (!p.exists())
    {
        return 0;
    }

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

duk_ret_t fs_readFile(duk_context* ctx)
{
    int argCount = duk_get_top(ctx);
    std::string inputPath(duk_get_string(ctx, 0));

    Poco::File p(inputPath);

    if (p.exists())
    {
        std::ifstream reader(p.path(), std::ios::binary);
        std::stringstream ss;;

        std::copy(
            std::istreambuf_iterator<char>(reader),
            std::istreambuf_iterator<char>(),
            std::ostreambuf_iterator<char>(ss));

        duk_push_string(ctx, ss.str().c_str());
        return 1;
    }

    return 0;
}

namespace JsEngine
{
    duk_ret_t dukopen_fs(duk_context* ctx)
    {
        duk_put_function_list(ctx, 0 /* export */, fs_funcs);
        return 0;
    }
}
