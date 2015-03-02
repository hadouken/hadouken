#include <Hadouken/Http/JsonRpc/SessionGetProxyMethod.hpp>

#include <Hadouken/BitTorrent/ProxySettings.hpp>
#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Util/Application.h>

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Http::JsonRpc;
using namespace Poco::JSON;
using namespace Poco::Util;

Poco::Dynamic::Var::Ptr SessionGetProxyMethod::execute(const Array::Ptr& params)
{
    /*
    Return the proxy settings as an object dictionary.
    */

    Application& app = Application::instance();
    Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();
    ProxySettings proxy = sess.getProxy();

    Poco::DynamicStruct result;
    result["host"] = proxy.getHost();
    result["password"] = proxy.getPassword();
    result["port"] = proxy.getPort();
    result["type"] = (int)proxy.getType();
    result["userName"] = proxy.getUserName();

    return new Poco::Dynamic::Var(result);
}
