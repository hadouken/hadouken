#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_ADDTORRENTPARAMSWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_ADDTORRENTPARAMSWRAPPER_HPP

namespace libtorrent
{
    struct add_torrent_params;
}

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            namespace bittorrent
            {
                class add_torrent_params_wrapper
                {
                public:
                    static int construct(void* ctx);

                    static void initialize(void* ctx, const libtorrent::add_torrent_params& params);

                private:
                    static int destruct(void* ctx);

                    static int get_data(void* ctx);
                    static int get_file_priorities(void* ctx);
                    static int get_flags(void* ctx);
                    static int get_resume_data(void* ctx);
                    static int get_save_path(void* ctx);
                    static int get_sparse_mode(void* ctx);
                    static int get_torrent(void* ctx);
                    static int get_trackers(void* ctx);
                    static int get_url(void* ctx);
                    static int set_data(void* ctx);
                    static int set_file_priorities(void* ctx);
                    static int set_flags(void* ctx);
                    static int set_resume_data(void* ctx);
                    static int set_save_path(void* ctx);
                    static int set_sparse_mode(void* ctx);
                    static int set_torrent(void* ctx);
                    static int set_trackers(void* ctx);
                    static int set_url(void* ctx);
                };
            }
        }
    }
}

#endif
