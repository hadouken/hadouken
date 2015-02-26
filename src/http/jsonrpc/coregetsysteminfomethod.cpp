#include <Hadouken/Http/JsonRpc/CoreGetSystemInfoMethod.hpp>

#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Util/Application.h>

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Http::JsonRpc;
using namespace Poco::JSON;
using namespace Poco::Util;

Poco::Dynamic::Var::Ptr CoreGetSystemInfoMethod::execute(const Array::Ptr& params)
{
    Application& app = Application::instance();
    Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();

    Poco::DynamicStruct obj;

    Poco::DynamicStruct versionsObj;
    versionsObj["libtorrent"] = sess.getLibtorrentVersion();
    versionsObj["hadouken"] = "5.0"; // TODO

    obj["versions"] = versionsObj;

    return new Poco::Dynamic::Var(obj);
}
