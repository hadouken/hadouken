#include <Hadouken/Http/JsonRpc/SessionAddTorrentFileMethod.hpp>

#include <Hadouken/BitTorrent/AddTorrentParams.hpp>
#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Base64Decoder.h>
#include <Poco/StreamCopier.h>
#include <Poco/Util/Application.h>

using namespace Hadouken::BitTorrent;
using namespace Hadouken::Http::JsonRpc;
using namespace Poco::JSON;
using namespace Poco::Util;

Poco::Dynamic::Var::Ptr SessionAddTorrentFileMethod::execute(const Array::Ptr& params)
{
    /*
    Add a torrent file to the session. The file is the first parameter in the
    array. The second parameter contains an optional object with data such as
    save path.
    */

    // Get the file.
    std::stringstream encoded_stream(params->getElement<std::string>(0));
    std::stringstream decoded_stream;

    Poco::Base64Decoder b64decoder(encoded_stream);
    Poco::StreamCopier::copyStream(b64decoder, decoded_stream);

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

    std::string data_str = decoded_stream.str();
    std::vector<char> data(data_str.begin(), data_str.end());

    std::string hash = sess.addTorrentFile(data, p);

    return new Poco::Dynamic::Var(hash);
}
