#ifndef HDKN_BT_SESSION_HPP
#define HDKN_BT_SESSION_HPP

#include <boost/asio.hpp>
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
        class session
        {
        public:
            session(boost::asio::io_service& io_service);
            ~session();

            void load();
            void unload();

        private:
            void alert_dispatch(std::auto_ptr<libtorrent::alert> alert_ptr);
            void handle_alert(libtorrent::alert* alert);

            void load_state();
            void load_resume_data();
            void save_state();
            void save_resume_data();

            boost::asio::io_service& io_srv_;
            libtorrent::session* sess_;
        };
    }
}

#endif