#include <Hadouken/Extensions/ExtensionSubsystem.hpp>

#include <iostream>
#include <Hadouken/Extensions/Extension.hpp>
#include <Poco/ClassLoader.h>
#include <Poco/Manifest.h>

using namespace Hadouken::Extensions;
using namespace Poco::Util;

typedef Poco::ClassLoader<Extension> ExtensionLoader;
typedef Poco::Manifest<Extension> ExtensionManifest;

void ExtensionSubsystem::initialize(Application& app)
{
    ExtensionLoader loader;

    std::string extName = "autoadd";
    extName += Poco::SharedLibrary::suffix();

    loader.loadLibrary(extName);

    ExtensionLoader::Iterator it(loader.begin());
    ExtensionLoader::Iterator end(loader.end());

    for (; it != end; ++it)
    {
        std::cout << "lib path: " << it->first << std::endl;

        ExtensionManifest::Iterator itMan(it->second->begin());
        ExtensionManifest::Iterator endMan(it->second->end());

        for (; itMan != endMan; ++itMan)
        {
            std::cout << itMan->name() << std::endl;
        }
    }
}

void ExtensionSubsystem::uninitialize()
{
}

const char* ExtensionSubsystem::name() const
{
    return "Extensions";
}
