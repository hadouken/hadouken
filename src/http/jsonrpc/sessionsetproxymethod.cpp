#include <Hadouken/Http/JsonRpc/SessionSetProxyMethod.hpp>

#include <Hadouken/BitTorrent/ProxySettings.hpp>
#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Util/Application.h>

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Http::JsonRpc;
using namespace Poco::JSON;
using namespace Poco::Util;

Poco::Dynamic::Var::Ptr SessionSetProxyMethod::execute(const Array::Ptr& params)
{
    /*
    Sets the proxy settings from the first parameter in the params array.

    {
      "host": <string>,
      "port": <int>,
      "type": <int>,
      "userName": <string>,
      "password": <string>
    }
    */

    if (params->size() != 1)
    {
        return new Poco::Dynamic::Var(false);
    }

    Poco::Dynamic::Var proxyPtr = params->get(0);
    Object::Ptr proxy = proxyPtr.extract<Object::Ptr>();

    ProxySettings p;
    if (proxy->has("host")) { p.setHost(proxy->getValue<std::string>("host")); }
    if (proxy->has("port")) { p.setPort(proxy->getValue<uint16_t>("port")); }
    if (proxy->has("type")) { p.setType((ProxySettings::ProxyType)proxy->getValue<int>("type")); }
    if (proxy->has("userName")) { p.setUserName(proxy->getValue<std::string>("userName")); }
    if (proxy->has("password")) { p.setPassword(proxy->getValue<std::string>("password")); }

    Application& app = Application::instance();
    Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();
    sess.setProxy(p);

    return new Poco::Dynamic::Var(true);
}
