#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_FEEDSETTINGSWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_FEEDSETTINGSWRAPPER_HPP

#define DUK_PROP(name) \
    static int get_##name(void* ctx); \
    static int set_##name(void* ctx);

namespace libtorrent
{
    struct feed_settings;
}

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            namespace bittorrent
            {
                class feed_settings_wrapper
                {
                public:
                    static int construct(void* ctx);

                private:
                    static int finalize(void* ctx);
                    
                    DUK_PROP(url);
                    DUK_PROP(ttl);
                    DUK_PROP(auto_download);
                };
            }
        }
    }
}

#endif
