#include "auto_move.hpp"

#include <boost/filesystem.hpp>
#include <boost/log/trivial.hpp>
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/json_parser.hpp>
#include <boost/regex.hpp>

auto_move::auto_move(hadouken::service_locator& service_locator)
    : plugin(service_locator)
{
    sess_ = service_locator.request<hadouken::bittorrent::session*>("bt.session");
}

void auto_move::load()
{
    BOOST_LOG_TRIVIAL(debug) << "Loading auto move";
    load_configuration();

    sess_->on_torrent_finished(boost::bind(&auto_move::torrent_finished, this, _1));
}

void auto_move::load_configuration()
{
    namespace fs = boost::filesystem;

    fs::path config_path("config/auto_move.json");

    if (!fs::exists(config_path))
    {
        BOOST_LOG_TRIVIAL(warning) << "No \"auto_move.json\" found at " << fs::current_path() / "config";
        return;
    }

    boost::property_tree::ptree pt;
    boost::property_tree::read_json(config_path.string(), pt);

    BOOST_LOG_TRIVIAL(info) << "Found " << pt.size() << " auto move rules.";

    for (auto item : pt)
    {
        rule r;
        r.input = item.second.get<std::string>("input");
        r.path = item.second.get<std::string>("path");
        r.pattern = item.second.get<std::string>("pattern");

        rules_.push_back(r);
    }
}

void auto_move::torrent_finished(const hadouken::bittorrent::torrent_handle& handle)
{
    BOOST_LOG_TRIVIAL(info) << "Torrent " << handle.name() << " finished downloading.";

    for (auto rule : rules_)
    {
        boost::regex re(rule.pattern, boost::regex_constants::icase);
        
        if (boost::regex_match(handle.name(), re))
        {
            BOOST_LOG_TRIVIAL(info) << "Found matching rule \"" << rule.pattern << "\".";
            BOOST_LOG_TRIVIAL(info) << "Moving torrent to \"" << rule.path << "\"";

            handle.move_storage(rule.path);

            break;
        }
    }
}