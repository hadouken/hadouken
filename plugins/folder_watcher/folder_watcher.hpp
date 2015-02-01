#ifndef FOLDER_WATCHER_HPP
#define FOLDER_WATCHER_HPP

#include <hadouken/plugin.hpp>

class folder_watcher : public hadouken::plugin
{
public:
    void load();
};

HADOUKEN_PLUGIN(folder_watcher)

#endif