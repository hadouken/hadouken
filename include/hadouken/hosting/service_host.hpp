#ifndef HDKN_SERVICE_HOST_HPP
#define HDKN_SERVICE_HOST_HPP

#ifndef WIN32
    #error This class should only be used on Windows.
#endif

#include <hadouken/hosting/host.hpp>

#include <hadouken/service_locator.hpp>
#include <windows.h>

namespace hadouken
{
    class service_host : public host
    {
    public:
        int run(boost::asio::io_service& io_service);

    protected:
        static service_host& instance()
        {
            return *host_instance_;
        }

        void service_main(DWORD dw_argc, LPSTR* lpsz_argv);

        void service_handler(DWORD dw_opcode);

    private:
        static void WINAPI service_main_entry(DWORD dw_argc, LPSTR* lpsz_argv)
        {
            host_instance_->service_main(dw_argc, lpsz_argv);
        }
        
        static void WINAPI service_handler_entry(DWORD dw_opcode)
        {
            host_instance_->service_handler(dw_opcode);
        }

        static service_host* host_instance_;
        boost::asio::io_service* io_service_;
        boost::asio::signal_set* signals_;
    };
}

#endif