#include <hadouken/scripting/modules/bittorrent/entry_wrapper.hpp>

#include <libtorrent/entry.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;

void entry_wrapper::initialize(duk_context* ctx, libtorrent::entry& entry)
{
    duk_idx_t entryIndex = duk_push_object(ctx);

    static duk_function_list_entry functions[] =
    {
        { NULL, NULL, 0 }
    };

    duk_put_function_list(ctx, 0, functions);

    // Set internal pointer
    libtorrent::entry* e = new libtorrent::entry(entry.type());
    entry.swap(*e);

    common::set_pointer<libtorrent::entry>(ctx, entryIndex, e);

    DUK_READONLY_PROPERTY(ctx, entryIndex, type, get_type);

    // Set finalizer
    duk_push_c_function(ctx, entry_wrapper::finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t entry_wrapper::finalize(duk_context* ctx)
{
    common::finalize<libtorrent::entry>(ctx);
    return 0;
}

duk_ret_t entry_wrapper::get_type(duk_context* ctx)
{
    int type = common::get_pointer<libtorrent::entry>(ctx)->type();
    duk_push_int(ctx, type);
    return 1;
}
