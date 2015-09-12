#include <hadouken/scripting/modules/bittorrent/bdecode_node_wrapper.hpp>

#include <libtorrent/bdecode.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;

void bdecode_node_wrapper::initialize(duk_context* ctx, libtorrent::bdecode_node& node)
{
    duk_idx_t entryIndex = duk_push_object(ctx);

    duk_function_list_entry functions[] =
    {
        { NULL, NULL, 0 }
    };

    duk_put_function_list(ctx, 0, functions);

    // Set internal pointer
    libtorrent::bdecode_node* e = new libtorrent::bdecode_node();
    node.swap(*e);
    
    common::set_pointer<libtorrent::bdecode_node>(ctx, entryIndex, e);

    DUK_READONLY_PROPERTY(ctx, entryIndex, type, get_type);

    // Set finalizer
    duk_push_c_function(ctx, finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t bdecode_node_wrapper::finalize(duk_context* ctx)
{
    common::finalize<libtorrent::bdecode_node>(ctx);
    return 0;
}

duk_ret_t bdecode_node_wrapper::get_type(duk_context* ctx)
{
    libtorrent::bdecode_node* node = common::get_pointer<libtorrent::bdecode_node>(ctx);
    duk_push_int(ctx, node->type());
    return 1;
}
