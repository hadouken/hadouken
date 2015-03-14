#ifdef __unix__

#include <Hadouken/Platform.hpp>
#include <Poco/Path.h>

using namespace Hadouken;

Poco::Path Platform::getApplicationDataPath()
{
    // TODO: do something useful.
    return Poco::Path();
}

Poco::Path Platform::getApplicationPath()
{
    // TODO: do something useful.
    return Poco::Path();
}

#endif
