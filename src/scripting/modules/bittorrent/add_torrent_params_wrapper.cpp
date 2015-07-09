#include <hadouken/scripting/modules/bittorrent/add_torrent_params_wrapper.hpp>

#include <libtorrent/add_torrent_params.hpp>
#include <libtorrent/session.hpp>
#include <hadouken/scripting/modules/bittorrent/torrent_info_wrapper.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;

duk_ret_t add_torrent_params_wrapper::construct(duk_context* ctx)
{
    duk_push_this(ctx);

    common::set_pointer<libtorrent::add_torrent_params>(ctx, -2, new libtorrent::add_torrent_params());

    DUK_READWRITE_PROPERTY(ctx, -4, metadata, data);
    DUK_READWRITE_PROPERTY(ctx, -4, filePriorities, file_priorities);
    DUK_READWRITE_PROPERTY(ctx, -4, flags, flags);
    DUK_READWRITE_PROPERTY(ctx, -4, resumeData, resume_data);
    DUK_READWRITE_PROPERTY(ctx, -4, savePath, save_path);
    DUK_READWRITE_PROPERTY(ctx, -4, sparseMode, sparse_mode);
    DUK_READWRITE_PROPERTY(ctx, -4, torrent, torrent);
    DUK_READWRITE_PROPERTY(ctx, -4, trackers, trackers);
    DUK_READWRITE_PROPERTY(ctx, -4, url, url);

    duk_push_c_function(ctx, destruct, 1);
    duk_set_finalizer(ctx, -2);

    return 0;
}

