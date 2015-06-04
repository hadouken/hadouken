#include <hadouken/scripting/modules/bittorrent/announce_entry_wrapper.hpp>

#include <libtorrent/torrent_handle.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;

void announce_entry_wrapper::initialize(duk_context* ctx, libtorrent::announce_entry& entry)
{
    duk_idx_t entryIndex = duk_push_object(ctx);

    // Set internal pointer
    common::set_pointer<libtorrent::announce_entry>(ctx, entryIndex, new libtorrent::announce_entry(entry));

    DUK_READONLY_PROPERTY(ctx, entryIndex, isUpdating, is_updating);
    DUK_READONLY_PROPERTY(ctx, entryIndex, isVerified, is_verified);
    DUK_READONLY_PROPERTY(ctx, entryIndex, message, get_message);
    DUK_READONLY_PROPERTY(ctx, entryIndex, tier, get_tier);
    DUK_READONLY_PROPERTY(ctx, entryIndex, url, get_url);

    // Set finalizer
    duk_push_c_function(ctx, announce_entry_wrapper::finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t announce_entry_wrapper::finalize(duk_context* ctx)
{
    common::finalize<libtorrent::announce_entry>(ctx);
    return 0;
}

duk_ret_t announce_entry_wrapper::is_updating(duk_context* ctx)
{
    libtorrent::announce_entry* entry = common::get_pointer<libtorrent::announce_entry>(ctx);
    duk_push_boolean(ctx, entry->updating);
    return 1;
}

duk_ret_t announce_entry_wrapper::is_verified(duk_context* ctx)
{
    libtorrent::announce_entry* entry = common::get_pointer<libtorrent::announce_entry>(ctx);
    duk_push_boolean(ctx, entry->verified);
    return 1;
}

duk_ret_t announce_entry_wrapper::get_message(duk_context* ctx)
{
    libtorrent::announce_entry* entry = common::get_pointer<libtorrent::announce_entry>(ctx);
    duk_push_string(ctx, entry->message.c_str());
    return 1;
}

duk_ret_t announce_entry_wrapper::get_tier(duk_context* ctx)
{
    libtorrent::announce_entry* entry = common::get_pointer<libtorrent::announce_entry>(ctx);
    duk_push_int(ctx, entry->tier);
    return 1;
}

duk_ret_t announce_entry_wrapper::get_url(duk_context* ctx)
{
    libtorrent::announce_entry* entry = common::get_pointer<libtorrent::announce_entry>(ctx);
    duk_push_string(ctx, entry->url.c_str());
    return 1;
}
