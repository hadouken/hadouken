#include "service_host.hpp"

#include <windows.h>

using namespace hadouken;

SERVICE_STATUS        g_ServiceStatus = { 0 };
SERVICE_STATUS_HANDLE g_StatusHandle = NULL;
HANDLE                g_ServiceStopEvent = INVALID_HANDLE_VALUE;

service_host* service_host::host_instance_ = 0;

int service_host::run()
{
    if (host_instance_ == 0)
    {
        host_instance_ = this;
    }

    SERVICE_TABLE_ENTRY ServiceTable[] =
    {
        { "Hadouken", (LPSERVICE_MAIN_FUNCTION)service_main_entry },
        { NULL, NULL }
    };

    if (!StartServiceCtrlDispatcher(ServiceTable))
    {
        return GetLastError();
    }

    return 0;
}

void service_host::service_main(DWORD dw_argc, LPSTR* lpsz_argv)
{
    DWORD Status = E_FAIL;

    // register the service
    g_StatusHandle = RegisterServiceCtrlHandler("Hadouken", ServiceControlHandler);

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

    // Here we should wait for the io service to finish.

    // Start a thread that will perform the main task.
    HANDLE thread = CreateThread(NULL, 0, ServiceWorkerThread, NULL, 0, NULL);

    // Wait for thread to exit
    WaitForSingleObject(thread, INFINITE);

    // Clean up
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

void WINAPI ServiceControlHandler(DWORD controlCode)
{

}

DWORD WINAPI ServiceWorkerThread(LPVOID lpParam)
{
    return ERROR_SUCCESS;
}