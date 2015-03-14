#ifdef WIN32

#include <Hadouken/Platform.hpp>

#include <Poco/Path.h>
#include <ShlObj.h>
#include <Windows.h>

using namespace Hadouken;

Poco::Path Platform::getApplicationDataPath()
{
    TCHAR szPath[MAX_PATH];
    HRESULT result = SHGetFolderPath(NULL, CSIDL_COMMON_APPDATA, NULL, 0, szPath);

    if (SUCCEEDED(result))
    {
        Poco::Path programData(szPath);
        programData.append("Hadouken");

        return programData;
    }

    return std::string();
}

Poco::Path Platform::getApplicationPath()
{
    TCHAR szPath[MAX_PATH];
    GetModuleFileName(NULL, szPath, sizeof(szPath));

    return Poco::Path(szPath).parent();
}

#endif
