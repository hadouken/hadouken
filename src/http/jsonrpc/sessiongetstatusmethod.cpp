#include <Hadouken/Http/JsonRpc/SessionGetStatusMethod.hpp>

#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/SessionStatus.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Util/Application.h>

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Http::JsonRpc;
using namespace Poco::JSON;
using namespace Poco::Util;

Poco::Dynamic::Var::Ptr SessionGetStatusMethod::execute(const Array::Ptr& params)
{
    /*
    Return the session status as an object dictionary.
    */

    Application& app = Application::instance();
    Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();
    SessionStatus status = sess.getStatus();

    Poco::DynamicStruct result;
    result["allowedUploadSlots"] = status.getAllowedUploadSlots();
    result["dhtDownloadRate"] = status.getDhtDownloadRate();
    result["dhtNodes"] = status.getDhtNodes();
    result["downloadRate"] = status.getDownloadRate();
    result["totalDownloadedBytes"] = status.getTotalDownload();
    result["totalUploadedBytes"] = status.getTotalUpload();
    result["uploadRate"] = status.getUploadRate();
    result["hasIncomingConnections"] = status.hasIncomingConnections();

    return new Poco::Dynamic::Var(result);
}
