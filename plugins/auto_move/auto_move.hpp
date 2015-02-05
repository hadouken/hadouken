#ifndef AUTO_MOVE_HPP
#define AUTO_MOVE_HPP

#include <hadouken/plugin.hpp>
#include <hadouken/bittorrent/session.hpp>
#include <hadouken/bittorrent/torrent_handle.hpp>

class auto_move : public hadouken::plugin
{
public:
    class rule
    {
    public:
        std::string pattern;
        std::string input;
        std::string path;
    };

    auto_move(hadouken::service_locator& service_locator);

    void load();

private:
    void load_configuration();
    void torrent_finished(const hadouken::bittorrent::torrent_handle& handle);

    hadouken::bittorrent::session* sess_;
    std::vector<rule> rules_;
};

HADOUKEN_PLUGIN(auto_move)

#endif
