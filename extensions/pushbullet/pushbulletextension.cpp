#include "pushbulletextension.hpp"

#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/BitTorrent/TorrentStatus.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Delegate.h>
#include <Poco/Net/HTTPBasicCredentials.h>
#include <Poco/Net/HTMLForm.h>
#include <Poco/Net/HTTPSClientSession.h>
#include <Poco/Net/HTTPRequest.h>
#include <Poco/Net/HTTPResponse.h>
#include <Poco/URI.h>
#include <Poco/Util/Application.h>
#include <iostream>

using namespace Pushbullet;
using namespace Hadouken::BitTorrent;
using namespace Poco::Util;

PushbulletExtension::PushbulletExtension()
    : logger_(Poco::Logger::get("pushbullet.pushbulletextension")),
      pushesUrl_("https://api.pushbullet.com/v2/pushes")
{
}

void PushbulletExtension::load(AbstractConfiguration& config)
{
    if (config.hasProperty("extensions.pushbullet.token"))
    {
        authToken_ = config.getString("extensions.pushbullet.token");

        for (int i = 0; i < std::numeric_limits<int>::max(); i++)
        {
            std::string index = std::to_string(i);
            std::string query = "extensions.pushbullet.enabledEvents[" + index + "]";

            if (config.has(query))
            {
                std::string eventName = config.getString(query);
                events_.push_back(eventName);
            }
            else
            {
                break;
            }
        }

        logger_.information("Pushbullet extension loaded, %z enabled event(s).", events_.size());
    }
    else
    {
        logger_.warning("No Pushbullet token set. Notifications will not be pushed.");
    }

    Session& sess = Application::instance().getSubsystem<TorrentSubsystem>().getSession();
    sess.onTorrentAdded += Poco::delegate(this, &PushbulletExtension::onTorrentAdded);
    sess.onTorrentFinished += Poco::delegate(this, &PushbulletExtension::onTorrentFinished);

    if (isEventEnabled("hadouken.loaded"))
    {
        push("Hadouken loaded", "Hadouken loaded");
    }
}

void PushbulletExtension::unload()
{
    Session& sess = Application::instance().getSubsystem<TorrentSubsystem>().getSession();
    sess.onTorrentAdded -= Poco::delegate(this, &PushbulletExtension::onTorrentAdded);
    sess.onTorrentFinished -= Poco::delegate(this, &PushbulletExtension::onTorrentFinished);
}

bool PushbulletExtension::isEventEnabled(std::string eventName)
{
    return (std::find(events_.begin(), events_.end(), eventName) != events_.end());
}

void PushbulletExtension::onTorrentAdded(const void* sender, std::shared_ptr<TorrentHandle>& handle)
{
    if (!isEventEnabled("torrent.added")) { return; }

    TorrentStatus status = handle->getStatus();
    push("Torrent added", "The torrent \"" + status.getName() + "\" has been added.");
}

void PushbulletExtension::onTorrentFinished(const void* sender, std::shared_ptr<TorrentHandle>& handle)
{
    if (!isEventEnabled("torrent.finished")) { return; }
    
    TorrentStatus status = handle->getStatus();
    push("Torrent finished", "The torrent \"" + status.getName() + "\" has finished downloading.");
}

void PushbulletExtension::push(std::string title, std::string body)
{
    using namespace Poco::Net;

    if (authToken_.empty())
    {
        return;
    }

    HTTPSClientSession session(pushesUrl_.getHost());
    HTTPRequest request(HTTPRequest::HTTP_POST, pushesUrl_.getPathAndQuery());
    
    HTTPBasicCredentials creds;
    creds.setUsername(authToken_);
    creds.authenticate(request);

    HTMLForm form;
    form.set("title", title);
    form.set("body", body);
    form.set("type", "note");
    form.prepareSubmit(request);

    try
    {
        form.write(session.sendRequest(request));
    }
    catch (Poco::Exception& exception)
    {
        logger_.error("Could not write push notification request: %s.", exception.displayText());
        return;
    }

    HTTPResponse response;
    session.receiveResponse(response);

    if (response.getStatus() != HTTPResponse::HTTP_OK)
    {
        // TODO: parse the response stream JSON object and print the real error message.
        logger_.error("Could not send Pushbullet notification. HTTP status %d.", (int)response.getStatus());
    }
}
