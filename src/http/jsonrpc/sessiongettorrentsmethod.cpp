#include <Hadouken/Http/JsonRpc/SessionGetTorrentsMethod.hpp>

#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Util/Application.h>

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Http::JsonRpc;
using namespace Poco::JSON;
using namespace Poco::Util;

Poco::Dynamic::Var::Ptr SessionGetTorrentsMethod::execute(const Array::Ptr& params)
{
    /*
    Return all torrents in this session in a dictionary where the torrents info
    hash is key.
    */
    
    Application& app = Application::instance();
    Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();

    Poco::DynamicStruct data;
    data["foo"] = "bar";

    Poco::Dynamic::Array objects;
    objects.push_back(data);

    return new Poco::Dynamic::Var(objects);
}
