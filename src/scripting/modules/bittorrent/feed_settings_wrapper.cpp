#include <hadouken/scripting/modules/bittorrent/feed_settings_wrapper.hpp>

/*
#include <libtorrent/rss.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;

duk_ret_t feed_settings_wrapper::construct(duk_context* ctx)
{
    duk_push_this(ctx);

    // Set internal pointer
    common::set_pointer<libtorrent::feed_settings>(ctx, -2, new libtorrent::feed_settings());

    DUK_READWRITE_PROPERTY(ctx, -4, url, url);
    DUK_READWRITE_PROPERTY(ctx, -4, ttl, ttl);
    DUK_READWRITE_PROPERTY(ctx, -4, autoDownload, auto_download);

    // Set finalizer
    duk_push_c_function(ctx, finalize, 1);
    duk_set_finalizer(ctx, -2);

    return 0;
}

duk_ret_t feed_settings_wrapper::finalize(duk_context* ctx)
{
    common::finalize<libtorrent::feed_settings>(ctx);
    return 0;
}

duk_ret_t feed_settings_wrapper::get_auto_download(duk_context* ctx)
{
    bool auto_dl = common::get_pointer<libtorrent::feed_settings>(ctx)->auto_download;
    duk_push_boolean(ctx, auto_dl);
    return 1;
}

duk_ret_t feed_settings_wrapper::get_url(duk_context* ctx)
{
    std::string url = common::get_pointer<libtorrent::feed_settings>(ctx)->url;
    duk_push_string(ctx, url.c_str());
    return 1;
}

duk_ret_t feed_settings_wrapper::get_ttl(duk_context* ctx)
{
    int ttl = common::get_pointer<libtorrent::feed_settings>(ctx)->default_ttl;
    duk_push_int(ctx, ttl);
    return 1;
}

duk_ret_t feed_settings_wrapper::set_auto_download(duk_context* ctx)
{
    bool auto_dl = duk_require_boolean(ctx, 0);
    common::get_pointer<libtorrent::feed_settings>(ctx)->auto_download = auto_dl;
    return 0;
}

duk_ret_t feed_settings_wrapper::set_url(duk_context* ctx)
{
    std::string url = duk_require_string(ctx, 0);
    common::get_pointer<libtorrent::feed_settings>(ctx)->url = url;
    return 0;
}

duk_ret_t feed_settings_wrapper::set_ttl(duk_context* ctx)
{
    int ttl = duk_require_int(ctx, 0);
    common::get_pointer<libtorrent::feed_settings>(ctx)->default_ttl = ttl;
    return 0;
}
*/
