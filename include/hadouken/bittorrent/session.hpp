#ifndef HDKN_BT_SESSION_HPP
#define HDKN_BT_SESSION_HPP

#include <hadouken/config.hpp>

#include <boost/asio.hpp>
#include <boost/filesystem.hpp>
#include <boost/property_tree/ptree.hpp>
#include <boost/signals2.hpp>
#include <memory>
#include <vector>

namespace pt = boost::property_tree;

namespace libtorrent
{
    class alert;
    class session;
}

namespace hadouken
{
    namespace bittorrent
    {
        class torrent_handle;

        class auto_add_rule
        {
        public:
            std::string source_path;
            std::string save_path;
            std::string pattern;
        };

        class auto_move_rule
        {
        public:
            std::string pattern;
            std::string input;
            std::string path;
        };

        class session
        {
        public:
            HDKN_API session(const pt::ptree& config, boost::asio::io_service& io_service);
            HDKN_API ~session();

            void HDKN_API load();
            void HDKN_API unload();

            void HDKN_API api_session_get_torrents(const pt::ptree& params, pt::ptree& result);

            void HDKN_API add_torrent_file(const std::string& file, const std::string& save_path);

        private:
            void alert_dispatch(std::auto_ptr<libtorrent::alert> alert_ptr);
            void handle_alert(libtorrent::alert* alert);

            void load_state();
            void load_resume_data();
            void load_auto_add_rules();
            void load_auto_move_rules();
            void save_state();
            void save_resume_data();

            void check_auto_add_folders(const boost::system::error_code& error);

            boost::filesystem::path get_state_path();
            boost::filesystem::path get_torrents_state_path();

            const boost::property_tree::ptree& config_;
            boost::asio::io_service& io_srv_;
            libtorrent::session* sess_;

            boost::asio::deadline_timer auto_add_timer_;
            std::vector<auto_add_rule> auto_add_rules_;
            std::vector<auto_move_rule> auto_move_rules_;


            // default values
            std::string default_save_path_;
            std::string state_path_;
        };
    }
}

#endif
