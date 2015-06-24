#ifndef HADOUKEN_HOSTING_SERVICEHOST_HPP
#define HADOUKEN_HOSTING_SERVICEHOST_HPP

#include <hadouken/hosting/host.hpp>
#include <boost/asio.hpp>
#include <windows.h>

namespace hadouken
{
    namespace hosting
    {
        class service_host : public host
        {
        public:
            int wait_for_exit(boost::shared_ptr<boost::asio::io_service> io);

        protected:
            void service_main(DWORD argc, LPWSTR* argv);
            static DWORD WINAPI service_control_handler(DWORD control, DWORD event_type, LPVOID event_data, LPVOID context);

            void set_status(DWORD state);

        private:
            static void WINAPI service_main_entry(DWORD argc, LPWSTR* argv);

            static service_host* instance_;
            SERVICE_STATUS status_;
            SERVICE_STATUS_HANDLE status_handle_;

            boost::shared_ptr<boost::asio::signal_set> signals_;
            boost::shared_ptr<boost::asio::io_service> io_;
        };
    }
}

#endif