void add_torrent_params_wrapper::initialize(void* ctx, const libtorrent::add_torrent_params& params)
{
    duk_idx_t idx = duk_push_object(ctx);

    common::set_pointer<libtorrent::add_torrent_params>(ctx, idx, new libtorrent::add_torrent_params(params));

    DUK_READWRITE_PROPERTY(ctx, idx, metadata, data);
    DUK_READWRITE_PROPERTY(ctx, idx, filePriorities, file_priorities);
    DUK_READWRITE_PROPERTY(ctx, idx, flags, flags);
    DUK_READWRITE_PROPERTY(ctx, idx, resumeData, resume_data);
    DUK_READWRITE_PROPERTY(ctx, idx, savePath, save_path);
    DUK_READWRITE_PROPERTY(ctx, idx, sparseMode, sparse_mode);
    DUK_READWRITE_PROPERTY(ctx, idx, torrent, torrent);
    DUK_READWRITE_PROPERTY(ctx, idx, trackers, trackers);
    DUK_READWRITE_PROPERTY(ctx, idx, url, url);

    duk_push_c_function(ctx, destruct, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t add_torrent_params_wrapper::destruct(duk_context* ctx)
{
    common::finalize<libtorrent::add_torrent_params>(ctx);
    return 0;
}

duk_ret_t add_torrent_params_wrapper::get_data(void* ctx)
{
    libtorrent::add_torrent_params* p = common::get_pointer<libtorrent::add_torrent_params>(ctx);

    if (!p->userdata)
    {
        duk_push_null(ctx);
    }
    else
    {
        std::string* dec = static_cast<std::string*>(p->userdata);

        duk_push_string(ctx, dec->c_str());
        duk_json_decode(ctx, -1);
    }

    return 1;
}

duk_ret_t add_torrent_params_wrapper::get_file_priorities(duk_context* ctx)
{
    libtorrent::add_torrent_params* p = common::get_pointer<libtorrent::add_torrent_params>(ctx);
    
    duk_idx_t arrIdx = duk_push_array(ctx);
    int i = 0;

    for (int prio : p->file_priorities)
    {
        duk_push_int(ctx, prio);
        duk_put_prop_index(ctx, arrIdx, i);

        ++i;
    }

    return 1;
}

duk_ret_t add_torrent_params_wrapper::get_flags(duk_context* ctx)
{
    uint64_t flags = common::get_pointer<libtorrent::add_torrent_params>(ctx)->flags;
    duk_push_number(ctx, flags);
    return 1;
}

duk_ret_t add_torrent_params_wrapper::get_resume_data(duk_context* ctx)
{
    libtorrent::add_torrent_params* p = common::get_pointer<libtorrent::add_torrent_params>(ctx);
    
    if (p->resume_data.empty())
    {
        duk_push_undefined(ctx);
        return 1;
    }

    void* buffer = duk_push_buffer(ctx, p->resume_data.size(), false);
    std::copy(p->resume_data.begin(), p->resume_data.end(), static_cast<char*>(buffer));

    return 1;
}

duk_ret_t add_torrent_params_wrapper::get_save_path(duk_context* ctx)
{
    std::string path = common::get_pointer<libtorrent::add_torrent_params>(ctx)->save_path;
    duk_push_string(ctx, path.c_str());
    return 1;
}

duk_ret_t add_torrent_params_wrapper::get_sparse_mode(duk_context* ctx)
{
    libtorrent::add_torrent_params* p = common::get_pointer<libtorrent::add_torrent_params>(ctx);
    bool sparse = (p->storage_mode == libtorrent::storage_mode_t::storage_mode_sparse);

    duk_push_boolean(ctx, sparse);
    return 1;
}

duk_ret_t add_torrent_params_wrapper::get_torrent(duk_context* ctx)
{
    libtorrent::add_torrent_params* p = common::get_pointer<libtorrent::add_torrent_params>(ctx);

    if (!p->ti)
    {
        duk_push_null(ctx);
        return 1;
    }

    torrent_info_wrapper::initialize(ctx, *p->ti);
    return 1;
}

duk_ret_t add_torrent_params_wrapper::get_trackers(duk_context* ctx)
{
    libtorrent::add_torrent_params* p = common::get_pointer<libtorrent::add_torrent_params>(ctx);

    duk_idx_t arrIdx = duk_push_array(ctx);
    int i = 0;

    for (std::string tracker : p->trackers)
    {
        duk_push_string(ctx, tracker.c_str());
        duk_put_prop_index(ctx, arrIdx, i);

        ++i;
    }

    return 1;
}

duk_ret_t add_torrent_params_wrapper::get_url(duk_context* ctx)
{
    libtorrent::add_torrent_params* p = common::get_pointer<libtorrent::add_torrent_params>(ctx);
    duk_push_string(ctx, p->url.c_str());
    return 1;
}

duk_ret_t add_torrent_params_wrapper::set_data(duk_context* ctx)
{
    libtorrent::add_torrent_params* p = common::get_pointer<libtorrent::add_torrent_params>(ctx);
    
    duk_json_encode(ctx, 0);
    p->userdata = new std::string(duk_require_string(ctx, 0));

    return 0;
}

duk_ret_t add_torrent_params_wrapper::set_file_priorities(duk_context* ctx)
{
    if (!duk_is_array(ctx, 0))
    {
        // TODO break something
        return 0;
    }

    libtorrent::add_torrent_params* p = common::get_pointer<libtorrent::add_torrent_params>(ctx);
    duk_enum(ctx, 0, DUK_ENUM_ARRAY_INDICES_ONLY);

    while (duk_next(ctx, -1, 1))
    {
        p->file_priorities.push_back(duk_get_int(ctx, -1));
        duk_pop_2(ctx);
    }

    duk_pop(ctx);
    return 0;
}

duk_ret_t add_torrent_params_wrapper::set_flags(duk_context* ctx)
{
    uint64_t flags(duk_require_number(ctx, 0));
    common::get_pointer<libtorrent::add_torrent_params>(ctx)->flags = flags;
    return 0;
}

duk_ret_t add_torrent_params_wrapper::set_resume_data(duk_context* ctx)
{
    duk_size_t size;
    const char* buffer = static_cast<const char*>(duk_require_buffer(ctx, 0, &size));
    
    libtorrent::add_torrent_params* p = common::get_pointer<libtorrent::add_torrent_params>(ctx);
    p->resume_data = std::vector<char>(buffer, buffer + size);

    return 0;
}

duk_ret_t add_torrent_params_wrapper::set_save_path(duk_context* ctx)
{
    std::string path(duk_require_string(ctx, 0));
    common::get_pointer<libtorrent::add_torrent_params>(ctx)->save_path = path;
    return 0;
}

duk_ret_t add_torrent_params_wrapper::set_sparse_mode(duk_context* ctx)
{
    bool sparse = duk_require_boolean(ctx, 0);
    libtorrent::add_torrent_params* p = common::get_pointer<libtorrent::add_torrent_params>(ctx);

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

duk_ret_t add_torrent_params_wrapper::set_torrent(duk_context* ctx)
{
    libtorrent::add_torrent_params* p = common::get_pointer<libtorrent::add_torrent_params>(ctx);
    libtorrent::torrent_info* info = common::get_pointer<libtorrent::torrent_info>(ctx, 0);

    p->ti = new libtorrent::torrent_info(*info);
    return 0;
}

duk_ret_t add_torrent_params_wrapper::set_trackers(duk_context* ctx)
{
    if (!duk_is_array(ctx, 0))
    {
        // TODO break something
        return 0;
    }

    libtorrent::add_torrent_params* p = common::get_pointer<libtorrent::add_torrent_params>(ctx);
    duk_enum(ctx, 0, DUK_ENUM_ARRAY_INDICES_ONLY);

    while (duk_next(ctx, -1, 1))
    {
        p->trackers.push_back(duk_get_string(ctx, -1));
        duk_pop_2(ctx);
    }

    duk_pop(ctx);
    return 0;
}

duk_ret_t add_torrent_params_wrapper::set_url(duk_context* ctx)
{
    libtorrent::add_torrent_params* p = common::get_pointer<libtorrent::add_torrent_params>(ctx);
    p->url = std::string(duk_require_string(ctx, 0));
    return 0;
}
