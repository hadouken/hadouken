#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_TORRENTSTATUSWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_TORRENTSTATUSWRAPPER_HPP

namespace libtorrent
{
    struct torrent_status;
}

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            namespace bittorrent
            {
                class torrent_status_wrapper
                {
                public:
                    static void initialize(void* ctx, const libtorrent::torrent_status& status);

                private:
                    static int finalize(void* ctx);

                    static int get_auto_managed(void* ctx);
                    static int get_error(void* ctx);
                    static int get_name(void* ctx);
                    static int get_eta(void* ctx);
                    static int get_added_time(void* ctx);
                    static int get_completed_time(void* ctx);
                    static int get_current_tracker(void* ctx);
                    static int get_progress(void* ctx);
                    static int get_save_path(void* ctx);
                    static int get_download_rate(void* ctx);
                    static int get_upload_rate(void* ctx);
                    static int get_downloaded_bytes(void* ctx);
                    static int get_downloaded_bytes_total(void* ctx);
                    static int get_failed_bytes(void* ctx);
                    static int get_redundant_bytes(void* ctx);
                    static int get_uploaded_bytes(void* ctx);
                    static int get_uploaded_bytes_total(void* ctx);
                    static int get_num_peers(void* ctx);
                    static int get_num_seeds(void* ctx);
                    static int get_num_complete(void* ctx);
                    static int get_num_incomplete(void* ctx);
                    static int get_ratio(void* ctx);
                    static int get_state(void* ctx);
                    static int get_handle(void* ctx);
                    static int has_metadata(void* ctx);
                    static int is_finished(void* ctx);
                    static int is_moving_storage(void* ctx);
                    static int is_paused(void* ctx);
                    static int is_seeding(void* ctx);
                    static int is_sequential_download(void* ctx);
                    static int need_save_resume(void* ctx);
                    static int get_list_seeds(void* ctx);
                    static int get_list_peers(void* ctx);
                    static int get_distributed_copies(void* ctx);
                    static int get_total_wanted(void* ctx);
                    static int get_seeding_time(void* ctx);
                };
            }
        }
    }
}

#endif
