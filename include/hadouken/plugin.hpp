#ifndef HDKN_PLUGIN_HPP
#define HDKN_PLUGIN_HPP

#define HDKN_API __declspec(dllexport)

#define HADOUKEN_PLUGIN(ClassName) \
    extern "C" HDKN_API ClassName* hdkn_create_##ClassName(hadouken::service_locator& service_locator) { return new ClassName(service_locator); } \
    extern "C" HDKN_API void hdkn_destroy_##ClassName(ClassName* instance) { delete instance; }

#include <boost/asio.hpp>
#include <hadouken/service_locator.hpp>

namespace hadouken
{
    class plugin
    {
    public:
        explicit plugin(hadouken::service_locator& service_locator) {}

        virtual void load() = 0;
        virtual void unload() {}
    };
}

#endif
