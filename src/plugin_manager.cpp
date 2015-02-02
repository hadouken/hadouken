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
    typedef plugin* create_t(hadouken::service_locator&);
    typedef void destroy_t(plugin*);

    BOOST_LOG_TRIVIAL(info) << "Loading plugin files.";

    HINSTANCE lib = LoadLibrary(L"folder_watcher.dll");

    create_t* creator = (create_t*) GetProcAddress(lib, "hdkn_create_folder_watcher");

    plugin* plugin = creator(service_locator_);
    plugin->load();

    plugins_->insert(std::make_pair("folder_watcher", plugin));

    return;

    destroy_t* destroyer = (destroy_t*)GetProcAddress(lib, "hdkn_destroy_folder_watcher");
    destroyer(plugin);

    FreeLibrary(lib);
}

void plugin_manager::unload()
{
    plugins_->at("folder_watcher")->unload();
}