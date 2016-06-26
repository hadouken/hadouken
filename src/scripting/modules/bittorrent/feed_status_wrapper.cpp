#include <hadouken/scripting/modules/bittorrent/feed_status_wrapper.hpp>

#include <libtorrent/rss.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;

void feed_status_wrapper::initialize(duk_context* ctx, const libtorrent::feed_status& status)
{
    duk_idx_t idx = duk_push_object(ctx);

    static duk_function_list_entry functions[] =
    {
        { "getItems", get_items, 0 },
        { NULL,       NULL,      0 }
    };

    duk_put_function_list(ctx, idx, functions);

    DUK_READONLY_PROPERTY(ctx, idx, description, get_description);
    DUK_READONLY_PROPERTY(ctx, idx, lastUpdate, get_last_update);
    DUK_READONLY_PROPERTY(ctx, idx, nextUpdate, get_next_update);
    DUK_READONLY_PROPERTY(ctx, idx, isUpdating, is_updating);
    DUK_READONLY_PROPERTY(ctx, idx, url, get_url);
    DUK_READONLY_PROPERTY(ctx, idx, title, get_title);

    common::set_pointer<libtorrent::feed_status>(ctx, idx, new libtorrent::feed_status(status));

    duk_push_c_function(ctx, finalize, 1);
    duk_set_finalizer(ctx, -2);
}

duk_ret_t feed_status_wrapper::finalize(duk_context* ctx)
{
    common::finalize<libtorrent::feed_status>(ctx);
    return 0;
}

duk_ret_t feed_status_wrapper::get_items(duk_context* ctx)
{
    libtorrent::feed_status* status = common::get_pointer<libtorrent::feed_status>(ctx);

    duk_idx_t arrIdx = duk_push_array(ctx);
    int i = 0;

    for (libtorrent::feed_item item : status->items)
    {
        duk_idx_t itemIdx = duk_push_object(ctx);

        duk_push_string(ctx, item.url.c_str());
        duk_put_prop_string(ctx, itemIdx, "url");

        duk_push_string(ctx, item.uuid.c_str());
        duk_put_prop_string(ctx, itemIdx, "uuid");

        duk_push_string(ctx, item.title.c_str());
        duk_put_prop_string(ctx, itemIdx, "title");

        duk_push_string(ctx, item.description.c_str());
        duk_put_prop_string(ctx, itemIdx, "description");

        duk_push_string(ctx, item.comment.c_str());
        duk_put_prop_string(ctx, itemIdx, "comment");

        duk_push_string(ctx, item.category.c_str());
        duk_put_prop_string(ctx, itemIdx, "category");

        duk_push_number(ctx, item.size);
        duk_put_prop_string(ctx, itemIdx, "size");

        duk_put_prop_index(ctx, arrIdx, i);
        ++i;
    }

    return 1;
}

duk_ret_t feed_status_wrapper::get_description(duk_context* ctx)
{
    libtorrent::feed_status* status = common::get_pointer<libtorrent::feed_status>(ctx);
    duk_push_string(ctx, status->description.c_str());
    return 1;
}

duk_ret_t feed_status_wrapper::get_last_update(duk_context* ctx)
{
    libtorrent::feed_status* status = common::get_pointer<libtorrent::feed_status>(ctx);
    duk_push_number(ctx, status->last_update);
    return 1;
}

duk_ret_t feed_status_wrapper::get_next_update(duk_context* ctx)
{
    libtorrent::feed_status* status = common::get_pointer<libtorrent::feed_status>(ctx);
    duk_push_number(ctx, status->next_update);
    return 1;
}

duk_ret_t feed_status_wrapper::is_updating(duk_context* ctx)
{
    libtorrent::feed_status* status = common::get_pointer<libtorrent::feed_status>(ctx);
    duk_push_boolean(ctx, status->updating);
    return 1;
}

duk_ret_t feed_status_wrapper::get_url(duk_context* ctx)
{
    libtorrent::feed_status* status = common::get_pointer<libtorrent::feed_status>(ctx);
    duk_push_string(ctx, status->url.c_str());
    return 1;
}

duk_ret_t feed_status_wrapper::get_title(duk_context* ctx)
{
    libtorrent::feed_status* status = common::get_pointer<libtorrent::feed_status>(ctx);
    duk_push_string(ctx, status->title.c_str());
    return 1;
}
