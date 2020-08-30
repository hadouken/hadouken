#pragma once

#include <map>
#include <memory>

#include <libtorrent/session.hpp>
#include <libtorrent/settings_pack.hpp>

namespace lt = libtorrent;

namespace hadouken
{
    class session_manager
    {
    public:
        session_manager();

        lt::settings_pack settings();

    private:
        std::unique_ptr<lt::session> m_session;
    };
}
