#include <hadouken/scripting/modules/bittorrent/feed_handle_wrapper.hpp>

#include <hadouken/scripting/modules/bittorrent/feed_status_wrapper.hpp>
#include <libtorrent/rss.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;

void feed_handle_wrapper::initialize(duk_context* ctx, const libtorrent::feed_handle& handle)
{
    duk_function_list_entry functions[] =
    {
        { "updateFeed",  update_feed,     0 },
        { "getStatus",   get_feed_status, 0 },
        { "getSettings", get_settings,    0 },
        { "setSettings", set_settings,    1 },
        { NULL,          NULL,            0 }
    };

    duk_idx_t idx = duk_push_object(ctx);
    duk_put_function_list(ctx, idx, functions);

    common::set_pointer<libtorrent::feed_handle>(ctx, idx, new libtorrent::feed_handle(handle));

    duk_push_c_function(ctx, finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t feed_handle_wrapper::finalize(duk_context* ctx)
{
    common::finalize<libtorrent::feed_handle>(ctx);
    return 0;
}

duk_ret_t feed_handle_wrapper::update_feed(duk_context* ctx)
{
    return 0;
}

duk_ret_t feed_handle_wrapper::get_feed_status(duk_context* ctx)
{
    libtorrent::feed_handle* handle = common::get_pointer<libtorrent::feed_handle>(ctx);
    feed_status_wrapper::initialize(ctx, handle->get_feed_status());
    return 1;
}

duk_ret_t feed_handle_wrapper::get_settings(duk_context* ctx)
{
    return 0;
}

duk_ret_t feed_handle_wrapper::set_settings(duk_context* ctx)
{
    return 0;
}
