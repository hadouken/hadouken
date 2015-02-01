#include "plugin_manager.hpp"

#include <iostream>
#include <windows.h>

#include <boost/log/trivial.hpp>
#include <hadouken/plugin.hpp>

using namespace hadouken;

void plugin_manager::load()
{
    typedef plugin* create_t(void);
    typedef void destroy_t(plugin*);

    BOOST_LOG_TRIVIAL(info) << "Loading plugin files.";

    HINSTANCE lib = LoadLibrary(L"folder_watcher.dll");

    create_t* creator = (create_t*) GetProcAddress(lib, "hdkn_create_folder_watcher");

    plugin* plugin = creator();
    plugin->load();

    destroy_t* destroyer = (destroy_t*)GetProcAddress(lib, "hdkn_destroy_folder_watcher");
    destroyer(plugin);

    FreeLibrary(lib);
}