#include <Hadouken/Http/JsonRpc/TorrentPauseMethod.hpp>

#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Util/Application.h>

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Http::JsonRpc;
using namespace Poco::JSON;
using namespace Poco::Util;

Poco::Dynamic::Var::Ptr TorrentPauseMethod::execute(const Array::Ptr& params)
{
    Application& app = Application::instance();
    Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();
    
    for (Poco::Dynamic::Var item : *params)
    {
        std::string hash = item.extract<std::string>();
        std::shared_ptr<TorrentHandle> handle = sess.findTorrent(hash);

        if (handle && handle->isValid())
        {
            handle->pause();
        }
        else
        {
            app.logger().warning("Could not find torrent with hash %s", hash);
        }
    }

    return nullptr;
}
