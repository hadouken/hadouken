#include <iostream>

#ifdef WIN32
    #include <windows.h>
#else
    #include <dlfcn.h>
#endif

#include <boost/asio/io_service.hpp>
#include <boost/filesystem.hpp>
#include <boost/log/trivial.hpp>
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/json_parser.hpp>

#include <hadouken/plugin_manager.hpp>
#include <hadouken/plugin.hpp>

using namespace hadouken;

plugin_manager::plugin_manager(hadouken::service_locator& service_locator)
    : service_locator_(service_locator)
{
}

plugin_manager::~plugin_manager()
{
}

void plugin_manager::load()
{
    namespace fs = boost::filesystem;

    fs::path plugins_config("config/plugins.json");
    
    if (!fs::exists(plugins_config))
    {
        BOOST_LOG_TRIVIAL(warning) << "No \"plugins.json\" found at " << fs::current_path() / "config";
        return;
    }

    boost::property_tree::ptree pt;
    boost::property_tree::read_json(plugins_config.string(), pt);

    std::string plugins_directory = pt.get<std::string>("plugins_directory");
    fs::path plugins_path(plugins_directory);

    if (!fs::exists(plugins_path))
    {
        BOOST_LOG_TRIVIAL(warning) << "Plugins directory does not exist.";
        return;
    }

    boost::property_tree::ptree plugins_list = pt.get_child("plugins");

    for (auto plugin_list_item : plugins_list)
    {
        std::string plugin_id = plugin_list_item.second.get<std::string>("");
        std::string plugin_file = plugin_id + ".dll";

        fs::path plugin_path = plugins_path / plugin_file;

        if (!fs::exists(plugin_path))
        {
            BOOST_LOG_TRIVIAL(warning) << "Plugin file " << plugin_path << " does not exist.";
            continue;
        }

        BOOST_LOG_TRIVIAL(info) << "Loading plugin " << plugin_id << " from file " << plugin_path;

        // open library and find create symbol
        void* handle = open_dynamic_library(plugin_path.string());
        create_t* creator = (create_t*)get_library_symbol(handle, "hdkn_create_" + plugin_id);

        // invoke create and load plugin
        plugin* plugin = creator(service_locator_);
        plugin->load();

        plugins_.insert(std::make_pair(plugin_id, plugin));
    }

    /*
    destroy_t* destroyer = (destroy_t*)GetProcAddress(lib, "hdkn_destroy_folder_watcher");
    destroyer(plugin);

    FreeLibrary(lib);
    */
}

void plugin_manager::unload()
{
    for (auto it : plugins_)
    {
        BOOST_LOG_TRIVIAL(info) << "Unloading plugin " << it.first;
        it.second->unload();
    }
}

void* plugin_manager::open_dynamic_library(const std::string& file)
{
    #ifdef WIN32
        return LoadLibraryA(file.c_str());
    #else
        return dlopen(file.c_str(), RTLD_LAZY | RTLD_GLOBAL);
    #endif
}

void plugin_manager::close_dynamic_library(void* handle)
{
    #ifdef WIN32
        FreeLibrary((HMODULE)handle);
    #else
        dlclose(handle);
    #endif
}

void* plugin_manager::get_library_symbol(void* handle, const std::string& symbol)
{
    #ifdef WIN32
        return GetProcAddress((HMODULE)handle, symbol.c_str());
    #else
        return dlsym(handle, symbol.c_str());
    #endif
}
