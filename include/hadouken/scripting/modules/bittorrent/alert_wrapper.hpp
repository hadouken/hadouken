#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_ALERTWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_ALERTWRAPPER_HPP

namespace libtorrent
{
    class alert;
    struct add_torrent_alert;
    struct block_downloading_alert;
    struct block_finished_alert;
    struct block_timeout_alert;
    struct cache_flushed_alert;
    struct dht_bootstrap_alert;
    struct dht_get_peers_alert;
    struct dht_reply_alert;
    struct external_ip_alert;
    struct fastresume_rejected_alert;
    struct file_completed_alert;
    struct file_error_alert;
    struct file_renamed_alert;
    struct file_rename_failed_alert;
    struct hash_failed_alert;
    struct incoming_connection_alert;
    struct invalid_request_alert;
    struct i2p_alert;
    struct listen_failed_alert;
    struct listen_succeeded_alert;
    struct lsd_peer_alert;
    struct metadata_failed_alert;
    struct metadata_received_alert;
    struct peer_alert;
    struct peer_ban_alert;
    struct peer_blocked_alert;
    struct peer_connect_alert;
    struct peer_disconnected_alert;
    struct peer_error_alert;
    struct peer_snubbed_alert;
    struct peer_unsnubbed_alert;
    struct performance_alert;
    struct piece_finished_alert;
    struct portmap_alert;
    struct portmap_error_alert;
    struct portmap_log_alert;
    struct read_piece_alert;
    struct request_dropped_alert;
    struct rss_alert;
    struct save_resume_data_alert;
    struct save_resume_data_failed_alert;
    struct scrape_failed_alert;
    struct scrape_reply_alert;
    struct state_changed_alert;
    struct stats_alert;
    struct storage_moved_alert;
    struct storage_moved_failed_alert;
    struct torrent_alert;
    struct torrent_checked_alert;
    struct torrent_deleted_alert;
    struct torrent_delete_failed_alert;
    struct torrent_error_alert;
    struct torrent_finished_alert;
    struct torrent_need_cert_alert;
    struct torrent_paused_alert;
    struct torrent_removed_alert;
    struct torrent_resumed_alert;
    struct torrent_update_alert;
    struct tracker_alert;
    struct trackerid_alert;
    struct tracker_announce_alert;
    struct tracker_error_alert;
    struct tracker_reply_alert;
    struct tracker_warning_alert;
    struct udp_error_alert;
    struct unwanted_block_alert;
    struct url_seed_alert;
}

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            namespace bittorrent
            {
                class alert_wrapper
                {
                public:
                    static void construct(void* ctx, libtorrent::alert* alert);

                    static int initialize(void* ctx, libtorrent::alert* alert);
                    static int initialize(void* ctx, libtorrent::torrent_alert* alert);
                    static int initialize(void* ctx, libtorrent::peer_alert* alert);
                    static int initialize(void* ctx, libtorrent::tracker_alert* alert);
                    static int initialize(void* ctx, libtorrent::save_resume_data_alert* alert);
                    static int initialize(void* ctx, libtorrent::torrent_removed_alert* alert);
                    static int initialize(void* ctx, libtorrent::read_piece_alert* alert);
                    static int initialize(void* ctx, libtorrent::file_completed_alert* alert);
                    static int initialize(void* ctx, libtorrent::file_renamed_alert* alert);
                    static int initialize(void* ctx, libtorrent::file_rename_failed_alert* alert);
                    static int initialize(void* ctx, libtorrent::performance_alert* alert);
                    static int initialize(void* ctx, libtorrent::state_changed_alert* alert);
                    static int initialize(void* ctx, libtorrent::tracker_error_alert* alert);
                    static int initialize(void* ctx, libtorrent::tracker_warning_alert* alert);
                    static int initialize(void* ctx, libtorrent::scrape_reply_alert* alert);
                    static int initialize(void* ctx, libtorrent::scrape_failed_alert* alert);
                    static int initialize(void* ctx, libtorrent::tracker_reply_alert* alert);
                    static int initialize(void* ctx, libtorrent::dht_reply_alert* alert);
                    static int initialize(void* ctx, libtorrent::tracker_announce_alert* alert);
                    static int initialize(void* ctx, libtorrent::hash_failed_alert* alert);
                    static int initialize(void* ctx, libtorrent::peer_ban_alert* alert);
                    static int initialize(void* ctx, libtorrent::peer_unsnubbed_alert* alert);
                    static int initialize(void* ctx, libtorrent::peer_snubbed_alert* alert);
                    static int initialize(void* ctx, libtorrent::peer_error_alert* alert);
                    static int initialize(void* ctx, libtorrent::peer_connect_alert* alert);
                    static int initialize(void* ctx, libtorrent::peer_disconnected_alert* alert);
                    static int initialize(void* ctx, libtorrent::invalid_request_alert* alert);
                    static int initialize(void* ctx, libtorrent::torrent_finished_alert* alert);
                    static int initialize(void* ctx, libtorrent::piece_finished_alert* alert);
                    static int initialize(void* ctx, libtorrent::request_dropped_alert* alert);
                    static int initialize(void* ctx, libtorrent::block_timeout_alert* alert);
                    static int initialize(void* ctx, libtorrent::block_finished_alert* alert);
                    static int initialize(void* ctx, libtorrent::block_downloading_alert* alert);
                    static int initialize(void* ctx, libtorrent::unwanted_block_alert* alert);
                    static int initialize(void* ctx, libtorrent::storage_moved_alert* alert);
                    static int initialize(void* ctx, libtorrent::storage_moved_failed_alert* alert);
                    static int initialize(void* ctx, libtorrent::torrent_deleted_alert* alert);
                    static int initialize(void* ctx, libtorrent::torrent_delete_failed_alert* alert);
                    static int initialize(void* ctx, libtorrent::save_resume_data_failed_alert* alert);
                    static int initialize(void* ctx, libtorrent::torrent_paused_alert* alert);
                    static int initialize(void* ctx, libtorrent::torrent_resumed_alert* alert);
                    static int initialize(void* ctx, libtorrent::torrent_checked_alert* alert);
                    static int initialize(void* ctx, libtorrent::url_seed_alert* alert);
                    static int initialize(void* ctx, libtorrent::file_error_alert* alert);
                    static int initialize(void* ctx, libtorrent::metadata_failed_alert* alert);
                    static int initialize(void* ctx, libtorrent::metadata_received_alert* alert);
                    static int initialize(void* ctx, libtorrent::udp_error_alert* alert);
                    static int initialize(void* ctx, libtorrent::external_ip_alert* alert);
                    static int initialize(void* ctx, libtorrent::listen_failed_alert* alert);
                    static int initialize(void* ctx, libtorrent::listen_succeeded_alert* alert);
                    static int initialize(void* ctx, libtorrent::portmap_error_alert* alert);
                    static int initialize(void* ctx, libtorrent::portmap_alert* alert);
                    static int initialize(void* ctx, libtorrent::portmap_log_alert* alert);
                    static int initialize(void* ctx, libtorrent::fastresume_rejected_alert* alert);
                    static int initialize(void* ctx, libtorrent::peer_blocked_alert* alert);
                    static int initialize(void* ctx, libtorrent::dht_get_peers_alert* alert);
                    static int initialize(void* ctx, libtorrent::stats_alert* alert);
                    static int initialize(void* ctx, libtorrent::cache_flushed_alert* alert);
                    static int initialize(void* ctx, libtorrent::lsd_peer_alert* alert);
                    static int initialize(void* ctx, libtorrent::trackerid_alert* alert);
                    static int initialize(void* ctx, libtorrent::dht_bootstrap_alert* alert);
                    static int initialize(void* ctx, libtorrent::rss_alert* alert);
                    static int initialize(void* ctx, libtorrent::torrent_error_alert* alert);
                    static int initialize(void* ctx, libtorrent::torrent_need_cert_alert* alert);
                    static int initialize(void* ctx, libtorrent::incoming_connection_alert* alert);
                    static int initialize(void* ctx, libtorrent::add_torrent_alert* alert);
                    static int initialize(void* ctx, libtorrent::torrent_update_alert* alert);
                    static int initialize(void* ctx, libtorrent::i2p_alert* alert);

                private:
                    static int finalize(void* ctx);
                };
            }
        }
    }
}

#endif
