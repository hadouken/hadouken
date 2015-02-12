#ifndef HDKN_HTTP_CONNECTION_MANAGER_HPP
#define HDKN_HTTP_CONNECTION_MANAGER_HPP

#include <hadouken/http/connection.hpp>
#include <set>

namespace hadouken
{
    namespace http
    {
        class connection_manager
        {
        public:
            connection_manager(const connection_manager&) = delete;
            connection_manager& operator=(const connection_manager&) = delete;

            connection_manager();

            // Add the specified connection to the manager and start it.
            void start(connection_ptr conn);

            // Stop the specified connection.
            void stop(connection_ptr conn);

            // Stop all connections
            void stop_all();

        private:
            std::set<connection_ptr> connections_;
        };
    }
}

#endif