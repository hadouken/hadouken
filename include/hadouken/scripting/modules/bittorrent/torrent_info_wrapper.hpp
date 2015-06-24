#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_TORRENTINFOWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_TORRENTINFOWRAPPER_HPP

namespace libtorrent
{
    class torrent_info;
    struct torrent_handle;
}

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            namespace bittorrent
            {
                class torrent_info_wrapper
                {
                public:
                    static int construct(void* ctx);
                    static void initialize(void* ctx, const libtorrent::torrent_info& info);

                private:
                    static int finalize(void* ctx);

                    static int get_files(void* ctx);
                    static int get_info_hash(void* ctx);
                    static int get_name(void* ctx);
                    static int get_total_size(void* ctx);
                };
            }
        }
    }
}

#endif
