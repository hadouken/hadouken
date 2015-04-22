#include <Hadouken/Scripting/Modules/BitTorrent/AnnounceEntryWrapper.hpp>

#include <Hadouken/BitTorrent/AnnounceEntry.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules;
using namespace Hadouken::Scripting::Modules::BitTorrent;

void AnnounceEntryWrapper::initialize(duk_context* ctx, AnnounceEntry& announceEntry)
{
    duk_idx_t entryIndex = duk_push_object(ctx);

    // Set internal pointer
    Common::setPointer<AnnounceEntry>(ctx, entryIndex, new AnnounceEntry(announceEntry));

    DUK_READONLY_PROPERTY(ctx, entryIndex, message, AnnounceEntryWrapper::getMessage);
    DUK_READONLY_PROPERTY(ctx, entryIndex, url, AnnounceEntryWrapper::getUrl);

    // Set finalizer
    duk_push_c_function(ctx, AnnounceEntryWrapper::finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t AnnounceEntryWrapper::finalize(duk_context* ctx)
{
    Common::finalize<AnnounceEntry>(ctx);
    return 0;
}

duk_ret_t AnnounceEntryWrapper::getMessage(duk_context* ctx)
{
    AnnounceEntry* entry = Common::getPointer<AnnounceEntry>(ctx);
    duk_push_string(ctx, entry->getMessage().c_str());
    return 1;
}

duk_ret_t AnnounceEntryWrapper::getUrl(duk_context* ctx)
{
    AnnounceEntry* entry = Common::getPointer<AnnounceEntry>(ctx);
    duk_push_string(ctx, entry->getUrl().c_str());
    return 1;
}
