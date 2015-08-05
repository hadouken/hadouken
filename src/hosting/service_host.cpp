#include <hadouken/hosting/service_host.hpp>

#include <boost/asio.hpp>
#include <boost/log/trivial.hpp>
#include <boost/make_shared.hpp>
#include <windows.h>

using namespace hadouken::hosting;
service_host* service_host::instance_ = 0;

int service_host::wait_for_exit(boost::shared_ptr<boost::asio::io_service> io)
{
    if (instance_ == 0)
    {
        instance_ = this;
        io_ = io;
        signals_ = boost::make_shared<boost::asio::signal_set>(*io);
    }

    SERVICE_TABLE_ENTRY tbl[] =
    {
        { L"Hadouken",  service_main_entry },
        { NULL, NULL }
    };

    if (!StartServiceCtrlDispatcher(tbl))
    {
        return GetLastError();
    }

    return 0;
}

void service_host::service_main(DWORD argc, LPWSTR* argv)
{
    status_handle_ = RegisterServiceCtrlHandlerEx(L"Hadouken", service_control_handler, this);

    if (status_handle_ == NULL)
    {
        BOOST_LOG_TRIVIAL(error) << "Could not register service control handler.";
        return;
    }

    signals_->async_wait([this](const boost::system::error_code& error, int signal)
    {
        io_->stop();
        set_status(SERVICE_STOPPED);
    });

    set_status(SERVICE_RUNNING);
    io_->run();
}

DWORD service_host::service_control_handler(DWORD control, DWORD event_type, LPVOID event_data, LPVOID context)
{
    service_host* host = static_cast<service_host*>(context);

    switch (control)
    {
    case SERVICE_CONTROL_SHUTDOWN:
    case SERVICE_CONTROL_STOP:
        host->set_status(SERVICE_STOP_PENDING);
        host->signals_->cancel();

        break;
    }

    return NO_ERROR;
}

void service_host::set_status(DWORD state)
{
    status_.dwCheckPoint = 0;
    status_.dwControlsAccepted = SERVICE_ACCEPT_STOP | SERVICE_ACCEPT_SHUTDOWN;
    status_.dwCurrentState = state;
    status_.dwServiceType = SERVICE_WIN32_OWN_PROCESS;
    status_.dwWin32ExitCode = 0;

    if (!SetServiceStatus(status_handle_, &status_))
    {
        BOOST_LOG_TRIVIAL(error) << "Could not set service status to: " << state;
    }
}

void service_host::service_main_entry(DWORD argc, LPWSTR* argv)
{
    if (instance_ == 0)
    {
        BOOST_LOG_TRIVIAL(fatal) << "Application instance is null.";
        return;
    }

    instance_->service_main(argc, argv);
}
