#ifdef WIN32

#include <hadouken/platform.hpp>

#include <atlstr.h>
#include <boost/log/trivial.hpp>
#include <windows.h>
#include <shlobj.h>
#include <shlwapi.h>
#include <dbghelp.h>
#include <tchar.h>

using namespace hadouken;

LONG WINAPI UnhandledException(LPEXCEPTION_POINTERS exceptionInfo)
{
    TCHAR tempPath[MAX_PATH];
    DWORD res = GetTempPath(MAX_PATH, tempPath);

    if (res > MAX_PATH || res == 0)
    {
        BOOST_LOG_TRIVIAL(error) << "Could not get temporary path for minidump.";
        return EXCEPTION_CONTINUE_SEARCH;
    }

    TCHAR dmpPath[MAX_PATH + 2] = { 0 };
    PathCombine(dmpPath, tempPath, TEXT("hadouken.dmp"));

    HANDLE file = CreateFile(dmpPath, GENERIC_READ | GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);

    if (file == NULL || file == INVALID_HANDLE_VALUE)
    {
        BOOST_LOG_TRIVIAL(error) << "Could not create minidump file.";
        return EXCEPTION_CONTINUE_SEARCH;
    }

    MINIDUMP_EXCEPTION_INFORMATION mei;
    mei.ThreadId = GetCurrentThreadId();
    mei.ExceptionPointers = exceptionInfo;
    mei.ClientPointers = FALSE;

    BOOL rv = MiniDumpWriteDump(GetCurrentProcess(), GetCurrentProcessId(), file, MiniDumpNormal, (exceptionInfo != 0) ? &mei : 0, 0, 0);
    CloseHandle(file);

    if (rv)
    {
        BOOST_LOG_TRIVIAL(fatal) << "Unhandled exception. Minidump written to '" << dmpPath << "'. Now crashing... :(";
    }
    else
    {
        BOOST_LOG_TRIVIAL(fatal) << "Unhandled exception. Could not write minidump file. Now crashing... :(";
    }

    return EXCEPTION_EXECUTE_HANDLER;
}

void platform::init()
{
    SetUnhandledExceptionFilter(UnhandledException);
}

void platform::install_service()
{
    wchar_t path[MAX_PATH] = { 0 };

    if (!GetModuleFileName(NULL, path, MAX_PATH))
    {
        BOOST_LOG_TRIVIAL(error) << "Could not get filename.";
        return;
    }
    
    SC_HANDLE managerHandle = OpenSCManager(NULL, SERVICES_ACTIVE_DATABASE, SC_MANAGER_CONNECT | SC_MANAGER_CREATE_SERVICE);

    if (!managerHandle)
    {
        BOOST_LOG_TRIVIAL(error) << "Could not open Service Control Manager";
        return;
    }

    std::wstring arg(path);
    arg.append(L" --daemon");

    SC_HANDLE serviceHandle = CreateService(managerHandle,
        L"Hadouken",
        L"Hadouken",
        SERVICE_QUERY_STATUS,
        SERVICE_WIN32_OWN_PROCESS,
        SERVICE_AUTO_START,
        SERVICE_ERROR_NORMAL,
        &arg[0],
        NULL,
        NULL,
        NULL,
        L"NT AUTHORITY\\LocalService",
        NULL);

    CloseServiceHandle(managerHandle);

    if (!serviceHandle)
    {
        BOOST_LOG_TRIVIAL(error) << "Could not create service, error: " << std::hex << GetLastError();
        return;
    }

    CloseServiceHandle(serviceHandle);

    BOOST_LOG_TRIVIAL(info) << "Service installed.";
}

void platform::uninstall_service()
{
    SC_HANDLE managerHandle = OpenSCManager(NULL, SERVICES_ACTIVE_DATABASE, SC_MANAGER_ALL_ACCESS);

    if (!managerHandle)
    {
        BOOST_LOG_TRIVIAL(error) << "Could not open Service Control Manager";
        return;
    }

    SC_HANDLE serviceHandle = OpenService(managerHandle, L"Hadouken", DELETE);

    if (!serviceHandle)
    {
        BOOST_LOG_TRIVIAL(error) << "Could not open service 'Hadouken', error: " << std::hex << GetLastError();
        CloseServiceHandle(managerHandle);
        return;
    }

    if (DeleteService(serviceHandle))
    {
        BOOST_LOG_TRIVIAL(info) << "Service uninstalled.";
    }
    else
    {
        BOOST_LOG_TRIVIAL(error) << "Could not delete service, error: " << GetLastError();
    }

    CloseServiceHandle(managerHandle);
    CloseServiceHandle(serviceHandle);
}

boost::filesystem::path platform::data_path()
{
    TCHAR szPath[MAX_PATH];
    HRESULT result = SHGetFolderPath(NULL, CSIDL_COMMON_APPDATA, NULL, 0, szPath);

    if (SUCCEEDED(result))
    {
        boost::filesystem::path programData(szPath);
        programData /= "Hadouken";

        return programData;
    }

    return std::string();
}

boost::filesystem::path platform::application_path()
{
    TCHAR szPath[MAX_PATH];
    GetModuleFileName(NULL, szPath, sizeof(szPath));

    return boost::filesystem::path(szPath).parent_path();
}

boost::filesystem::path platform::get_current_directory()
{
    TCHAR buffer[MAX_PATH];
    GetCurrentDirectory(MAX_PATH, buffer);

    return boost::filesystem::path(buffer);
}

int platform::launch_process(std::string executable, std::vector<std::string> args)
{
    STARTUPINFO startInfo;
    PROCESS_INFORMATION procInfo;

    std::string cmd = executable;

    for (std::string arg : args)
    {
        cmd.append(" " + arg);
    }

    TCHAR p[4096];
    _tcscpy_s(p, CA2T(cmd.c_str()));

    if (!CreateProcess(NULL,
        p,
        NULL,
        NULL,
        TRUE,
        CREATE_NEW_PROCESS_GROUP,
        NULL,
        NULL,
        &startInfo,
        &procInfo))
    {
        BOOST_LOG_TRIVIAL(error) << "CreateProcess failed: " << GetLastError();
        return -999;
    }

    WaitForSingleObject(procInfo.hProcess, INFINITE);

    DWORD exitCode;
    GetExitCodeProcess(procInfo.hProcess, &exitCode);

    CloseHandle(procInfo.hThread);
    CloseHandle(procInfo.hProcess);

    return exitCode;
}

#endif
