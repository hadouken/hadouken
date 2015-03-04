#include <Hadouken/Http/JsonRpc/SessionAddTorrentUriMethod.hpp>

#include <Hadouken/BitTorrent/AddTorrentParams.hpp>
#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Util/Application.h>

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Http::JsonRpc;
using namespace Poco::JSON;
using namespace Poco::Util;

Poco::Dynamic::Var::Ptr SessionAddTorrentUriMethod::execute(const Array::Ptr& params)
{
    /*
    Add a magnet link to the session. The magnet link URI is the first parameter in the
    array. The second parameter contains an optional object with data such as
    save path.
    */

    AddTorrentParams p;
    
    if (params->size() > 1)
    {
        Poco::Dynamic::Var addParamsPtr = params->get(1);
        Object::Ptr addParams = addParamsPtr.extract<Object::Ptr>();

        // Extract any parameters from the object.
        if (addParams->has("savePath")) { p.savePath = addParams->getValue<std::string>("savePath"); }
    }

    Application& app = Application::instance();
    Session& sess = app.getSubsystem<TorrentSubsystem>().getSession();

    sess.addTorrentUri(params->getElement<std::string>(0), p);

    return new Poco::Dynamic::Var(true);
}
