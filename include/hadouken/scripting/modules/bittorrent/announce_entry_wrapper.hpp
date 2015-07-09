#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_ANNOUNCEENTRYWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_ANNOUNCEENTRYWRAPPER_HPP

namespace libtorrent
{
    struct announce_entry;
}

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            namespace bittorrent
            {
                class announce_entry_wrapper
                {
                public:
                    static void initialize(void* ctx, libtorrent::announce_entry& entry);

                private:
                    static int finalize(void* ctx);

                    static int is_updating(void* ctx);
                    static int is_verified(void* ctx);
                    static int get_fail_count(void* ctx);
                    static int get_fail_limit(void* ctx);
                    static int get_last_error(void* ctx);
                    static int get_message(void* ctx);
                    static int get_min_announce(void* ctx);
                    static int get_next_announce(void* ctx);
                    static int get_scrape_complete(void* ctx);
                    static int get_scrape_downloaded(void* ctx);
                    static int get_scrape_incomplete(void* ctx);
                    static int get_source(void* ctx);
                    static int get_tier(void* ctx);
                    static int get_url(void* ctx);
                };
            }
        }
    }
}

#endif
