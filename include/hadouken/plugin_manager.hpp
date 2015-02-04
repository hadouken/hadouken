#ifndef HDKN_PLUGIN_MANAGER_HPP
#define HDKN_PLUGIN_MANAGER_HPP

#include <boost/asio/io_service.hpp>
#include <hadouken/plugin.hpp>

namespace hadouken
{
    typedef plugin* create_t(hadouken::service_locator&);
    typedef void destroy_t(plugin*);

    class __declspec(dllexport) plugin_manager
    {
    public:
        plugin_manager(hadouken::service_locator& service_locator);
        ~plugin_manager();

        void load();
        void unload();

    private:
        void* open_dynamic_library(const std::string& file);
        void close_dynamic_library(void* handle);
        void* get_library_symbol(void* handle, const std::string& symbol);

        hadouken::service_locator& service_locator_;
        std::map<std::string, hadouken::plugin*> plugins_;
    };
}

#endif
