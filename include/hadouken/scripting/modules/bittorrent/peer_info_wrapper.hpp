#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_PEERINFOWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_PEERINFOWRAPPER_HPP

namespace libtorrent
{
    struct peer_info;
}

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            namespace bittorrent
            {
                class peer_info_wrapper
                {
                public:
                    static void initialize(void* ctx, libtorrent::peer_info& peer);

                private:
                    static int finalize(void* ctx);

                    static int get_peer_id(void* ctx);
                    static int get_flags(void* ctx);
                    static int get_country(void* ctx);
                    static int get_ip(void* ctx);
                    static int get_port(void* ctx);
                    static int get_connection_type(void* ctx);
                    static int get_client(void* ctx);
                    static int get_progress(void* ctx);
                    static int get_download_rate(void* ctx);
                    static int get_download_rate_remote(void* ctx);
                    static int get_upload_rate(void* ctx);
                    static int get_downloaded_bytes(void* ctx);
                    static int get_uploaded_bytes(void* ctx);
                    static int get_last_active(void* ctx);
                    static int get_last_request(void* ctx);
                    static int get_download_queue_length(void* ctx);
                    static int get_upload_queue_length(void* ctx);
                    static int get_num_hashfails(void* ctx);
                    static int get_source(void* ctx);
                };
            }
        }
    }
}

#endif
