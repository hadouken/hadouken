#ifndef HDKN_HTTP_CONNECTION_HPP
#define HDKN_HTTP_CONNECTION_HPP

#include <array>
#include <boost/asio.hpp>
#include <boost/signals2.hpp>
#include <hadouken/http/http_parser.h>
#include <hadouken/http/reply.hpp>
#include <hadouken/http/request.hpp>
#include <memory>

using boost::asio::ip::tcp;

namespace hadouken
{
    namespace http
    {
        class connection_manager;

        typedef boost::signals2::signal<void(const request& request, reply& reply)> incoming_request_t;

        class connection : public std::enable_shared_from_this<connection>
        {
        public:
            connection(const connection&) = delete;
            connection& operator=(const connection&) = delete;

            explicit connection(tcp::socket socket,
                connection_manager& manager);

            void start();

            void stop();

            boost::signals2::connection on_incoming_request(const incoming_request_t::slot_type& subscriber);

        private:
            void do_read();

            void do_write();

            http_parser_settings parser_settings_;

            http_parser parser_;

            tcp::socket socket_;

            connection_manager& manager_;

            std::array<char, 8192> buffer_;

            request request_;

            reply reply_;

            incoming_request_t incoming_request_;
        };

        typedef std::shared_ptr<connection> connection_ptr;
    }
}

#endif