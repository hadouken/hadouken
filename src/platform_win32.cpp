#include <hadouken/platform.hpp>

#include <boost/log/trivial.hpp>
#include <windows.h>
#include <shellapi.h>
#include <shlobj.h>
#include <shlwapi.h>
#include <dbghelp.h>

using namespace hadouken;

static LONG WINAPI UnhandledException(LPEXCEPTION_POINTERS exceptionInfo)
{
    WCHAR tempPath[MAX_PATH];
    DWORD res = GetTempPath(ARRAYSIZE(tempPath), tempPath);

    if (res > ARRAYSIZE(tempPath) || res == 0)
    {
        BOOST_LOG_TRIVIAL(error) << "Could not get temporary path for minidump.";
        return EXCEPTION_CONTINUE_SEARCH;
    }

    WCHAR dmpPath[MAX_PATH + 2] = { 0 };
    PathCombine(dmpPath, tempPath, L"hadouken.dmp");

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

static DWORD relaunch_as_different_user(LPCWSTR path, LPCWSTR parameters)
{
    SHELLEXECUTEINFOW info = { 0 };
    info.cbSize = sizeof(SHELLEXECUTEINFO);
    info.lpVerb = L"runas";
    info.lpFile = path;
    info.lpParameters = parameters;
    info.nShow = SW_HIDE;

    if (!ShellExecuteExW(&info))
    {
        DWORD errorCode2 = GetLastError();

        return errorCode2;
    }
    else
    {
        return ERROR_SUCCESS;
    }
}

static std::wstring utf8_to_utf16(const std::string& input)
{
    std::wstring output;
    int size = MultiByteToWideChar(CP_UTF8, 0, input.c_str(), input.size(), NULL, 0);
    if (size > 0)
    {
        output.resize(size);

        size = MultiByteToWideChar(CP_UTF8, 0, input.c_str(), input.size(), &output[0], size);
    }

    output.resize(size);

    return output;
}

void platform::init()
{
    SetUnhandledExceptionFilter(UnhandledException);
}

void platform::install_service(bool relaunch_if_needed)
{
    WCHAR path[MAX_PATH] = { 0 };

    if (!GetModuleFileName(NULL, path, ARRAYSIZE(path)))
    {
        BOOST_LOG_TRIVIAL(error) << "Could not get filename.";
        return;
    }
    
    SC_HANDLE managerHandle = OpenSCManager(NULL, SERVICES_ACTIVE_DATABASE, SC_MANAGER_CONNECT | SC_MANAGER_CREATE_SERVICE);

    if (!managerHandle)
    {
        DWORD errorCode = GetLastError();
        if (errorCode == ERROR_ACCESS_DENIED)
        {
            DWORD errorCode2 = errorCode;
            if (relaunch_if_needed)
            {
                errorCode2 = relaunch_as_different_user(path, L"--install-service --runas");
            }

            if (errorCode2 != ERROR_SUCCESS)
            {
                BOOST_LOG_TRIVIAL(error) << "The current user account does not have permission to install a service.";

                return;
            }
            else
            {
                BOOST_LOG_TRIVIAL(info) << "Program relaunched as a different user";
                return;
            }
        }
        else
        {
            BOOST_LOG_TRIVIAL(error) << "Could not open Service Control Manager";
        }
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

void platform::uninstall_service(bool relaunch_if_needed)
{
    SC_HANDLE managerHandle = OpenSCManager(NULL, SERVICES_ACTIVE_DATABASE, SC_MANAGER_ALL_ACCESS);

    if (!managerHandle)
    {
        DWORD errorCode = GetLastError();
        if (errorCode == ERROR_ACCESS_DENIED)
        {
            WCHAR path[MAX_PATH] = { 0 };

            if (!GetModuleFileName(NULL, path, ARRAYSIZE(path)))
            {
                BOOST_LOG_TRIVIAL(error) << "Could not get filename.";
                return;
            }

            DWORD errorCode2 = errorCode;
            if (relaunch_if_needed)
            {
                errorCode2 = relaunch_as_different_user(path, L"--uninstall-service --runas");
            }

            if (errorCode2 != ERROR_SUCCESS)
            {
                BOOST_LOG_TRIVIAL(error) << "The current user account does not have permission to uninstall a service.";

                return;
            }
            else
            {
                BOOST_LOG_TRIVIAL(info) << "Program relaunched as a different user";
                return;
            }
        }
        else
        {
            BOOST_LOG_TRIVIAL(error) << "Could not open Service Control Manager";
        }

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
    WCHAR* szPath;
    HRESULT result = SHGetKnownFolderPath(FOLDERID_ProgramData, 0, NULL, &szPath);

    if (SUCCEEDED(result))
    {
        boost::filesystem::path programData(szPath);
        programData /= "Hadouken";

        CoTaskMemFree(szPath);

        return programData;
    }

    return std::string();
}

boost::filesystem::path platform::application_path()
{
    WCHAR szPath[MAX_PATH];
    GetModuleFileName(NULL, szPath, ARRAYSIZE(szPath));

    return boost::filesystem::path(szPath).parent_path();
}

boost::filesystem::path platform::get_current_directory()
{
    WCHAR buffer[MAX_PATH];
    GetCurrentDirectory(ARRAYSIZE(buffer), buffer);

    return boost::filesystem::path(buffer);
}

int platform::launch_process(const std::string& executable, const std::vector<std::string>& args)
{
    STARTUPINFO startInfo = { sizeof(STARTUPINFO) };
    PROCESS_INFORMATION procInfo;

    std::string cmd = executable;

    for (std::string arg : args)
    {
        cmd.append(" " + arg);
    }

    std::wstring p = utf8_to_utf16(cmd);

    if (!CreateProcess(NULL,
        &p[0],
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
