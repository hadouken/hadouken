#include "automoveextension.hpp"

#include <Hadouken/BitTorrent/Session.hpp>
#include <Hadouken/BitTorrent/TorrentHandle.hpp>
#include <Hadouken/BitTorrent/TorrentStatus.hpp>
#include <Hadouken/BitTorrent/TorrentSubsystem.hpp>
#include <Poco/Delegate.h>
#include <Poco/Util/Application.h>

using namespace AutoMove;
using namespace Hadouken::BitTorrent;
using namespace Poco::Util;

AutoMoveExtension::AutoMoveExtension()
    : logger_(Poco::Logger::get("automove.automoveextension"))
{
}

void AutoMoveExtension::load(AbstractConfiguration& config)
{
    for (int i = 0; i < std::numeric_limits<int>::max(); i++)
    {
        std::string index = std::to_string(i);
        std::string query = "plugins.automove.rules[" + index + "]";

        try
        {
            AbstractConfiguration* ruleView = config.createView(query);
            std::string field = ruleView->getString("field");
            std::string pattern = ruleView->getString("pattern");
            std::string targetPath = ruleView->getString("targetPath");

            Rule r(field, std::regex(pattern), targetPath);
            rules_.push_back(r);
        }
        catch (Poco::Exception)
        {
            break;
        }
    }

    logger_.information("AutoMove loaded with %z rule(s).", rules_.size());

    Session& sess = Application::instance().getSubsystem<TorrentSubsystem>().getSession();
    sess.onTorrentFinished += Poco::delegate(this, &AutoMoveExtension::onTorrentCompleted);
}

void AutoMoveExtension::unload()
{
    Session& sess = Application::instance().getSubsystem<TorrentSubsystem>().getSession();
    sess.onTorrentFinished -= Poco::delegate(this, &AutoMoveExtension::onTorrentCompleted);
}

void AutoMoveExtension::onTorrentCompleted(const void* sender, TorrentHandle& handle)
{
    for (Rule rule : rules_)
    {
        std::string fieldValue = getFieldValue(handle, rule.getField());
        if (fieldValue.empty()) { continue; }

        if (!std::regex_match(fieldValue, rule.getPattern()))
        {
            logger_.debug("Pattern '%s' did not match input '%s'.", rule.getPattern(), fieldValue);
            continue;
        }

        // Matched and ready.
        handle.moveStorage(rule.getTargetPath());
    }
}

std::string AutoMoveExtension::getFieldValue(TorrentHandle& handle, std::string fieldName)
{
    if (fieldName.compare("name") == 0)
    {
        TorrentStatus status = handle.getStatus();
        return status.getName();
    }

    return std::string();
}
