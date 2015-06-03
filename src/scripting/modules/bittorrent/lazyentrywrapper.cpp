#include <Hadouken/Scripting/Modules/BitTorrent/LazyEntryWrapper.hpp>

#include <libtorrent/lazy_entry.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace Hadouken::Scripting::Modules;
using namespace Hadouken::Scripting::Modules::BitTorrent;

void LazyEntryWrapper::initialize(duk_context* ctx, libtorrent::lazy_entry& entry)
{
    duk_idx_t entryIndex = duk_push_object(ctx);

    duk_function_list_entry functions[] =
    {
        { NULL, NULL, 0 }
    };

    duk_put_function_list(ctx, 0, functions);

    // Set internal pointer
    libtorrent::lazy_entry* e = new libtorrent::lazy_entry();
    entry.swap(*e);
    
    Common::setPointer<libtorrent::lazy_entry>(ctx, entryIndex, e);

    DUK_READONLY_PROPERTY(ctx, entryIndex, type, getType);

    // Set finalizer
    duk_push_c_function(ctx, LazyEntryWrapper::finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t LazyEntryWrapper::finalize(duk_context* ctx)
{
    Common::finalize<libtorrent::lazy_entry>(ctx);
    return 0;
}

duk_ret_t LazyEntryWrapper::getType(duk_context* ctx)
{
    libtorrent::lazy_entry* entry = Common::getPointer<libtorrent::lazy_entry>(ctx);
    duk_push_int(ctx, entry->type());
    return 1;
}
