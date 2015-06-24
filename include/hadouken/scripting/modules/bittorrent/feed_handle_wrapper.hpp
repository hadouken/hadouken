#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_FEEDHANDLEWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_FEEDHANDLEWRAPPER_HPP

namespace libtorrent
{
    struct feed_handle;
}

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            namespace bittorrent
            {
                class feed_handle_wrapper
                {
                public:
                    static void initialize(void* ctx, const libtorrent::feed_handle& handle);

                private:
                    static int finalize(void* ctx);

                    static int update_feed(void* ctx);
                    static int get_feed_status(void* ctx);
                    static int set_settings(void* ctx);
                    static int get_settings(void* ctx);
                };
            }
        }
    }
}

#endif
