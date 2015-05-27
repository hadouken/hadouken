#include <Hadouken/Scripting/Modules/BitTorrent/EntryWrapper.hpp>

#include <libtorrent/entry.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace Hadouken::Scripting::Modules;
using namespace Hadouken::Scripting::Modules::BitTorrent;

void EntryWrapper::initialize(duk_context* ctx, libtorrent::entry& entry)
{
    duk_idx_t entryIndex = duk_push_object(ctx);

    duk_function_list_entry functions[] =
    {
        { NULL, NULL, 0 }
    };

    duk_put_function_list(ctx, 0, functions);

    // Set internal pointer
    libtorrent::entry* e = new libtorrent::entry(entry.type());
    entry.swap(*e);

    Common::setPointer<libtorrent::entry>(ctx, entryIndex, e);

    DUK_READONLY_PROPERTY(ctx, entryIndex, type, getType);

    // Set finalizer
    duk_push_c_function(ctx, EntryWrapper::finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t EntryWrapper::finalize(duk_context* ctx)
{
    Common::finalize<libtorrent::entry>(ctx);
    return 0;
}

duk_ret_t EntryWrapper::getType(duk_context* ctx)
{
    int type = Common::getPointer<libtorrent::entry>(ctx)->type();
    duk_push_int(ctx, type);
    return 1;
}
