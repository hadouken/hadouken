#include "service_host.hpp"

#include <boost/asio.hpp>
#include <hadouken/logger.hpp>
#include <windows.h>

using namespace hadouken;

SERVICE_STATUS        g_ServiceStatus = { 0 };
SERVICE_STATUS_HANDLE g_StatusHandle = NULL;
HANDLE                g_ServiceStopEvent = INVALID_HANDLE_VALUE;

service_host* service_host::host_instance_ = 0;

int service_host::run(boost::asio::io_service& io_service)
{
    if (host_instance_ == 0)
    {
        host_instance_ = this;
        io_service_ = &io_service;
        signals_ = new boost::asio::signal_set(io_service);
    }

    SERVICE_TABLE_ENTRY ServiceTable[] =
    {
        { "Hadouken", service_main_entry },
        { NULL, NULL }
    };

    if (!StartServiceCtrlDispatcher(ServiceTable))
    {
        return GetLastError();
    }

    return 0;
}

void service_host::service_handler(DWORD dw_opcode)
{
    HDKN_LOG(trace) << "Service received opcode " << dw_opcode << ".";

    switch (dw_opcode)
    {
    case SERVICE_CONTROL_STOP:
        if (g_ServiceStatus.dwCurrentState != SERVICE_RUNNING)
        {
            break;
        }

        g_ServiceStatus.dwCheckPoint = 4;
        g_ServiceStatus.dwControlsAccepted = 0;
        g_ServiceStatus.dwCurrentState = SERVICE_STOP_PENDING;
        g_ServiceStatus.dwWin32ExitCode = 0;

        if (!SetServiceStatus(g_StatusHandle, &g_ServiceStatus))
        {
            OutputDebugString("SetServiceStatus returned error.");
        }

        signals_->cancel();

        break;
    }
}

void service_host::service_main(DWORD dw_argc, LPSTR* lpsz_argv)
{
    // register the service
    g_StatusHandle = RegisterServiceCtrlHandler("Hadouken", &service_host::service_handler_entry);

    if (g_StatusHandle == NULL)
    {
        return;
    }

    ZeroMemory(&g_ServiceStatus, sizeof(g_ServiceStatus));

    g_ServiceStatus.dwCheckPoint = 0;
    g_ServiceStatus.dwControlsAccepted = 0;
    g_ServiceStatus.dwCurrentState = SERVICE_START_PENDING;
    g_ServiceStatus.dwServiceSpecificExitCode = 0;
    g_ServiceStatus.dwServiceType = SERVICE_WIN32_OWN_PROCESS;
    g_ServiceStatus.dwWin32ExitCode = 0;

    if (!SetServiceStatus(g_StatusHandle, &g_ServiceStatus))
    {
        OutputDebugString("SetServiceStatus returned error.");
    }

    g_ServiceStopEvent = CreateEvent(NULL, TRUE, FALSE, NULL);

    if (g_ServiceStopEvent == NULL)
    {
        // Error creating event.
        g_ServiceStatus.dwCheckPoint = 1;
        g_ServiceStatus.dwControlsAccepted = 0;
        g_ServiceStatus.dwCurrentState = SERVICE_STOPPED;
        g_ServiceStatus.dwWin32ExitCode = GetLastError();

        if (!SetServiceStatus(g_StatusHandle, &g_ServiceStatus))
        {
            OutputDebugString("SetServiceStatus returned error.");
        }

        return;
    }

    // Tell the Windows overlords we are started.
    g_ServiceStatus.dwCheckPoint = 0;
    g_ServiceStatus.dwControlsAccepted = SERVICE_ACCEPT_STOP;
    g_ServiceStatus.dwCurrentState = SERVICE_RUNNING;
    g_ServiceStatus.dwWin32ExitCode = 0;

    if (!SetServiceStatus(g_StatusHandle, &g_ServiceStatus))
    {
        OutputDebugString("SetServiceStatus returned error.");
    }

    signals_->async_wait([this](const boost::system::error_code& error, int signal)
    {
        HDKN_LOG(info) << "Signal received: " << signal << ", error: " << error.message();
        io_service_->stop();
    });

    io_service_->run();

    CloseHandle(g_ServiceStopEvent);

    // Tell the service controller we are stopped
    g_ServiceStatus.dwCheckPoint = 3;
    g_ServiceStatus.dwControlsAccepted = 0;
    g_ServiceStatus.dwCurrentState = SERVICE_STOPPED;
    g_ServiceStatus.dwWin32ExitCode = 0;

    if (!SetServiceStatus(g_StatusHandle, &g_ServiceStatus))
    {
        OutputDebugString("SetServiceStatus returned error.");
    }
}
