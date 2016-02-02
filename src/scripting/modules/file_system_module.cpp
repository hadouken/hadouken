#include <hadouken/scripting/modules/file_system_module.hpp>

#include <boost/filesystem/detail/utf8_codecvt_facet.hpp>
#include <boost/filesystem.hpp>
#include <boost/filesystem/fstream.hpp>
#include <boost/log/trivial.hpp>

#include <sstream>

#include "../duktape.h"

using namespace hadouken::scripting;
using namespace hadouken::scripting::modules;

namespace fs = boost::filesystem;

duk_ret_t file_system_module::initialize(duk_context* ctx)
{
    duk_function_list_entry functions[] =
    {
        { "combine",           combine,            DUK_VARARGS },
        { "createDirectories", create_directories, 1 },
        { "deleteFile",        delete_file,        1 },
        { "directoryExists",   directory_exists,   1 },
        { "fileExists",        file_exists,        1 },
        { "getFiles",          get_files,          1 },
        { "isRelative",        is_relative,        1 },
        { "makeAbsolute",      make_absolute,      1 },
        { "readBuffer",        read_buffer,        1 },
        { "readText",          read_text,          1 },
        { "rename",            rename,             2 },
        { "space",             space,              1 },
        { "writeBuffer",       write_buffer,       2 },
        { "writeText",         write_text,         2 },
        { NULL, NULL, 0 }
    };

    duk_put_function_list(ctx, 0 /* exports */, functions);
    return 0;
}

duk_ret_t file_system_module::combine(duk_context* ctx)
{
    boost::filesystem::detail::utf8_codecvt_facet facet;
    int args = duk_get_top(ctx);

    fs::path path(duk_require_string(ctx, 0), facet);
    
    for (int i = 1; i < duk_get_top(ctx); i++)
    {
        path /= duk_get_string(ctx, i);
    }

    duk_push_string(ctx, path.normalize().string(facet).c_str());
    return 1;
}

duk_ret_t file_system_module::create_directories(duk_context* ctx)
{
    boost::filesystem::detail::utf8_codecvt_facet facet;
    fs::path path(duk_require_string(ctx, 0), facet);
    fs::create_directories(path);
    return 0;
}

duk_ret_t file_system_module::delete_file(duk_context* ctx)
{
    boost::filesystem::detail::utf8_codecvt_facet facet;
    fs::path path(duk_require_string(ctx, 0), facet);
    
    if (!fs::exists(path))
    {
        duk_push_error_object(ctx, DUK_ERR_ERROR, "File not found: %s", path.string(facet).c_str());
        return 1;
    }

    if (!fs::is_regular_file(path))
    {
        duk_push_error_object(ctx, DUK_ERR_ERROR, "Path is not a file: %s", path.string(facet).c_str());
        return 1;
    }

    duk_push_boolean(ctx, fs::remove(path));
    return 1;
}

duk_ret_t file_system_module::directory_exists(duk_context* ctx)
{
    boost::filesystem::detail::utf8_codecvt_facet facet;
    fs::path path(duk_require_string(ctx, 0), facet);
    duk_push_boolean(ctx, (fs::exists(path) && fs::is_directory(path)));
    return 1;
}

duk_ret_t file_system_module::file_exists(duk_context* ctx)
{
    boost::filesystem::detail::utf8_codecvt_facet facet;
    fs::path path(duk_require_string(ctx, 0), facet);
    duk_push_boolean(ctx, (fs::exists(path) && fs::is_regular_file(path)));
    return 1;
}

duk_ret_t file_system_module::get_files(duk_context* ctx)
{
    boost::filesystem::detail::utf8_codecvt_facet facet;
    fs::path path(duk_require_string(ctx, 0), facet);

    if (!fs::exists(path))
    {
        return 0;
    }

    int arrayIndex = duk_push_array(ctx);
    int i = 0;

    for (fs::path p : fs::directory_iterator(path))
    {
        duk_push_string(ctx, p.string(facet).c_str());
        duk_put_prop_index(ctx, arrayIndex, i);

        ++i;
    }

    return 1;
}

