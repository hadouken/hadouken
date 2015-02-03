#ifndef HDKN_BT_SESSION_HPP
#define HDKN_BT_SESSION_HPP

#include <boost/asio.hpp>
#include <boost/signals2.hpp>
#include <memory>

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

        class __declspec(dllexport) session
        {
        public:
            typedef boost::signals2::signal<void()> torrent_added_t;
            typedef boost::signals2::signal<void(const torrent_handle& handle)> torrent_finished_t;
            typedef boost::signals2::signal<void()> torrent_removed_t;

            session(boost::asio::io_service& io_service);
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
            void save_state();
            void save_resume_data();

            boost::asio::io_service& io_srv_;
            libtorrent::session* sess_;

            // Signals
            torrent_added_t torrent_added_;
            torrent_finished_t torrent_finished_;
            torrent_removed_t torrent_removed_;
        };
    }
}

#endif