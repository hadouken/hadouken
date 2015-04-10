#include "pushoverextension.hpp"

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

using namespace Pushover;
using namespace Hadouken::BitTorrent;
using namespace Poco::Util;

PushoverExtension::PushoverExtension()
    : logger_(Poco::Logger::get("pushover.pushoverextension")),
      messagesUrl_("https://api.pushover.net/1/messages.json")
{
}

void PushoverExtension::load(AbstractConfiguration& config)
{
    if (config.hasProperty("extensions.pushover.token")
        && config.hasProperty("extensions.pushover.user"))
    {
        token_ = config.getString("extensions.pushover.token");
        user_  = config.getString("extensions.pushover.user");

        for (int i = 0; i < std::numeric_limits<int>::max(); i++)
        {
            std::string index = std::to_string(i);
            std::string query = "extensions.pushover.enabledEvents[" + index + "]";

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

        logger_.information("Pushover extension loaded, %z enabled event(s).", events_.size());
    }
    else
    {
        logger_.warning("No Pushover token/user set. Notifications will not be pushed.");
    }

    Session& sess = Application::instance().getSubsystem<TorrentSubsystem>().getSession();
    sess.onTorrentAdded += Poco::delegate(this, &PushoverExtension::onTorrentAdded);
    sess.onTorrentFinished += Poco::delegate(this, &PushoverExtension::onTorrentFinished);

    if (isEventEnabled("hadouken.loaded"))
    {
        push("Hadouken loaded", "Hadouken loaded");
    }
}

void PushoverExtension::unload()
{
    Session& sess = Application::instance().getSubsystem<TorrentSubsystem>().getSession();
    sess.onTorrentAdded -= Poco::delegate(this, &PushoverExtension::onTorrentAdded);
    sess.onTorrentFinished -= Poco::delegate(this, &PushoverExtension::onTorrentFinished);
}

bool PushoverExtension::isEventEnabled(std::string eventName)
{
    return (std::find(events_.begin(), events_.end(), eventName) != events_.end());
}

void PushoverExtension::onTorrentAdded(const void* sender, std::shared_ptr<TorrentHandle>& handle)
{
    if (!isEventEnabled("torrent.added")) { return; }

    TorrentStatus status = handle->getStatus();
    push("Torrent added", "The torrent \"" + status.getName() + "\" has been added.");
}

void PushoverExtension::onTorrentFinished(const void* sender, std::shared_ptr<TorrentHandle>& handle)
{
    if (!isEventEnabled("torrent.finished")) { return; }
    
    TorrentStatus status = handle->getStatus();
    push("Torrent finished", "The torrent \"" + status.getName() + "\" has finished downloading.");
}

void PushoverExtension::push(std::string title, std::string body)
{
    using namespace Poco::Net;

    if (token_.empty() || user_.empty())
    {
        return;
    }

    HTTPSClientSession session(messagesUrl_.getHost());
    HTTPRequest request(HTTPRequest::HTTP_POST, messagesUrl_.getPathAndQuery());
    
    HTMLForm form;
    form.set("token", token_);
    form.set("user", user_);
    form.set("title", title);
    form.set("message", body);
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
        logger_.error("Could not send Pushover notification. HTTP status %d.", (int)response.getStatus());
    }
}
