using System;
using System.Linq;
using Hadouken.Common.Data;
using Hadouken.Common.Messaging;
using Ragnar;

namespace Hadouken.Core.BitTorrent.Handlers
{
    internal sealed class SessionSettingsChangedHandler : IMessageHandler<KeyValueChangedMessage>
    {
        private readonly ISession _session;
        private readonly IKeyValueStore _keyValueStore;

        public SessionSettingsChangedHandler(ISession session, IKeyValueStore keyValueStore)
        {
            if (session == null) throw new ArgumentNullException("session");
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            _session = session;
            _keyValueStore = keyValueStore;
        }

        public void Handle(KeyValueChangedMessage message)
        {
            if (!message.Keys.Any(k => k.StartsWith("bt."))) return;

            using (var settings = _session.QuerySettings())
            {
                // net
                settings.ActiveDhtLimit = _keyValueStore.Get<int>("bt.net.active_dht_limit");
                settings.ActiveDownloads = _keyValueStore.Get<int>("bt.net.active_downloads");
                settings.ActiveLimit = _keyValueStore.Get<int>("bt.net.active_limit");
                settings.ActiveLsdLimit = _keyValueStore.Get<int>("bt.net.active_lsd_limit");
                settings.ActiveSeeds = _keyValueStore.Get<int>("bt.net.active_seeds");
                settings.ActiveTrackerLimit = _keyValueStore.Get<int>("bt.net.active_tracker_limit");
                settings.AllowI2PMixed = _keyValueStore.Get<bool>("bt.net.allow_i2p_mixed");
                settings.AllowMultipleConnectionsPerIP = _keyValueStore.Get<bool>("bt.net.allow_multiple_connections_per_ip");
                settings.AllowedFastSetSize = _keyValueStore.Get<int>("bt.net.allowed_fast_set_size");
                settings.AlwaysSendUserAgent = _keyValueStore.Get<bool>("bt.net.always_send_user_agent");
                settings.AnnounceDoubleNat = _keyValueStore.Get<bool>("bt.net.announce_double_nat");
                settings.AnnounceIP = _keyValueStore.Get<string>("bt.net.announce_ip");
                settings.AnnounceToAllTiers = _keyValueStore.Get<bool>("bt.net.announce_to_all_tiers");
                settings.AnnounceToAllTrackers = _keyValueStore.Get<bool>("bt.net.announce_to_all_trackers");
                settings.AnonymousMode = _keyValueStore.Get<bool>("bt.net.anonymous_mode");
                settings.ApplyIPFilterToTrackers = _keyValueStore.Get<bool>("bt.net.apply_ip_filter_to_trackers");
                settings.AutoManageInterval = _keyValueStore.Get<int>("bt.net.auto_manage_interval");
                settings.AutoManagePreferSeeds = _keyValueStore.Get<bool>("bt.net.auto_manage_prefer_seeds");
                settings.AutoManageStartup = _keyValueStore.Get<int>("bt.net.auto_manage_startup");
                settings.AutoScrapeInterval = _keyValueStore.Get<int>("bt.net.auto_scrape_interval");
                settings.AutoScrapeMinInterval = _keyValueStore.Get<int>("bt.net.auto_scrape_min_interval");
                settings.BanWebSeeds = _keyValueStore.Get<bool>("bt.net.ban_web_seeds");
                settings.BroadcastLsd = _keyValueStore.Get<bool>("bt.net.broadcast_lsd");
                settings.ChokingAlgorithm = _keyValueStore.Get<int>("bt.net.choking_algorithm");
                settings.CloseRedundantConnections = _keyValueStore.Get<bool>("bt.net.close_redundant_connections");
                settings.ConnectionSpeed = _keyValueStore.Get<int>("bt.net.connection_speed");
                settings.ConnectionsLimit = _keyValueStore.Get<int>("bt.net.connections_limit");
                settings.ConnectionsSlack = _keyValueStore.Get<int>("bt.net.connections_slack");
                settings.DecreaseEstReciprocationRate = _keyValueStore.Get<int>("bt.net.decrease_est_reciprocation_rate");
                settings.DefaultEstReciprocationRate = _keyValueStore.Get<int>("bt.net.default_est_reciprocation_rate");
                settings.DhtAnnounceInterval = _keyValueStore.Get<int>("bt.net.dht_announce_interval");
                settings.DhtUploadRateLimit = _keyValueStore.Get<int>("bt.net.dht_upload_rate_limit");
                settings.DontCountSlowTorrents = _keyValueStore.Get<bool>("bt.net.dont_count_slow_torrents");
                settings.DownloadRateLimit = _keyValueStore.Get<int>("bt.net.download_rate_limit");
                settings.DropSkippedRequests = _keyValueStore.Get<bool>("bt.net.drop_skipped_requests");
                settings.EnableIncomingTcp = _keyValueStore.Get<bool>("bt.net.enable_incoming_tcp");
                settings.EnableIncomingUtp = _keyValueStore.Get<bool>("bt.net.enable_incoming_ucp");
                settings.EnableOutgoingTcp = _keyValueStore.Get<bool>("bt.net.enable_outgoing_tcp");
                settings.EnableOutgoingUtp = _keyValueStore.Get<bool>("bt.net.enable_outgoing_utp");
                settings.ForceProxy = _keyValueStore.Get<bool>("bt.net.force_proxy");
                settings.HalfOpenLimit = _keyValueStore.Get<int>("bt.net.half_open_limit");
                settings.HandshakeTimeout = _keyValueStore.Get<int>("bt.net.handshake_timeout");
                settings.IgnoreLimitsOnLocalNetwork = _keyValueStore.Get<bool>("bt.net.ignore_limits_on_local_network");
                settings.InactiveDownRate = _keyValueStore.Get<int>("bt.net.inactive_down_rate");
                settings.InactiveUpRate = _keyValueStore.Get<int>("bt.net.inactive_up_rate");
                settings.InactivityTimeout = _keyValueStore.Get<int>("bt.net.inactivity_timeout");
                settings.IncomingStartsQueuedTorrents = _keyValueStore.Get<bool>("bt.net.incoming_starts_queued_torrents");
                settings.IncreaseEstReciprocationRate = _keyValueStore.Get<int>("bt.net.increase_est_reciprocation_rate");
                settings.InitialPickerThreshold = _keyValueStore.Get<int>("bt.net.initial_picker_threshold");
                settings.LazyBitfields = _keyValueStore.Get<bool>("bt.net.lazy_bitfields");
                settings.ListenQueueSize = _keyValueStore.Get<int>("bt.net.listen_queue_size");
                settings.LocalDownloadRateLimit = _keyValueStore.Get<int>("bt.net.local_download_rate_limit");
                settings.LocalServiceAnnounceInterval = _keyValueStore.Get<int>("bt.net.local_service_announce_interval");
                settings.LocalUploadRateLimit = _keyValueStore.Get<int>("bt.net.local_upload_rate_limit");
                settings.MaxAllowedInRequestQueue = _keyValueStore.Get<int>("bt.net.max_allowed_in_request_queue");
                settings.MaxFailCount = _keyValueStore.Get<int>("bt.net.max_failcount");
                settings.MaxHttpReceiveBufferSize = _keyValueStore.Get<int>("bt.net.max_http_recv_buffer_size");
                settings.MaxMetadataSize = _keyValueStore.Get<int>("bt.net.max_metadata_size");
                settings.MaxOutRequestQueue = _keyValueStore.Get<int>("bt.net.max_out_request_size");
                settings.MaxPausedPeerlistSize = _keyValueStore.Get<int>("bt.net.max_paused_peerlist_size");
                settings.MaxPeerExchangePeers = _keyValueStore.Get<int>("bt.net.max_pex_peers");
                settings.MaxPeerlistSize = _keyValueStore.Get<int>("bt.net.max_peerlist_size");
                settings.MaxRejects = _keyValueStore.Get<int>("bt.net.max_rejects");
                settings.MaxSuggestPieces = _keyValueStore.Get<int>("bt.net.max_suggest_pieces");
                settings.MinAnnounceInterval = _keyValueStore.Get<int>("bt.net.min_announce_interval");
                settings.MinReconnectTime = _keyValueStore.Get<int>("bt.net.min_reconnect_time");
                settings.MixedModeAlgorithm = _keyValueStore.Get<int>("bt.net.mixed_mode_algorithm");
                settings.NumOptimisticUnchokeSlots = _keyValueStore.Get<int>("bt.net.num_optimistic_unchoke_slots");
                settings.NumWant = _keyValueStore.Get<int>("bt.net.num_want");
                settings.OptimisticUnchokeInterval = _keyValueStore.Get<int>("bt.net.optimistic_unchoke_interval");
                settings.PeerConnectTimeout = _keyValueStore.Get<int>("bt.net.peer_connect_timeout");
                settings.PeerTimeout = _keyValueStore.Get<int>("bt.net.peer_timeout");
                settings.PeerTurnover = _keyValueStore.Get<float>("bt.net.peer_turnover");
                settings.PeerTurnoverCutoff = _keyValueStore.Get<float>("bt.net.peer_turnover_cutoff");
                settings.PeerTurnoverInterval = _keyValueStore.Get<int>("bt.net.peer_turnover_interval");
                settings.PieceTimeout = _keyValueStore.Get<int>("bt.net.piece_timeout");
                settings.PreferUdpTrackers = _keyValueStore.Get<bool>("bt.net.prefer_udp_trackers");
                settings.PrioritizePartialPieces = _keyValueStore.Get<bool>("bt.net.prioritize_partial_pieces");
                settings.RateLimitIPOverhead = _keyValueStore.Get<bool>("bt.net.rate_limit_ip_overhead");
                settings.RateLimitUtp = _keyValueStore.Get<bool>("bt.net.rate_limit_utp");
                settings.ReceiveSocketBufferSize = _keyValueStore.Get<int>("bt.net.recv_socket_buffer_size");
                settings.ReportReduntantBytes = _keyValueStore.Get<bool>("bt.net.report_redundant_bytes");
                settings.ReportTrueDownloaded = _keyValueStore.Get<bool>("bt.net.report_true_downloaded");
                settings.ReportWebSeedDownloads = _keyValueStore.Get<bool>("bt.net.report_web_seed_downloads");
                settings.RequestQueueTime = _keyValueStore.Get<int>("bt.net.request_queue_time");
                settings.RequestTimeout = _keyValueStore.Get<int>("bt.net.request_timeout");
                settings.SeedChokingAlgorithm = _keyValueStore.Get<int>("bt.net.seed_choking_algorithm");
                settings.SeedTimeLimit = _keyValueStore.Get<int>("bt.net.seed_time_limit");
                settings.SeedTimeRatioLimit = _keyValueStore.Get<float>("bt.net.seed_time_ratio_limit");
                settings.SeedingOutgoingConnections = _keyValueStore.Get<bool>("bt.net.seeding_outgoing_connections");
                settings.SeedingPieceQuota = _keyValueStore.Get<int>("bt.net.seeding_piece_quota");
                settings.SendBufferLowWatermark = _keyValueStore.Get<int>("bt.net.send_buffer_low_watermark");
                settings.SendBufferWatermark = _keyValueStore.Get<int>("bt.net.send_buffer_watermark");
                settings.SendBufferWatermarkFactor = _keyValueStore.Get<int>("bt.net.send_buffer_watermark_factor");
                settings.SendRedundantHave = _keyValueStore.Get<bool>("bt.net.send_redundant_have");
                settings.SendSocketBufferSize = _keyValueStore.Get<int>("bt.net.send_socket_buffer_size");
                settings.ShareModeTarget = _keyValueStore.Get<int>("bt.net.share_mode_target");
                settings.ShareRatioLimit = _keyValueStore.Get<float>("bt.net.share_ratio_target");
                settings.SmoothConnects = _keyValueStore.Get<bool>("bt.net.smooth_connects");
                settings.SslListen = _keyValueStore.Get<int>("bt.net.ssl_listen_port");
                settings.StopTrackerTimeout = _keyValueStore.Get<int>("bt.net.stop_tracker_timeout");
                settings.StrictEndGameMode = _keyValueStore.Get<bool>("bt.net.strict_end_game_mode");
                settings.StrictSuperSeeding = _keyValueStore.Get<bool>("bt.net.strict_super_seeding");
                settings.SuggestMode = _keyValueStore.Get<int>("bt.net.suggest_mode");
                settings.SupportMerkleTorrents = _keyValueStore.Get<bool>("bt.net.support_merkle_torrents");
                settings.SupportShareMode = _keyValueStore.Get<bool>("bt.net.support_share_mode");
                settings.TorrentConnectBoost = _keyValueStore.Get<int>("bt.net.torrent_connect_boost");
                settings.TrackerBackoff = _keyValueStore.Get<int>("bt.net.tracker_backoff");
                settings.TrackerCompletionTimeout = _keyValueStore.Get<int>("bt.net.tracker_completion_timeout");
                settings.TrackerMaximumResponseLength = _keyValueStore.Get<int>("bt.net.tracker_maximum_response_length");
                settings.TrackerReceiveTimeout = _keyValueStore.Get<int>("bt.net.tracker_receive_timeout");
                settings.UdpTrackerTokenExpiry = _keyValueStore.Get<int>("bt.net.udp_tracker_token_expiry");
                settings.UnchokeInterval = _keyValueStore.Get<int>("bt.net.unchoke_interval");
                settings.UnchokeSlotsLimit = _keyValueStore.Get<int>("bt.net.unchoke_slots_limit");
                settings.UploadRateLimit = _keyValueStore.Get<int>("bt.net.upload_rate_limit");
                settings.UpnpIgnoreNonRouters = _keyValueStore.Get<bool>("bt.net.upnp_ignore_nonrouters");
                settings.UrlSeedPipelineSize = _keyValueStore.Get<int>("bt.net.urlseed_pipeline_size");
                settings.UrlSeedTimeout = _keyValueStore.Get<int>("bt.net.urlseed_timeout");
                settings.UrlSeedWaitRetry = _keyValueStore.Get<int>("bt.net.urlseed_wait_retry");
                settings.UseDhtAsFallback = _keyValueStore.Get<bool>("bt.net.use_dht_as_fallback");
                settings.UseParoleMode = _keyValueStore.Get<bool>("bt.net.use_parole_mode");
                settings.UtpConnectTimeout = _keyValueStore.Get<int>("bt.net.utp_connect_timeout");
                settings.UtpDynamicSocketBuffer = _keyValueStore.Get<bool>("bt.net.utp_dynamic_sock_buf");
                settings.UtpFinResends = _keyValueStore.Get<int>("bt.net.utp_fin_resends");
                settings.UtpGainFactor = _keyValueStore.Get<int>("bt.net.utp_gain_factor");
                settings.UtpLossMultiplier = _keyValueStore.Get<int>("bt.net.utp_loss_multiplier");
                settings.UtpMinTimeout = _keyValueStore.Get<int>("bt.net.utp_min_timeout");
                settings.UtpNumResends = _keyValueStore.Get<int>("bt.net.utp_num_resends");
                settings.UtpSynResends = _keyValueStore.Get<int>("bt.net.utp_syn_resends");
                settings.UtpTargetDelay = _keyValueStore.Get<int>("bt.net.utp_target_delay");
                settings.WholePiecesThreshold = _keyValueStore.Get<int>("bt.net.whole_pieces_threshold");

                // other
                settings.DisableHashChecks = _keyValueStore.Get<bool>("bt.disable_hash_checks");
                settings.IgnoreResumeTimestamps = _keyValueStore.Get<bool>("bt.ignore_resume_timestamps");
                settings.NoRecheckIncompleteResume = _keyValueStore.Get<bool>("bt.no_recheck_incomplete_resume");
                settings.TickInterval = _keyValueStore.Get<int>("bt.tick_interval");

                // diskio
                settings.AllowReorderedDiskOperations = _keyValueStore.Get<bool>("bt.diskio.allow_reordered_disk_operations");
                settings.CacheBufferChunkSize = _keyValueStore.Get<int>("bt.diskio.cache_buffer_chunk_size");
                settings.CacheExpiry = _keyValueStore.Get<int>("bt.diskio.cache_expiry");
                settings.CacheSize = _keyValueStore.Get<int>("bt.diskio.cache_size");
                settings.CoalesceReads = _keyValueStore.Get<bool>("bt.diskio.coalesce_reads");
                settings.CoalesceWrites = _keyValueStore.Get<bool>("bt.diskio.coalesce_writes");
                settings.DefaultCacheMinAge = _keyValueStore.Get<int>("bt.diskio.default_cache_min_age");
                settings.DiskIOReadMode = _keyValueStore.Get<int>("bt.diskio.read_mode");
                settings.DiskIOWriteMode = _keyValueStore.Get<int>("bt.diskio.write_mode");
                settings.ExplicitCacheInterval = _keyValueStore.Get<int>("bt.diskio.explicit_cache_interval");
                settings.ExplicitReadCache = _keyValueStore.Get<bool>("bt.diskio.explicit_read_cache");
                settings.FileChecksDelayPerBlock = _keyValueStore.Get<int>("bt.diskio.file_checks_delay_per_block");
                settings.FilePoolSize = _keyValueStore.Get<int>("bt.diskio.file_pool_size");
                settings.GuidedReadCache = _keyValueStore.Get<bool>("bt.diskio.guided_read_cache");
                settings.LockDiskCache = _keyValueStore.Get<bool>("bt.diskio.lock_disk_cache");
                settings.LockFiles = _keyValueStore.Get<bool>("bt.diskio.lock_files");
                settings.LowPrioDisk = _keyValueStore.Get<bool>("bt.diskio.low_prio_disk");
                settings.MaxQueuedDiskBytes = _keyValueStore.Get<int>("bt.diskio.max_queued_disk_bytes");
                settings.MaxQueuedDiskBytesLowWatermark = _keyValueStore.Get<int>("bt.diskio.max_queued_disk_bytes_low_watermark");
                settings.MaxSparseRegions = _keyValueStore.Get<int>("bt.diskio.max_sparse_regions");
                settings.OptimisticDiskRetry = _keyValueStore.Get<int>("bt.diskio.optimistic_disk_retry");
                settings.OptimizeHashingForSpeed = _keyValueStore.Get<bool>("bt.diskio.optimize_hashing_for_speed");
                settings.ReadCacheLineSize = _keyValueStore.Get<int>("bt.diskio.read_cache_line_size");
                settings.ReadJobEvery = _keyValueStore.Get<int>("bt.diskio.read_job_every");
                settings.UseDiskCachePool = _keyValueStore.Get<bool>("bt.diskio.use_disk_cache_pool");
                settings.UseDiskReadAhead = _keyValueStore.Get<bool>("bt.diskio.use_disk_read_ahead");
                settings.UseReadCache = _keyValueStore.Get<bool>("bt.diskio.use_read_cache");
                settings.VolatileReadCache = _keyValueStore.Get<bool>("bt.diskio.volatile_read_cache");
                settings.WriteCacheLineSize = _keyValueStore.Get<int>("bt.diskio.write_cache_line_size");

                _session.SetSettings(settings);
            }
        }
    }
}
