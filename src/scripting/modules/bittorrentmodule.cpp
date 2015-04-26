#include <Hadouken/Scripting/Modules/BitTorrentModule.hpp>

#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Hadouken/Scripting/Modules/BitTorrent/SessionWrapper.hpp>
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
    Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();

    SessionWrapper::initialize(ctx, sess);
    duk_put_prop_string(ctx, 0, "session");

    return 0;
}
