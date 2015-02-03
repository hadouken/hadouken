#include "plugin_manager.hpp"

#include <iostream>
#include <windows.h>

#include <boost/asio/io_service.hpp>
#include <boost/log/trivial.hpp>
#include <hadouken/plugin.hpp>

using namespace hadouken;

plugin_manager::plugin_manager(hadouken::service_locator& service_locator)
    : service_locator_(service_locator)
{
    plugins_ = new std::map<std::string, hadouken::plugin*>();
}

plugin_manager::~plugin_manager()
{
    delete plugins_;
}

void plugin_manager::load()
{
    BOOST_LOG_TRIVIAL(info) << "Loading plugin files.";

    void* handle = open_dynamic_library("folder_watcher.dll");

    create_t* creator = (create_t*)get_library_symbol(handle, "hdkn_create_folder_watcher");

    plugin* plugin = creator(service_locator_);
    plugin->load();

    plugins_->insert(std::make_pair("folder_watcher", plugin));

    return;

    /*
    destroy_t* destroyer = (destroy_t*)GetProcAddress(lib, "hdkn_destroy_folder_watcher");
    destroyer(plugin);

    FreeLibrary(lib);
    */
}

void plugin_manager::unload()
{
    plugins_->at("folder_watcher")->unload();
}

void* plugin_manager::open_dynamic_library(const std::string& file)
{
    return LoadLibraryA(file.c_str());
}

void plugin_manager::close_dynamic_library(void* handle)
{
    FreeLibrary((HMODULE)handle);
}

void* plugin_manager::get_library_symbol(void* handle, const std::string& symbol)
{
    return GetProcAddress((HMODULE)handle, symbol.c_str());
}