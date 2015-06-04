#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_TORRENTHANDLEWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_TORRENTHANDLEWRAPPER_HPP

namespace libtorrent
{
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
                class torrent_handle_wrapper
                {
                public:
                    static void initialize(void* ctx, const libtorrent::torrent_handle& handle);

                private:
                    static int finalize(void* ctx);

                    static int clear_error(void* ctx);
                    static int flush_cache(void* ctx);
                    static int force_recheck(void* ctx);
                    static int get_info_hash(void* ctx);
                    static int get_file_progress(void* ctx);
                    static int get_peers(void* ctx);
                    static int get_queue_position(void* ctx);
                    static int get_status(void* ctx);
                    static int get_torrent_info(void* ctx);
                    static int get_trackers(void* ctx);
                    static int have_piece(void* ctx);
                    static int is_valid(void* ctx);
                    static int move_storage(void* ctx);
                    static int pause(void* ctx);
                    static int read_piece(void* ctx);
                    static int rename_file(void* ctx);
                    static int resume(void* ctx);
                    static int save_resume_data(void* ctx);
                    static int set_priority(void* ctx);

                    static int queue_bottom(void* ctx);
                    static int queue_down(void* ctx);
                    static int queue_top(void* ctx);
                    static int queue_up(void* ctx);

                    static int get_max_connections(void* ctx);
                    static int get_max_uploads(void* ctx);
                    static int get_resolve_countries(void* ctx);
                    static int get_sequential_download(void* ctx);
                    static int get_upload_limit(void* ctx);
                    static int get_upload_mode(void* ctx);
                    static int set_max_connections(void* ctx);
                    static int set_max_uploads(void* ctx);
                    static int set_resolve_countries(void* ctx);
                    static int set_sequential_download(void* ctx);
                    static int set_upload_limit(void* ctx);
                    static int set_upload_mode(void* ctx);
                };
            }
        }
    }
}

#endif
