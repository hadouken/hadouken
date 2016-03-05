#include <hadouken/scripting/modules/bittorrent/torrent_info_wrapper.hpp>
#include <boost/log/trivial.hpp>

#include <libtorrent/torrent_info.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;

duk_ret_t torrent_info_wrapper::construct(duk_context* ctx)
{
    int t = duk_get_type(ctx, 0);
    libtorrent::torrent_info* info;

    try
    {
        if (t == DUK_TYPE_STRING)
        {
            std::string file(duk_require_string(ctx, 0));
            info = new libtorrent::torrent_info(file);
        }
        else if (t == DUK_TYPE_BUFFER)
        {
            duk_size_t size;
            const char* buffer = static_cast<const char*>(duk_require_buffer(ctx, 0, &size));
            info = new libtorrent::torrent_info(buffer, size);
        }

        duk_push_this(ctx);
        common::set_pointer<libtorrent::torrent_info>(ctx, -2, info);

        duk_push_c_function(ctx, finalize, 1);
        duk_set_finalizer(ctx, -2);
    }
    catch (const libtorrent::libtorrent_exception& ex)
    {
        BOOST_LOG_TRIVIAL(warning) << "Invalid torrent file: " << ex.what();
        return DUK_RET_UNSUPPORTED_ERROR;
    }
    catch (const std::exception& ex)
    {
        BOOST_LOG_TRIVIAL(warning) << "Error adding torrent: " << ex.what();
        return DUK_RET_ERROR;
    }

    return 0;
}

void torrent_info_wrapper::initialize(duk_context* ctx, const libtorrent::torrent_info& info)
{
    duk_function_list_entry functions[] =
    {
        { "getFiles", get_files, 0 },
        { NULL, NULL, 0 }
    };

    duk_idx_t infoIndex = duk_push_object(ctx);
    duk_put_function_list(ctx, infoIndex, functions);

    // Set internal pointers
    common::set_pointer<libtorrent::torrent_info>(ctx, infoIndex, new libtorrent::torrent_info(info));

    DUK_READONLY_PROPERTY(ctx, infoIndex, comment, get_comment);
    DUK_READONLY_PROPERTY(ctx, infoIndex, creationDate, get_creation_date);
    DUK_READONLY_PROPERTY(ctx, infoIndex, creator, get_creator);
    DUK_READONLY_PROPERTY(ctx, infoIndex, infoHash, get_info_hash);
    DUK_READONLY_PROPERTY(ctx, infoIndex, name, get_name);
    DUK_READONLY_PROPERTY(ctx, infoIndex, totalSize, get_total_size);

    duk_push_c_function(ctx, finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t torrent_info_wrapper::finalize(duk_context* ctx)
{
    common::finalize<libtorrent::torrent_info>(ctx);
    return 0;
}

duk_ret_t torrent_info_wrapper::get_creation_date(duk_context* ctx)
{
    libtorrent::torrent_info* info = common::get_pointer<libtorrent::torrent_info>(ctx);

    if (info->creation_date())
    {
        duk_push_number(ctx, info->creation_date().get());
    }
    else
    {
        duk_push_number(ctx, -1);
    }

    return 1;
}

duk_ret_t torrent_info_wrapper::get_comment(duk_context* ctx)
{
    libtorrent::torrent_info* info = common::get_pointer<libtorrent::torrent_info>(ctx);
    duk_push_string(ctx, info->comment().c_str());
    return 1;
}

duk_ret_t torrent_info_wrapper::get_creator(duk_context* ctx)
{
    libtorrent::torrent_info* info = common::get_pointer<libtorrent::torrent_info>(ctx);
    duk_push_string(ctx, info->creator().c_str());
    return 1;
}

duk_ret_t torrent_info_wrapper::get_files(duk_context* ctx)
{
    libtorrent::torrent_info* info = common::get_pointer<libtorrent::torrent_info>(ctx);

    int arrayIndex = duk_push_array(ctx);
    int i = 0;

    const libtorrent::file_storage& storage = info->files();

    for (int i = 0; i < storage.num_files(); i++)
    {
        libtorrent::file_entry entry = storage.at(i);

        duk_idx_t entryIndex = duk_push_object(ctx);
        duk_push_string(ctx, entry.path.c_str());
        duk_put_prop_string(ctx, entryIndex, "path");

        duk_push_number(ctx, static_cast<duk_double_t>(entry.size));
        duk_put_prop_string(ctx, entryIndex, "size");

        // Put entry object
        duk_put_prop_index(ctx, arrayIndex, i);
    }

    return 1;
}

duk_ret_t torrent_info_wrapper::get_info_hash(duk_context* ctx)
{
    libtorrent::torrent_info* info = common::get_pointer<libtorrent::torrent_info>(ctx);
    duk_push_string(ctx, libtorrent::to_hex(info->info_hash().to_string()).c_str());
    return 1;
}

duk_ret_t torrent_info_wrapper::get_name(duk_context* ctx)
{
    libtorrent::torrent_info* info = common::get_pointer<libtorrent::torrent_info>(ctx);
    duk_push_string(ctx, info->name().c_str());
    return 1;
}

duk_ret_t torrent_info_wrapper::get_total_size(duk_context* ctx)
{
    libtorrent::torrent_info* info = common::get_pointer<libtorrent::torrent_info>(ctx);
    duk_push_number(ctx, static_cast<duk_double_t>(info->total_size()));
    return 1;
}
