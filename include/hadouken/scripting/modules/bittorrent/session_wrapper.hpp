#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_SESSIONWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_SESSIONWRAPPER_HPP

namespace libtorrent
{
    class session;
}

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            namespace bittorrent
            {
                class session_wrapper
                {
                public:
                    static void initialize(void* ctx, libtorrent::session& session);

                private:
                    static int add_dht_router(void* ctx);
                    static int add_feed(void* ctx);
                    static int add_torrent(void* ctx);
                    static int find_torrent(void* ctx);
                    static int get_listen_port(void* ctx);
                    static int get_ssl_listen_port(void* ctx);
                    static int get_alerts(void* ctx);
                    static int get_feeds(void* ctx);
                    static int get_settings(void* ctx);
                    static int get_status(void* ctx);
                    static int get_torrents(void* ctx);
                    static int is_listening(void* ctx);
                    static int is_paused(void* ctx);
                    static int listen_on(void* ctx);
                    static int load_country_db(void* ctx);
                    static int load_state(void* ctx);
                    static int pause(void* ctx);
                    static int post_torrent_updates(void* ctx);
                    static int remove_torrent(void* ctx);
                    static int resume(void* ctx);
                    static int save_state(void* ctx);
                    static int start_dht(void* ctx);
                    static int start_nat_pmp(void* ctx);
                    static int start_upnp(void* ctx);
                    static int wait_for_alert(void* ctx);
                };
            }
        }
    }
}

#endif
