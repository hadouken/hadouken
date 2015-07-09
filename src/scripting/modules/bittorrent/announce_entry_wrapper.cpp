#include <hadouken/scripting/modules/bittorrent/announce_entry_wrapper.hpp>

#include <hadouken/scripting/modules/bittorrent/error_code_wrapper.hpp>
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

    DUK_READONLY_PROPERTY(ctx, entryIndex, failCount, get_fail_count);
    DUK_READONLY_PROPERTY(ctx, entryIndex, failLimit, get_fail_limit);
    DUK_READONLY_PROPERTY(ctx, entryIndex, isUpdating, is_updating);
    DUK_READONLY_PROPERTY(ctx, entryIndex, isVerified, is_verified);
    DUK_READONLY_PROPERTY(ctx, entryIndex, lastError, get_last_error);
    DUK_READONLY_PROPERTY(ctx, entryIndex, message, get_message);
    DUK_READONLY_PROPERTY(ctx, entryIndex, minAnnounce, get_min_announce);
    DUK_READONLY_PROPERTY(ctx, entryIndex, nextAnnounce, get_next_announce);
    DUK_READONLY_PROPERTY(ctx, entryIndex, scrapeComplete, get_scrape_complete);
    DUK_READONLY_PROPERTY(ctx, entryIndex, scrapeDownloaded, get_scrape_downloaded);
    DUK_READONLY_PROPERTY(ctx, entryIndex, scrapeIncomplete, get_scrape_incomplete);
    DUK_READONLY_PROPERTY(ctx, entryIndex, source, get_source);
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

duk_ret_t announce_entry_wrapper::get_fail_count(duk_context* ctx)
{
    libtorrent::announce_entry* entry = common::get_pointer<libtorrent::announce_entry>(ctx);
    duk_push_int(ctx, entry->fails);
    return 1;
}

duk_ret_t announce_entry_wrapper::get_fail_limit(duk_context* ctx)
{
    libtorrent::announce_entry* entry = common::get_pointer<libtorrent::announce_entry>(ctx);
    duk_push_int(ctx, entry->fail_limit);
    return 1;
}

duk_ret_t announce_entry_wrapper::get_last_error(duk_context* ctx)
{
    libtorrent::announce_entry* entry = common::get_pointer<libtorrent::announce_entry>(ctx);
    
    if (entry->last_error)
    {
        error_code_wrapper::initialize(ctx, entry->last_error);
    }
    else
    {
        duk_push_null(ctx);
    }

    return 1;
}

duk_ret_t announce_entry_wrapper::get_message(duk_context* ctx)
{
    libtorrent::announce_entry* entry = common::get_pointer<libtorrent::announce_entry>(ctx);
    duk_push_string(ctx, entry->message.c_str());
    return 1;
}

duk_ret_t announce_entry_wrapper::get_min_announce(duk_context* ctx)
{
    libtorrent::announce_entry* entry = common::get_pointer<libtorrent::announce_entry>(ctx);
    duk_push_int(ctx, entry->min_announce_in());
    return 1;
}

duk_ret_t announce_entry_wrapper::get_next_announce(duk_context* ctx)
{
    libtorrent::announce_entry* entry = common::get_pointer<libtorrent::announce_entry>(ctx);
    duk_push_int(ctx, entry->next_announce_in());
    return 1;
}

duk_ret_t announce_entry_wrapper::get_scrape_complete(duk_context* ctx)
{
    libtorrent::announce_entry* entry = common::get_pointer<libtorrent::announce_entry>(ctx);
    duk_push_int(ctx, entry->scrape_complete);
    return 1;
}

duk_ret_t announce_entry_wrapper::get_scrape_downloaded(duk_context* ctx)
{
    libtorrent::announce_entry* entry = common::get_pointer<libtorrent::announce_entry>(ctx);
    duk_push_int(ctx, entry->scrape_downloaded);
    return 1;
}

duk_ret_t announce_entry_wrapper::get_scrape_incomplete(duk_context* ctx)
{
    libtorrent::announce_entry* entry = common::get_pointer<libtorrent::announce_entry>(ctx);
    duk_push_int(ctx, entry->scrape_incomplete);
    return 1;
}

duk_ret_t announce_entry_wrapper::get_source(duk_context* ctx)
{
    libtorrent::announce_entry* entry = common::get_pointer<libtorrent::announce_entry>(ctx);
    duk_push_int(ctx, entry->source);
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
