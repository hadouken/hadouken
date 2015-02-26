#include <Hadouken/Http/JsonRpc/TorrentMoveStorageMethod.hpp>

#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Util/Application.h>

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Http::JsonRpc;
using namespace Poco::JSON;
using namespace Poco::Util;

Poco::Dynamic::Var::Ptr TorrentMoveStorageMethod::execute(const Array::Ptr& params)
{
    Application& app = Application::instance();
    Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();

    if (params->size() > 1)
    {
        std::string hash = params->getElement<std::string>(0);
        std::string savePath = params->getElement<std::string>(1);

        TorrentHandle handle = sess.findTorrent(hash);

        if (!handle.isValid())
        {
            return new Poco::Dynamic::Var(false);
        }

        handle.moveStorage(savePath);
    }

    return new Poco::Dynamic::Var(true);
}
