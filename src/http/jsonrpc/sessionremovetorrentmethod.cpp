#include <Hadouken/Http/JsonRpc/SessionRemoveTorrentMethod.hpp>

#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Util/Application.h>

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Http::JsonRpc;
using namespace Poco::JSON;
using namespace Poco::Util;

Poco::Dynamic::Var::Ptr SessionRemoveTorrentMethod::execute(const Array::Ptr& params)
{
    Application& app = Application::instance();
    Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();

    if (params->size() > 0)
    {
        std::string hash = params->getElement<std::string>(0);
        int options = 0;

        std::shared_ptr<TorrentHandle> handle = sess.findTorrent(hash);

        if (!handle->isValid())
        {
            return new Poco::Dynamic::Var(false);
        }

        if (params->size() > 1)
        {
            options = params->getElement<int>(1);
        }

        sess.removeTorrent(handle, options);
    }
    
    return new Poco::Dynamic::Var(true);
}
