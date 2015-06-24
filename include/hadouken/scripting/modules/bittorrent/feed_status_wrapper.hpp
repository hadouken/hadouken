#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_FEEDSTATUSWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_FEEDSTATUSWRAPPER_HPP

namespace libtorrent
{
    struct feed_status;
}

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            namespace bittorrent
            {
                class feed_status_wrapper
                {
                public:
                    static void initialize(void* ctx, const libtorrent::feed_status& status);

                private:
                    static int finalize(void* ctx);

                    static int get_items(void* ctx);
                    static int get_last_update(void* ctx);
                    static int get_next_update(void* ctx);
                    static int get_description(void* ctx);
                    static int get_url(void* ctx);
                    static int get_title(void* ctx);
                    static int is_updating(void* ctx);
                };
            }
        }
    }
}

#endif
