#include <Hadouken/Extensions/ExtensionSubsystem.hpp>

#include <iostream>
#include <Hadouken/Extensions/Extension.hpp>

using namespace Hadouken::Extensions;
using namespace Poco::Util;

void ExtensionSubsystem::initialize(Application& app)
{
    std::string extName = "autoadd";
    extName += Poco::SharedLibrary::suffix();

    try
    {
        loader_.loadLibrary(extName);
    }
    catch (Poco::Exception& loaderException)
    {
        printf("%s\n", loaderException.what());
        return;
    }

    libs_.push_back(extName);

    ExtensionLoader::Iterator it(loader_.begin());
    ExtensionLoader::Iterator end(loader_.end());

    for (; it != end; ++it)
    {
        std::cout << "lib path: " << it->first << std::endl;

        ExtensionManifest::Iterator itMan(it->second->begin());
        ExtensionManifest::Iterator endMan(it->second->end());

        for (; itMan != endMan; ++itMan)
        {
            std::cout << itMan->name() << std::endl;

            itMan->create()->load();
        }
    }
}

void ExtensionSubsystem::uninitialize()
{
    for (auto lib : libs_)
    {
        loader_.unloadLibrary(lib);
    }
}

const char* ExtensionSubsystem::name() const
{
    return "Extensions";
}
