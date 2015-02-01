#ifndef HDKN_TORRENT_ENGINE_HPP
#define HDKN_TORRENT_ENGINE_HPP

#include <libtorrent/session.hpp>

namespace hadouken
{
    class torrent_engine
    {
    public: 
        torrent_engine();
        ~torrent_engine();

        void load();
        void unload();

    protected:
        void load_state();
        void load_resume_data();
        
        void save_state();
        void save_resume_data();

    private:
        libtorrent::session* session_;
    };
}

#endif