duk_ret_t file_system_module::is_relative(duk_context* ctx)
{
    boost::filesystem::detail::utf8_codecvt_facet facet;
    fs::path path(duk_require_string(ctx, 0), facet);
    duk_push_boolean(ctx, path.is_relative());
    return 1;
}

duk_ret_t file_system_module::make_absolute(duk_context* ctx)
{
    boost::filesystem::detail::utf8_codecvt_facet facet;
    fs::path path(duk_require_string(ctx, 0), facet);
    duk_push_string(ctx, fs::absolute(path).string(facet).c_str());
    return 1;
}

duk_ret_t file_system_module::rename(duk_context* ctx)
{
    boost::filesystem::detail::utf8_codecvt_facet facet;
    fs::path prev(duk_require_string(ctx, 0), facet);
    fs::path next(duk_require_string(ctx, 1), facet);

    fs::rename(prev, next);

    return 0;
}

duk_ret_t file_system_module::read_buffer(duk_context* ctx)
{
    void* buffer = nullptr;

    try
    {
        boost::filesystem::detail::utf8_codecvt_facet facet;
        fs::path path(duk_require_string(ctx, 0), facet);

        if (fs::exists(path))
        {
            boost::filesystem::ifstream file(path, std::ios::binary);

            file.seekg(0, std::ios_base::end);
            size_t size = file.tellg();
            buffer = duk_push_buffer(ctx, size, false);

            file.seekg(0, std::ios_base::beg);
            file.read(reinterpret_cast<char*>(buffer), size);

            return 1;
        }

        return 0;
    }
    catch (boost::filesystem::filesystem_error& ex)
    {
        BOOST_LOG_TRIVIAL(error) << "Failed to read file: " << ex.what();

        if (buffer)
        {
            duk_pop(ctx);
        }

        return 0;
    }
}

duk_ret_t file_system_module::read_text(duk_context* ctx)
{
    boost::filesystem::detail::utf8_codecvt_facet facet;
    fs::path path(duk_require_string(ctx, 0), facet);

    if (fs::exists(path))
    {
        std::ifstream reader(path.string(), std::ios::binary);
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

duk_ret_t file_system_module::space(duk_context* ctx)
{
    boost::filesystem::detail::utf8_codecvt_facet facet;
    fs::path path(duk_require_string(ctx, 0), facet);
    fs::space_info info = fs::space(path);

    duk_idx_t idx = duk_push_object(ctx);
    duk_push_number(ctx, info.available);
    duk_put_prop_string(ctx, idx, "available");

    duk_push_number(ctx, info.capacity);
    duk_put_prop_string(ctx, idx, "capacity");

    duk_push_number(ctx, info.free);
    duk_put_prop_string(ctx, idx, "free");

    return 1;
}

duk_ret_t file_system_module::write_buffer(duk_context* ctx)
{
    void* buffer = nullptr;

    try
    {
        boost::filesystem::detail::utf8_codecvt_facet facet;
        fs::path path(duk_require_string(ctx, 0), facet);

        duk_size_t size;
        const char* buffer = static_cast<const char*>(duk_require_buffer(ctx, 1, &size));

        boost::filesystem::ofstream file(path, std::ios::binary);

        file.write(buffer, size);
        duk_push_number(ctx, size);

        return 1;
    }
    catch (boost::filesystem::filesystem_error& ex)
    {
        BOOST_LOG_TRIVIAL(error) << "Failed to read file: " << ex.what();

        if (buffer)
        {
            duk_pop(ctx);
        }

        return 0;
    }
}

duk_ret_t file_system_module::write_text(duk_context* ctx)
{
    boost::filesystem::detail::utf8_codecvt_facet facet;
    fs::path path(duk_require_string(ctx, 0), facet);

    duk_size_t size;
    const char* text = duk_require_lstring(ctx, 1, &size);

    std::FILE *fp = std::fopen(path.string().c_str(), "wb");

    if (fp)
    {
        size_t written = std::fwrite(text, sizeof(char), size, fp);
        std::fclose(fp);

        duk_push_number(ctx, written);
        return 1;
    }

    return 0;
}
