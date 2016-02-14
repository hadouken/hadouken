#include <hadouken/hosting/service_host.hpp>

#include <boost/asio.hpp>
#include <boost/log/trivial.hpp>
#include <boost/make_shared.hpp>
#include <windows.h>

using namespace hadouken::hosting;
service_host* service_host::instance_ = 0;

service_host::service_host()
    : status_handle_(NULL)
    , startup_event_(NULL)
    , signals_()
    , io_()
    , host()
{
}

int service_host::initialization_start(boost::shared_ptr<boost::asio::io_service> io)
{
    if (instance_ == 0)
    {
        instance_ = this;
        io_ = io;
        signals_ = boost::make_shared<boost::asio::signal_set>(*io);
    }

    startup_event_ = CreateEvent(NULL, TRUE, FALSE, NULL);
    if (startup_event_ == NULL)
    {
        BOOST_LOG_TRIVIAL(error) << "Service startup handle creation failed " << GetLastError();
        return ERROR_INVALID_HANDLE;
    }

    HANDLE thread = CreateThread(NULL, 0, service_startup_threadproc, this, 0, NULL);
    if (thread == NULL)
    {
        BOOST_LOG_TRIVIAL(error) << "Service thread creation failed " << GetLastError();
        CloseHandle(startup_event_);
        return ERROR_INVALID_HANDLE;
    }

    CloseHandle(thread);

    if (WaitForSingleObject(startup_event_, 30000) != WAIT_OBJECT_0)
    {
        BOOST_LOG_TRIVIAL(error) << "Service startup timed out";
        CloseHandle(startup_event_);
        return WAIT_TIMEOUT;
    }

    CloseHandle(startup_event_);

    return NO_ERROR;
}

int service_host::initialization_complete(int success_code)
{
    if (success_code == NO_ERROR)
    {
        BOOST_LOG_TRIVIAL(info) << "Service running";
        set_status(SERVICE_RUNNING);
    }
    else
    {
        BOOST_LOG_TRIVIAL(info) << "Startup failed, service stopping";
        set_status(SERVICE_STOP_PENDING);
    }

    return NO_ERROR;
}

int service_host::wait_for_exit()
{
    signals_->async_wait([this](const boost::system::error_code& error, int signal)
    {
        BOOST_LOG_TRIVIAL(info) << "Service stopping: " << error.message();
        io_->stop();
        set_status(SERVICE_STOP_PENDING);
    });

    io_->run();

    return NO_ERROR;
}

int service_host::shutdown_start()
{
    BOOST_LOG_TRIVIAL(info) << "Service stopping";
   set_status(SERVICE_STOP_PENDING);

   return NO_ERROR;
}

int service_host::shutdown_complete(int success_code)
{
    BOOST_LOG_TRIVIAL(info) << "Service stopped";
    set_status(SERVICE_STOPPED, static_cast<DWORD>(success_code));

    return NO_ERROR;
}

void service_host::service_main(DWORD argc, LPWSTR* argv)
{
    status_handle_ = RegisterServiceCtrlHandlerEx(L"Hadouken", service_control_handler, this);

    if (status_handle_ == NULL)
    {
        BOOST_LOG_TRIVIAL(error) << "Could not register service control handler.";
        return;
    }

    set_status(SERVICE_START_PENDING);

    SetEvent(startup_event_);
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

DWORD service_host::service_startup_threadproc(LPVOID lpParameter)
{
    SERVICE_TABLE_ENTRY tbl[] =
    {
        { L"Hadouken", service_main_entry },
        { NULL, NULL }
    };

    BOOST_LOG_TRIVIAL(info) << "Service initialization start";

    if (!StartServiceCtrlDispatcher(tbl))
    {
        BOOST_LOG_TRIVIAL(error) << "Service failed to start " << GetLastError();
        return GetLastError();
    }

    return NO_ERROR;
}

void service_host::set_status(DWORD state, DWORD exit_code)
{
    if (status_handle_ == NULL)
    {
        return;
    }

    if (exit_code == EXIT_FAILURE)
    {
        exit_code = ERROR_FATAL_APP_EXIT;
    }

    SERVICE_STATUS status = { 0 };
    status.dwControlsAccepted = (state == SERVICE_START_PENDING) ? 0 : SERVICE_ACCEPT_STOP | SERVICE_ACCEPT_SHUTDOWN;
    status.dwCurrentState = state;
    status.dwServiceType = SERVICE_WIN32_OWN_PROCESS;
    status.dwWin32ExitCode = exit_code;

    if (!SetServiceStatus(status_handle_, &status))
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
