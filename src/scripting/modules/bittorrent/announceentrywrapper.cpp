#include <Hadouken/Scripting/Modules/BitTorrent/AnnounceEntryWrapper.hpp>

#include <libtorrent/torrent_handle.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace Hadouken::Scripting::Modules;
using namespace Hadouken::Scripting::Modules::BitTorrent;

void AnnounceEntryWrapper::initialize(duk_context* ctx, libtorrent::announce_entry& entry)
{
    duk_idx_t entryIndex = duk_push_object(ctx);

    // Set internal pointer
    Common::setPointer<libtorrent::announce_entry>(ctx, entryIndex, new libtorrent::announce_entry(entry));

    DUK_READONLY_PROPERTY(ctx, entryIndex, isUpdating, getIsUpdating);
    DUK_READONLY_PROPERTY(ctx, entryIndex, isVerified, getIsVerified);
    DUK_READONLY_PROPERTY(ctx, entryIndex, message, getMessage);
    DUK_READONLY_PROPERTY(ctx, entryIndex, tier, getTier);
    DUK_READONLY_PROPERTY(ctx, entryIndex, url, getUrl);

    // Set finalizer
    duk_push_c_function(ctx, AnnounceEntryWrapper::finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t AnnounceEntryWrapper::finalize(duk_context* ctx)
{
    Common::finalize<libtorrent::announce_entry>(ctx);
    return 0;
}

duk_ret_t AnnounceEntryWrapper::getIsUpdating(duk_context* ctx)
{
    libtorrent::announce_entry* entry = Common::getPointer<libtorrent::announce_entry>(ctx);
    duk_push_boolean(ctx, entry->updating);
    return 1;
}

duk_ret_t AnnounceEntryWrapper::getIsVerified(duk_context* ctx)
{
    libtorrent::announce_entry* entry = Common::getPointer<libtorrent::announce_entry>(ctx);
    duk_push_boolean(ctx, entry->verified);
    return 1;
}

duk_ret_t AnnounceEntryWrapper::getMessage(duk_context* ctx)
{
    libtorrent::announce_entry* entry = Common::getPointer<libtorrent::announce_entry>(ctx);
    duk_push_string(ctx, entry->message.c_str());
    return 1;
}

duk_ret_t AnnounceEntryWrapper::getTier(duk_context* ctx)
{
    libtorrent::announce_entry* entry = Common::getPointer<libtorrent::announce_entry>(ctx);
    duk_push_int(ctx, entry->tier);
    return 1;
}

duk_ret_t AnnounceEntryWrapper::getUrl(duk_context* ctx)
{
    libtorrent::announce_entry* entry = Common::getPointer<libtorrent::announce_entry>(ctx);
    duk_push_string(ctx, entry->url.c_str());
    return 1;
}
