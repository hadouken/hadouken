#include "automoveextension.hpp"
#include "patternfilter.hpp"
#include "tagsfilter.hpp"

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
        std::string query = "extensions.automove.rules[" + index + "]";

        if (config.has(query))
        {
            AbstractConfiguration* ruleView = config.createView(query);

            std::string path = ruleView->getString("path");
            std::string filter = ruleView->getString("filter");

            AbstractConfiguration* dataView = ruleView->createView("data");

            if (filter == "pattern")
            {
                std::regex pattern(dataView->getString("pattern"));
                std::string field = dataView->getString("field");

                Rule rule(path, new PatternFilter(pattern, field));
                rules_.push_back(rule);
            }
            else if (filter == "tags")
            {
                std::vector<std::string> tags;

                for (int j = 0; j < std::numeric_limits<int>::max(); j++)
                {
                    index = std::to_string(j);
                    std::string tagQuery = query + ".data[" + index + "]";

                    if (config.has(tagQuery))
                    {
                        tags.push_back(config.getString(tagQuery));
                    }
                    else
                    {
                        break;
                    }
                }

                Rule rule(path, new TagsFilter(tags));
                rules_.push_back(rule);
            }
        }
        else
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
        if (!rule.filter->isMatch(handle))
        {
            continue;
        }

        handle.moveStorage(rule.path);
        break;
    }
}

