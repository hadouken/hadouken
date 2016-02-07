#include <hadouken/scripting/modules/file_system_module.hpp>

#include <boost/filesystem/detail/utf8_codecvt_facet.hpp>
#include <boost/filesystem.hpp>
#include <boost/filesystem/fstream.hpp>
#include <boost/log/trivial.hpp>
#include <boost/scoped_array.hpp>

#include <sstream>
#include <functional>
#include <memory>

#include "../duktape.h"

using namespace hadouken::scripting;
using namespace hadouken::scripting::modules;

namespace fs = boost::filesystem;

size_t read_impl(const fs::path& path, std::function<char*(size_t)> alloc)
{
    try
    {
        if (fs::exists(path))
        {
            boost::filesystem::ifstream file(path, std::ios::binary);

            file.seekg(0, std::ios_base::end);
            size_t size = file.tellg();
            char* buffer = alloc(size);

            file.seekg(0, std::ios_base::beg);
            file.read(buffer, size);

            return size;
        }
    }
    catch (boost::filesystem::filesystem_error& ex)
    {
        BOOST_LOG_TRIVIAL(error) << "Failed to read file: " << ex.what();

        return 0;
    }
}

size_t write_impl(const fs::path& path, const char* data, size_t size)
{
    try
    {
        boost::filesystem::ofstream file(path, std::ios::binary);

        file.write(data, size);

        return size;
    }
    catch (boost::filesystem::filesystem_error& ex)
    {
        BOOST_LOG_TRIVIAL(error) << "Failed to write file: " << ex.what();

        return 0;
    }
}

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
    try
    {
        boost::filesystem::detail::utf8_codecvt_facet facet;
        fs::path path(duk_require_string(ctx, 0), facet);

        auto deleter = [&ctx](char*){ duk_pop(ctx); }; // In case of failure, make sure any allocated buffer is popped off the stack before returning to duk
        std::unique_ptr<char, decltype(deleter)> buffer(nullptr, deleter);

        size_t size_read = read_impl(path, [&ctx, &buffer](size_t size)-> char*
        {
            void* buf = duk_push_buffer(ctx, size, false);
            buffer.reset(reinterpret_cast<char*>(buf));
            return buffer.get();
        });

        if (size_read > 0 && buffer)
        {
            buffer.release();
            return 1;
        }
    }
    catch (std::exception&)
    {
    }

    return 0;
}

duk_ret_t file_system_module::read_text(duk_context* ctx)
{
    boost::filesystem::detail::utf8_codecvt_facet facet;
    fs::path path(duk_require_string(ctx, 0), facet);

    try
    {
        boost::scoped_array<char> data;
        size_t size_read = read_impl(path, [&data](size_t size)-> char*
        {
            data.reset(new char[size + 1 /*+1 for null-terminator*/]);
            return data.get();
        });

        if (size_read > 0 && data)
        {
            data[size_read] = '\0'; // Data is not guaranteed to be null-terminated, so add an explicit null at the end
            duk_push_string(ctx, data.get());
            return 1;
        }
    }
    catch (std::exception&)
    {
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
    try
    {
        boost::filesystem::detail::utf8_codecvt_facet facet;
        fs::path path(duk_require_string(ctx, 0), facet);

        duk_size_t size;
        const char* buffer = static_cast<const char*>(duk_require_buffer(ctx, 1, &size));

        duk_size_t size_written = write_impl(path, buffer, size);

        if (size_written == size)
        {
            duk_push_number(ctx, size_written);
            return 1;
        }
    }
    catch (std::exception&)
    {
    }

    return 0;
}

duk_ret_t file_system_module::write_text(duk_context* ctx)
{
    try
    {
        boost::filesystem::detail::utf8_codecvt_facet facet;
        fs::path path(duk_require_string(ctx, 0), facet);

        duk_size_t size;
        const char* text = duk_require_lstring(ctx, 1, &size);

        duk_size_t size_written = write_impl(path, text, size);

        if (size_written == size)
        {
            duk_push_number(ctx, size_written);
            return 1;
        }
    }
    catch (std::exception&)
    {
    }

    return 0;
}
