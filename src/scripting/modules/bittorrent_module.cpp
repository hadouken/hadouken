#include <hadouken/scripting/modules/bittorrent_module.hpp>

#include <hadouken/scripting/modules/bittorrent/add_torrent_params_wrapper.hpp>
#include <hadouken/scripting/modules/bittorrent/feed_settings_wrapper.hpp>
#include <hadouken/scripting/modules/bittorrent/session_wrapper.hpp>
#include <hadouken/scripting/modules/bittorrent/torrent_creator_wrapper.hpp>
#include <hadouken/scripting/modules/bittorrent/torrent_info_wrapper.hpp>
#include <libtorrent/session.hpp>

#include "common.hpp"
#include "../duktape.h"

using namespace hadouken::scripting::modules;
using namespace hadouken::scripting::modules::bittorrent;

duk_ret_t bittorrent_module::initialize(duk_context* ctx, libtorrent::session& session)
{
    // Set properties and functions on exports

    session_wrapper::initialize(ctx, session);
    duk_put_prop_string(ctx, 2, "session");

    duk_push_c_function(ctx, add_torrent_params_wrapper::construct, 0);
    duk_put_prop_string(ctx, 2, "AddTorrentParams");

    duk_push_c_function(ctx, feed_settings_wrapper::construct, 0);
    duk_put_prop_string(ctx, 2, "FeedSettings");

    duk_push_c_function(ctx, torrent_creator_wrapper::construct, 1);
    duk_put_prop_string(ctx, 2, "TorrentCreator");

    duk_push_c_function(ctx, torrent_info_wrapper::construct, 1);
    duk_put_prop_string(ctx, 2, "TorrentInfo");

    return 0;
}
