#ifndef HADOUKEN_APP_HPP
#define HADOUKEN_APP_HPP

#include <boost/asio.hpp>
#include <boost/property_tree/ptree.hpp>
#include <hadouken/http/http_server.hpp>
#include <hadouken/scripting/script_host.hpp>
#include <memory>
#include <thread>

namespace libtorrent
{
    class alert;
    class session;
}

namespace pt = boost::property_tree;

namespace hadouken
{
    class application
    {
    public:
        application(boost::shared_ptr<boost::asio::io_service> io, const pt::ptree& config);
        ~application();

        void start();
        void stop();

        libtorrent::session& session() const;
        hadouken::scripting::script_host& script_host() const;

    protected:
        void alert_dispatch(std::auto_ptr<libtorrent::alert> alert_ptr);
        void alert_handler(libtorrent::alert* alert);

        bool is_authenticated(std::string auth_header);
        std::string rpc(std::string data);

    private:
        boost::shared_ptr<boost::asio::io_service> io_;
        pt::ptree config_;
        std::unique_ptr<libtorrent::session> session_;
        std::unique_ptr<hadouken::http::http_server> http_;
        std::unique_ptr<hadouken::scripting::script_host> script_host_;
    };
}

#endif
