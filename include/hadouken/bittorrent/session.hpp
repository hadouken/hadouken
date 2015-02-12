#ifndef HDKN_BT_SESSION_HPP
#define HDKN_BT_SESSION_HPP

#include <hadouken/config.hpp>

#include <boost/asio.hpp>
#include <boost/filesystem.hpp>
#include <boost/property_tree/ptree.hpp>
#include <boost/signals2.hpp>
#include <memory>
#include <vector>

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

        class HDKN_API session
        {
        public:
            typedef boost::signals2::signal<void()> torrent_added_t;
            typedef boost::signals2::signal<void(const torrent_handle& handle)> torrent_finished_t;
            typedef boost::signals2::signal<void()> torrent_removed_t;

            session(const boost::property_tree::ptree& config, boost::asio::io_service& io_service);
            ~session();

            void load();
            void unload();

            void add_torrent_file(const std::string& file, const std::string& save_path);

            boost::signals2::connection on_torrent_added(const torrent_added_t::slot_type &subscriber);
            boost::signals2::connection on_torrent_finished(const torrent_finished_t::slot_type &subscriber);

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

            const boost::property_tree::ptree& config_;
            boost::asio::io_service& io_srv_;
            libtorrent::session* sess_;

            boost::asio::deadline_timer auto_add_timer_;
            std::vector<auto_add_rule> auto_add_rules_;
            std::vector<auto_move_rule> auto_move_rules_;


            // default values
            std::string default_save_path_;
            std::string state_path_;

            // Signals
            torrent_added_t torrent_added_;
            torrent_finished_t torrent_finished_;
            torrent_removed_t torrent_removed_;
        };
    }
}

#endif
