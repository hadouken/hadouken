#include <Hadouken/Http/JsonRpc/CoreGetSystemInfoMethod.hpp>

#include <Hadouken/Version.hpp>
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
    obj["commitish"] = Hadouken::Version::GIT_COMMIT_HASH();
    obj["branch"] = Hadouken::Version::GIT_BRANCH();

    Poco::DynamicStruct versionsObj;
    versionsObj["libtorrent"] = sess.getLibtorrentVersion();
    versionsObj["hadouken"] = Hadouken::Version::VERSION();

    obj["versions"] = versionsObj;

    return new Poco::Dynamic::Var(obj);
}
