#include <Hadouken/Http/JsonRpc/TorrentGetPeersMethod.hpp>

#include <Hadouken/BitTorrent/PeerInfo.hpp>
#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Util/Application.h>

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Http::JsonRpc;
using namespace Poco::JSON;
using namespace Poco::Util;

Poco::Dynamic::Var::Ptr TorrentGetPeersMethod::execute(const Array::Ptr& params)
{
    Application& app = Application::instance();
    Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();

    if (params->size() < 1)
    {
        return nullptr;
    }

    std::string hash = params->getElement<std::string>(0);
    std::shared_ptr<TorrentHandle> handle = sess.findTorrent(hash);

    if (!handle->isValid())
    {
        return nullptr;
    }

    std::vector<PeerInfo> peers;
    handle->getPeerInfo(peers);

    Poco::Dynamic::Array result;
    
    for (PeerInfo peer : peers)
    {
        Poco::DynamicStruct obj;
        obj["country"] = peer.getCountry();
        obj["ip"] = peer.getRemoteAddress().host().toString();
        obj["port"] = peer.getRemoteAddress().port();
        obj["connectionType"] = (int)peer.getConnectionType();
        obj["flags"] = 0;
        obj["client"] = peer.getClient();
        obj["progress"] = peer.getProgress();
        obj["downloadRate"] = peer.getDownSpeed();
        obj["uploadRate"] = peer.getUpSpeed();
        obj["downloadedBytes"] = peer.getDownloadedBytes();
        obj["uploadedBytes"] = peer.getUploadedBytes();

        result.push_back(obj);
    }
    
    return new Poco::Dynamic::Var(result);
}
