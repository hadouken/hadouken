#include <hadouken/scripting/modules/bittorrent/lazy_entry_wrapper.hpp>

#include <libtorrent/lazy_entry.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;

void lazy_entry_wrapper::initialize(duk_context* ctx, libtorrent::lazy_entry& entry)
{
    duk_idx_t entryIndex = duk_push_object(ctx);

    static duk_function_list_entry functions[] =
    {
        { NULL, NULL, 0 }
    };

    duk_put_function_list(ctx, 0, functions);

    // Set internal pointer
    libtorrent::lazy_entry* e = new libtorrent::lazy_entry();
    entry.swap(*e);
    
    common::set_pointer<libtorrent::lazy_entry>(ctx, entryIndex, e);

    DUK_READONLY_PROPERTY(ctx, entryIndex, type, get_type);

    // Set finalizer
    duk_push_c_function(ctx, finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t lazy_entry_wrapper::finalize(duk_context* ctx)
{
    common::finalize<libtorrent::lazy_entry>(ctx);
    return 0;
}

duk_ret_t lazy_entry_wrapper::get_type(duk_context* ctx)
{
    libtorrent::lazy_entry* entry = common::get_pointer<libtorrent::lazy_entry>(ctx);
    duk_push_int(ctx, entry->type());
    return 1;
}
