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
    TorrentHandle handle = sess.findTorrent(hash);

    if (!handle.isValid())
    {
        return nullptr;
    }

    std::vector<PeerInfo> peers;
    handle.getPeerInfo(peers);

    Poco::Dynamic::Array result;
    
    for (PeerInfo peer : peers)
    {
        Poco::DynamicStruct obj;
        obj["ip"] = peer.getRemoteAddress().toString();
        obj["client"] = peer.getClient();
        obj["downloadRate"] = peer.getDownSpeed();
        obj["uploadRate"] = peer.getUpSpeed();

        result.push_back(obj);
    }
    
    return new Poco::Dynamic::Var(result);
}
