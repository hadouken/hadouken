#ifndef HDKN_PLUGIN_MANAGER_HPP
#define HDKN_PLUGIN_MANAGER_HPP

#include <boost/asio/io_service.hpp>
#include <hadouken/service_locator.hpp>

namespace hadouken
{
    class __declspec(dllexport) plugin_manager
    {
    public:
        plugin_manager(hadouken::service_locator& service_locator);
        ~plugin_manager();

        void load();
        void unload();

    private:
        hadouken::service_locator& service_locator_;
        std::map<std::string, void*> plugins_;
    };
}

#endif
