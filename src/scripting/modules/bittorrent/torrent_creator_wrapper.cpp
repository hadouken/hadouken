#include <hadouken/scripting/modules/bittorrent/torrent_creator_wrapper.hpp>

#include <hadouken/scripting/modules/bittorrent/entry_wrapper.hpp>
#include <libtorrent/create_torrent.hpp>
#include <libtorrent/entry.hpp>

#include "../common.hpp"
#include "../../duktape.h"

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;

duk_ret_t torrent_creator_wrapper::construct(duk_context* ctx)
{
    duk_push_this(ctx);

    libtorrent::torrent_info* info = common::get_pointer<libtorrent::torrent_info>(ctx, 0);
    libtorrent::create_torrent* creator = new libtorrent::create_torrent(*info);

    common::set_pointer<libtorrent::create_torrent>(ctx, -2, creator);

    static duk_function_list_entry functions[] =
    {
        { "generate", generate, 0 },
        { NULL, NULL, 0 }
    };

    duk_put_function_list(ctx, -1, functions);

    duk_push_c_function(ctx, finalize, 1);
    duk_set_finalizer(ctx, -2);

    return 0;
}

duk_ret_t torrent_creator_wrapper::finalize(duk_context* ctx)
{
    common::finalize<libtorrent::create_torrent>(ctx);
    return 0;
}

duk_ret_t torrent_creator_wrapper::generate(duk_context* ctx)
{
    libtorrent::create_torrent* creator = common::get_pointer<libtorrent::create_torrent>(ctx);
    libtorrent::entry entry = creator->generate();

    entry_wrapper::initialize(ctx, entry);
    return 1;
}
