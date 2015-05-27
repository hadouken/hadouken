#include <Hadouken/Scripting/Modules/BitTorrentModule.hpp>

#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/AddTorrentParamsWrapper.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/SessionWrapper.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/TorrentCreatorWrapper.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/TorrentInfoWrapper.hpp>
#include <libtorrent/session.hpp>
#include <Poco/Util/Application.h>

#include "common.hpp"
#include "../duktape.h"

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Scripting::Modules;
using namespace Hadouken::Scripting::Modules::BitTorrent;
using namespace Poco::Util;

duk_ret_t BitTorrentModule::initialize(duk_context* ctx)
{
    // Set properties and functions on exports
    Application& app = Application::instance();
    libtorrent::session& session = app.getSubsystem<TorrentSubsystem>().getSession();

    SessionWrapper::initialize(ctx, session);
    duk_put_prop_string(ctx, 0, "session");

    duk_push_c_function(ctx, AddTorrentParamsWrapper::construct, 0);
    duk_put_prop_string(ctx, 0, "AddTorrentParams");

    duk_push_c_function(ctx, TorrentCreatorWrapper::construct, 1);
    duk_put_prop_string(ctx, 0, "TorrentCreator");

    duk_push_c_function(ctx, TorrentInfoWrapper::construct, 1);
    duk_put_prop_string(ctx, 0, "TorrentInfo");

    return 0;
}

duk_ret_t BitTorrentModule::ctorAddTorrentParams(duk_context* ctx)
{
    return 0;
}
