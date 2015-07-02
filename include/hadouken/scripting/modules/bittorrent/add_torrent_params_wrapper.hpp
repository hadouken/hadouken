#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_ADDTORRENTPARAMSWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_ADDTORRENTPARAMSWRAPPER_HPP

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

                private:
                    static int destruct(void* ctx);

                    static int get_flags(void* ctx);
                    static int get_resume_data(void* ctx);
                    static int get_save_path(void* ctx);
                    static int get_sparse_mode(void* ctx);
                    static int get_torrent(void* ctx);
                    static int get_url(void* ctx);
                    static int set_flags(void* ctx);
                    static int set_resume_data(void* ctx);
                    static int set_save_path(void* ctx);
                    static int set_sparse_mode(void* ctx);
                    static int set_torrent(void* ctx);
                    static int set_url(void* ctx);
                };
            }
        }
    }
}

#endif