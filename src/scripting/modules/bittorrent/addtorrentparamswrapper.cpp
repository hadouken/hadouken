#include <Hadouken/Scripting/Modules/BitTorrent/AddTorrentParamsWrapper.hpp>

#include <libtorrent/add_torrent_params.hpp>
#include <libtorrent/session.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/TorrentInfoWrapper.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace Hadouken::Scripting::Modules;
using namespace Hadouken::Scripting::Modules::BitTorrent;

duk_ret_t AddTorrentParamsWrapper::construct(duk_context* ctx)
{
    duk_push_this(ctx);

    Common::setPointer<libtorrent::add_torrent_params>(ctx, -2, new libtorrent::add_torrent_params());

    DUK_READWRITE_PROPERTY(ctx, -4, flags, Flags);
    DUK_READWRITE_PROPERTY(ctx, -4, savePath, SavePath);
    DUK_READWRITE_PROPERTY(ctx, -4, sparseMode, SparseMode);
    DUK_READWRITE_PROPERTY(ctx, -4, torrent, Torrent);
    DUK_READWRITE_PROPERTY(ctx, -4, url, Url);

    duk_push_c_function(ctx, destruct, 1);
    duk_set_finalizer(ctx, -2);

    return 0;
}

duk_ret_t AddTorrentParamsWrapper::destruct(duk_context* ctx)
{
    Common::finalize<libtorrent::add_torrent_params>(ctx);
    return 0;
}

duk_ret_t AddTorrentParamsWrapper::getFlags(duk_context* ctx)
{
    uint64_t flags = Common::getPointer<libtorrent::add_torrent_params>(ctx)->flags;
    duk_push_number(ctx, flags);
    return 1;
}

duk_ret_t AddTorrentParamsWrapper::getResumeData(duk_context* ctx)
{
    libtorrent::add_torrent_params* p = Common::getPointer<libtorrent::add_torrent_params>(ctx);
    
    if (p->resume_data.empty())
    {
        duk_push_undefined(ctx);
        return 1;
    }

    void* buffer = duk_push_buffer(ctx, p->resume_data.size(), false);
    std::copy(p->resume_data.begin(), p->resume_data.end(), static_cast<char*>(buffer));

    return 1;
}

duk_ret_t AddTorrentParamsWrapper::getSavePath(duk_context* ctx)
{
    std::string savePath = Common::getPointer<libtorrent::add_torrent_params>(ctx)->save_path;
    duk_push_string(ctx, savePath.c_str());
    return 1;
}

duk_ret_t AddTorrentParamsWrapper::getSparseMode(duk_context* ctx)
{
    libtorrent::add_torrent_params* p = Common::getPointer<libtorrent::add_torrent_params>(ctx);
    bool sparse = (p->storage_mode == libtorrent::storage_mode_t::storage_mode_sparse);

    duk_push_boolean(ctx, sparse);
    return 1;
}

duk_ret_t AddTorrentParamsWrapper::getTorrent(duk_context* ctx)
{
    libtorrent::add_torrent_params* p = Common::getPointer<libtorrent::add_torrent_params>(ctx);

    if (!p->ti)
    {
        duk_push_null(ctx);
        return 1;
    }

    TorrentInfoWrapper::initialize(ctx, *p->ti);
    return 1;
}

duk_ret_t AddTorrentParamsWrapper::getUrl(duk_context* ctx)
{
    libtorrent::add_torrent_params* p = Common::getPointer<libtorrent::add_torrent_params>(ctx);
    duk_push_string(ctx, p->url.c_str());
    return 1;
}

duk_ret_t AddTorrentParamsWrapper::setFlags(duk_context* ctx)
{
    uint64_t flags(duk_require_number(ctx, 0));
    Common::getPointer<libtorrent::add_torrent_params>(ctx)->flags = flags;
    return 0;
}

duk_ret_t AddTorrentParamsWrapper::setResumeData(duk_context* ctx)
{
    duk_size_t size;
    const char* buffer = static_cast<const char*>(duk_require_buffer(ctx, 0, &size));
    
    libtorrent::add_torrent_params* p = Common::getPointer<libtorrent::add_torrent_params>(ctx);
    p->resume_data = std::vector<char>(buffer, buffer + size);

    return 0;
}

duk_ret_t AddTorrentParamsWrapper::setSavePath(duk_context* ctx)
{
    std::string savePath(duk_require_string(ctx, 0));
    Common::getPointer<libtorrent::add_torrent_params>(ctx)->save_path = savePath;
    return 0;
}

duk_ret_t AddTorrentParamsWrapper::setSparseMode(duk_context* ctx)
{
    bool sparse = duk_require_boolean(ctx, 0);
    libtorrent::add_torrent_params* p = Common::getPointer<libtorrent::add_torrent_params>(ctx);

    if (sparse)
    {
        p->storage_mode = libtorrent::storage_mode_t::storage_mode_sparse;
    }
    else
    {
        p->storage_mode = libtorrent::storage_mode_t::storage_mode_allocate;
    }

    return 0;
}

duk_ret_t AddTorrentParamsWrapper::setTorrent(duk_context* ctx)
{
    libtorrent::add_torrent_params* p = Common::getPointer<libtorrent::add_torrent_params>(ctx);
    libtorrent::torrent_info* info = Common::getPointer<libtorrent::torrent_info>(ctx, 0);

    p->ti = new libtorrent::torrent_info(*info);
    return 0;
}

duk_ret_t AddTorrentParamsWrapper::setUrl(duk_context* ctx)
{
    libtorrent::add_torrent_params* p = Common::getPointer<libtorrent::add_torrent_params>(ctx);
    p->url = std::string(duk_require_string(ctx, 0));
    return 0;
}
