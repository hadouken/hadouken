#include <Hadouken/Scripting/Modules/BitTorrent/TorrentInfoWrapper.hpp>

#include <libtorrent/torrent_info.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace Hadouken::Scripting::Modules;
using namespace Hadouken::Scripting::Modules::BitTorrent;

duk_ret_t TorrentInfoWrapper::construct(duk_context* ctx)
{
    int t = duk_get_type(ctx, 0);
    libtorrent::torrent_info* info;

    if (t == DUK_TYPE_STRING)
    {
        std::string file(duk_require_string(ctx, 0));
        // TODO: error handling
        info = new libtorrent::torrent_info(file);
    }
    else if (t == DUK_TYPE_BUFFER)
    {
        duk_size_t size;
        const char* buffer = static_cast<const char*>(duk_require_buffer(ctx, 0, &size));
        // TODO: error handling
        info = new libtorrent::torrent_info(buffer, size);
    }

    duk_push_this(ctx);
    Common::setPointer<libtorrent::torrent_info>(ctx, -2, info);

    duk_push_c_function(ctx, finalize, 1);
    duk_set_finalizer(ctx, -2);

    return 0;
}

void TorrentInfoWrapper::initialize(duk_context* ctx, const libtorrent::torrent_info& info)
{
    duk_function_list_entry functions[] =
    {
        { "getFiles", getFiles, 0 },
        { NULL, NULL, 0 }
    };

    duk_idx_t infoIndex = duk_push_object(ctx);
    duk_put_function_list(ctx, infoIndex, functions);

    // Set internal pointers
    Common::setPointer<libtorrent::torrent_info>(ctx, infoIndex, new libtorrent::torrent_info(info));

    DUK_READONLY_PROPERTY(ctx, infoIndex, infoHash, getInfoHash);
    DUK_READONLY_PROPERTY(ctx, infoIndex, name, getName);
    DUK_READONLY_PROPERTY(ctx, infoIndex, totalSize, getTotalSize);

    duk_push_c_function(ctx, TorrentInfoWrapper::finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t TorrentInfoWrapper::finalize(duk_context* ctx)
{
    Common::finalize<libtorrent::torrent_info>(ctx);
    return 0;
}

duk_ret_t TorrentInfoWrapper::getFiles(duk_context* ctx)
{
    libtorrent::torrent_info* info = Common::getPointer<libtorrent::torrent_info>(ctx);

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

duk_ret_t TorrentInfoWrapper::getInfoHash(duk_context* ctx)
{
    libtorrent::torrent_info* info = Common::getPointer<libtorrent::torrent_info>(ctx);
    duk_push_string(ctx, libtorrent::to_hex(info->info_hash().to_string()).c_str());
    return 1;
}

duk_ret_t TorrentInfoWrapper::getName(duk_context* ctx)
{
    libtorrent::torrent_info* info = Common::getPointer<libtorrent::torrent_info>(ctx);
    duk_push_string(ctx, info->name().c_str());
    return 1;
}

duk_ret_t TorrentInfoWrapper::getTotalSize(duk_context* ctx)
{
    libtorrent::torrent_info* info = Common::getPointer<libtorrent::torrent_info>(ctx);
    duk_push_number(ctx, static_cast<duk_double_t>(info->total_size()));
    return 1;
}
