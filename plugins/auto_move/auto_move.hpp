#ifndef AUTO_MOVE_HPP
#define AUTO_MOVE_HPP

#include <hadouken/plugin.hpp>

class auto_move : public hadouken::plugin
{
public:
    auto_move(hadouken::service_locator& service_locator);

    void load();
};

HADOUKEN_PLUGIN(auto_move)

#endif