#ifndef HADOUKEN_SCRIPTING_MODULES_BITTORRENT_SESSIONSETTINGSWRAPPER_HPP
#define HADOUKEN_SCRIPTING_MODULES_BITTORRENT_SESSIONSETTINGSWRAPPER_HPP

#define DUK_PROP(name) \
    static int get_##name(void* ctx); \
    static int set_##name(void* ctx);

namespace libtorrent
{
    struct session_settings;
}

namespace hadouken
{
    namespace scripting
    {
        namespace modules
        {
            namespace bittorrent
            {
                class session_settings_wrapper
                {
                public:
                    static void initialize(void* ctx, libtorrent::session_settings& settings);

                private:
                    static int finalize(void* ctx);

                    DUK_PROP(user_agent);
                    DUK_PROP(tracker_completion_timeout);
                    DUK_PROP(tracker_receive_timeout);
                    DUK_PROP(stop_tracker_timeout);
                    DUK_PROP(tracker_maximum_response_length);
                    DUK_PROP(piece_timeout);
                    DUK_PROP(request_timeout);
                    DUK_PROP(request_queue_time);
                    DUK_PROP(max_allowed_in_request_queue);
                    DUK_PROP(max_out_request_queue);
                    DUK_PROP(whole_pieces_threshold);
                    DUK_PROP(peer_timeout);
                    DUK_PROP(urlseed_timeout);
                    DUK_PROP(urlseed_pipeline_size);
                    DUK_PROP(allow_multiple_connections_per_ip);
                    DUK_PROP(download_rate_limit);
                    DUK_PROP(upload_rate_limit);
                    DUK_PROP(rate_limit_ip_overhead);
                    DUK_PROP(rate_limit_utp);
                    DUK_PROP(connections_limit);
                    DUK_PROP(mixed_mode_algorithm);
                    DUK_PROP(half_open_limit);
                    DUK_PROP(anonymous_mode);
                    DUK_PROP(active_downloads);
                    DUK_PROP(active_seeds);
                    DUK_PROP(active_limit);
                    DUK_PROP(dont_count_slow_torrents);
                };
            }
        }
    }
}

#endif
