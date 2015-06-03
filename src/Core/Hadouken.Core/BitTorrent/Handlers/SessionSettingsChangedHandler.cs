using System;
using System.Linq;
using Hadouken.Common.Data;
using Hadouken.Common.Messaging;
using Ragnar;

namespace Hadouken.Core.BitTorrent.Handlers {
    internal sealed class SessionSettingsChangedHandler : IMessageHandler<KeyValueChangedMessage> {
        private readonly IKeyValueStore _keyValueStore;
        private readonly ISession _session;

        public SessionSettingsChangedHandler(ISession session, IKeyValueStore keyValueStore) {
            if (session == null) {
                throw new ArgumentNullException("session");
            }
            if (keyValueStore == null) {
                throw new ArgumentNullException("keyValueStore");
            }
            this._session = session;
            this._keyValueStore = keyValueStore;
        }

        public void Handle(KeyValueChangedMessage message) {
            if (!message.Keys.Any(k => k.StartsWith("bt."))) {
                return;
            }

            using (var settings = this._session.QuerySettings()) {
                // net
                settings.ActiveDhtLimit = this._keyValueStore.Get<int>("bt.net.active_dht_limit");
                settings.ActiveDownloads = this._keyValueStore.Get<int>("bt.net.active_downloads");
                settings.ActiveLimit = this._keyValueStore.Get<int>("bt.net.active_limit");
                settings.ActiveLsdLimit = this._keyValueStore.Get<int>("bt.net.active_lsd_limit");
                settings.ActiveSeeds = this._keyValueStore.Get<int>("bt.net.active_seeds");
                settings.ActiveTrackerLimit = this._keyValueStore.Get<int>("bt.net.active_tracker_limit");
                settings.AllowI2PMixed = this._keyValueStore.Get<bool>("bt.net.allow_i2p_mixed");
                settings.AllowMultipleConnectionsPerIP =
                    this._keyValueStore.Get<bool>("bt.net.allow_multiple_connections_per_ip");
                settings.AllowedFastSetSize = this._keyValueStore.Get<int>("bt.net.allowed_fast_set_size");
                settings.AlwaysSendUserAgent = this._keyValueStore.Get<bool>("bt.net.always_send_user_agent");
                settings.AnnounceDoubleNat = this._keyValueStore.Get<bool>("bt.net.announce_double_nat");
                settings.AnnounceIP = this._keyValueStore.Get<string>("bt.net.announce_ip");
                settings.AnnounceToAllTiers = this._keyValueStore.Get<bool>("bt.net.announce_to_all_tiers");
                settings.AnnounceToAllTrackers = this._keyValueStore.Get<bool>("bt.net.announce_to_all_trackers");
                settings.AnonymousMode = this._keyValueStore.Get<bool>("bt.net.anonymous_mode");
                settings.ApplyIPFilterToTrackers = this._keyValueStore.Get<bool>("bt.net.apply_ip_filter_to_trackers");
                settings.AutoManageInterval = this._keyValueStore.Get<int>("bt.net.auto_manage_interval");
                settings.AutoManagePreferSeeds = this._keyValueStore.Get<bool>("bt.net.auto_manage_prefer_seeds");
                settings.AutoManageStartup = this._keyValueStore.Get<int>("bt.net.auto_manage_startup");
                settings.AutoScrapeInterval = this._keyValueStore.Get<int>("bt.net.auto_scrape_interval");
                settings.AutoScrapeMinInterval = this._keyValueStore.Get<int>("bt.net.auto_scrape_min_interval");
                settings.BanWebSeeds = this._keyValueStore.Get<bool>("bt.net.ban_web_seeds");
                settings.BroadcastLsd = this._keyValueStore.Get<bool>("bt.net.broadcast_lsd");
                settings.ChokingAlgorithm = this._keyValueStore.Get<int>("bt.net.choking_algorithm");
                settings.CloseRedundantConnections = this._keyValueStore.Get<bool>("bt.net.close_redundant_connections");
                settings.ConnectionSpeed = this._keyValueStore.Get<int>("bt.net.connection_speed");
                settings.ConnectionsLimit = this._keyValueStore.Get<int>("bt.net.connections_limit");
                settings.ConnectionsSlack = this._keyValueStore.Get<int>("bt.net.connections_slack");
                settings.DecreaseEstReciprocationRate =
                    this._keyValueStore.Get<int>("bt.net.decrease_est_reciprocation_rate");
                settings.DefaultEstReciprocationRate =
                    this._keyValueStore.Get<int>("bt.net.default_est_reciprocation_rate");
                settings.DhtAnnounceInterval = this._keyValueStore.Get<int>("bt.net.dht_announce_interval");
                settings.DhtUploadRateLimit = this._keyValueStore.Get<int>("bt.net.dht_upload_rate_limit");
                settings.DontCountSlowTorrents = this._keyValueStore.Get<bool>("bt.net.dont_count_slow_torrents");
                settings.DownloadRateLimit = this._keyValueStore.Get<int>("bt.net.download_rate_limit");
                settings.DropSkippedRequests = this._keyValueStore.Get<bool>("bt.net.drop_skipped_requests");
                settings.EnableIncomingTcp = this._keyValueStore.Get<bool>("bt.net.enable_incoming_tcp");
                settings.EnableIncomingUtp = this._keyValueStore.Get<bool>("bt.net.enable_incoming_ucp");
                settings.EnableOutgoingTcp = this._keyValueStore.Get<bool>("bt.net.enable_outgoing_tcp");
                settings.EnableOutgoingUtp = this._keyValueStore.Get<bool>("bt.net.enable_outgoing_utp");
                settings.ForceProxy = this._keyValueStore.Get<bool>("bt.net.force_proxy");
                settings.HalfOpenLimit = this._keyValueStore.Get<int>("bt.net.half_open_limit");
                settings.HandshakeTimeout = this._keyValueStore.Get<int>("bt.net.handshake_timeout");
                settings.IgnoreLimitsOnLocalNetwork =
                    this._keyValueStore.Get<bool>("bt.net.ignore_limits_on_local_network");
                settings.InactiveDownRate = this._keyValueStore.Get<int>("bt.net.inactive_down_rate");
                settings.InactiveUpRate = this._keyValueStore.Get<int>("bt.net.inactive_up_rate");
                settings.InactivityTimeout = this._keyValueStore.Get<int>("bt.net.inactivity_timeout");
                settings.IncomingStartsQueuedTorrents =
                    this._keyValueStore.Get<bool>("bt.net.incoming_starts_queued_torrents");
                settings.IncreaseEstReciprocationRate =
                    this._keyValueStore.Get<int>("bt.net.increase_est_reciprocation_rate");
                settings.InitialPickerThreshold = this._keyValueStore.Get<int>("bt.net.initial_picker_threshold");
                settings.LazyBitfields = this._keyValueStore.Get<bool>("bt.net.lazy_bitfields");
                settings.ListenQueueSize = this._keyValueStore.Get<int>("bt.net.listen_queue_size");
                settings.LocalDownloadRateLimit = this._keyValueStore.Get<int>("bt.net.local_download_rate_limit");
                settings.LocalServiceAnnounceInterval =
                    this._keyValueStore.Get<int>("bt.net.local_service_announce_interval");
                settings.LocalUploadRateLimit = this._keyValueStore.Get<int>("bt.net.local_upload_rate_limit");
                settings.MaxAllowedInRequestQueue = this._keyValueStore.Get<int>("bt.net.max_allowed_in_request_queue");
                settings.MaxFailCount = this._keyValueStore.Get<int>("bt.net.max_failcount");
                settings.MaxHttpReceiveBufferSize = this._keyValueStore.Get<int>("bt.net.max_http_recv_buffer_size");
                settings.MaxMetadataSize = this._keyValueStore.Get<int>("bt.net.max_metadata_size");
                settings.MaxOutRequestQueue = this._keyValueStore.Get<int>("bt.net.max_out_request_size");
                settings.MaxPausedPeerlistSize = this._keyValueStore.Get<int>("bt.net.max_paused_peerlist_size");
                settings.MaxPeerExchangePeers = this._keyValueStore.Get<int>("bt.net.max_pex_peers");
                settings.MaxPeerlistSize = this._keyValueStore.Get<int>("bt.net.max_peerlist_size");
                settings.MaxRejects = this._keyValueStore.Get<int>("bt.net.max_rejects");
                settings.MaxSuggestPieces = this._keyValueStore.Get<int>("bt.net.max_suggest_pieces");
                settings.MinAnnounceInterval = this._keyValueStore.Get<int>("bt.net.min_announce_interval");
                settings.MinReconnectTime = this._keyValueStore.Get<int>("bt.net.min_reconnect_time");
                settings.MixedModeAlgorithm = this._keyValueStore.Get<int>("bt.net.mixed_mode_algorithm");
                settings.NumOptimisticUnchokeSlots = this._keyValueStore.Get<int>("bt.net.num_optimistic_unchoke_slots");
                settings.NumWant = this._keyValueStore.Get<int>("bt.net.num_want");
                settings.OptimisticUnchokeInterval = this._keyValueStore.Get<int>("bt.net.optimistic_unchoke_interval");
                settings.PeerConnectTimeout = this._keyValueStore.Get<int>("bt.net.peer_connect_timeout");
                settings.PeerTimeout = this._keyValueStore.Get<int>("bt.net.peer_timeout");
                settings.PeerTurnover = this._keyValueStore.Get<float>("bt.net.peer_turnover");
                settings.PeerTurnoverCutoff = this._keyValueStore.Get<float>("bt.net.peer_turnover_cutoff");
                settings.PeerTurnoverInterval = this._keyValueStore.Get<int>("bt.net.peer_turnover_interval");
                settings.PieceTimeout = this._keyValueStore.Get<int>("bt.net.piece_timeout");
                settings.PreferUdpTrackers = this._keyValueStore.Get<bool>("bt.net.prefer_udp_trackers");
                settings.PrioritizePartialPieces = this._keyValueStore.Get<bool>("bt.net.prioritize_partial_pieces");
                settings.RateLimitIPOverhead = this._keyValueStore.Get<bool>("bt.net.rate_limit_ip_overhead");
                settings.RateLimitUtp = this._keyValueStore.Get<bool>("bt.net.rate_limit_utp");
                settings.ReceiveSocketBufferSize = this._keyValueStore.Get<int>("bt.net.recv_socket_buffer_size");
                settings.ReportReduntantBytes = this._keyValueStore.Get<bool>("bt.net.report_redundant_bytes");
                settings.ReportTrueDownloaded = this._keyValueStore.Get<bool>("bt.net.report_true_downloaded");
                settings.ReportWebSeedDownloads = this._keyValueStore.Get<bool>("bt.net.report_web_seed_downloads");
                settings.RequestQueueTime = this._keyValueStore.Get<int>("bt.net.request_queue_time");
                settings.RequestTimeout = this._keyValueStore.Get<int>("bt.net.request_timeout");
                settings.SeedChokingAlgorithm = this._keyValueStore.Get<int>("bt.net.seed_choking_algorithm");
                settings.SeedTimeLimit = this._keyValueStore.Get<int>("bt.net.seed_time_limit");
                settings.SeedTimeRatioLimit = this._keyValueStore.Get<float>("bt.net.seed_time_ratio_limit");
                settings.SeedingOutgoingConnections =
                    this._keyValueStore.Get<bool>("bt.net.seeding_outgoing_connections");
                settings.SeedingPieceQuota = this._keyValueStore.Get<int>("bt.net.seeding_piece_quota");
                settings.SendBufferLowWatermark = this._keyValueStore.Get<int>("bt.net.send_buffer_low_watermark");
                settings.SendBufferWatermark = this._keyValueStore.Get<int>("bt.net.send_buffer_watermark");
                settings.SendBufferWatermarkFactor = this._keyValueStore.Get<int>("bt.net.send_buffer_watermark_factor");
                settings.SendRedundantHave = this._keyValueStore.Get<bool>("bt.net.send_redundant_have");
                settings.SendSocketBufferSize = this._keyValueStore.Get<int>("bt.net.send_socket_buffer_size");
                settings.ShareModeTarget = this._keyValueStore.Get<int>("bt.net.share_mode_target");
                settings.ShareRatioLimit = this._keyValueStore.Get<float>("bt.net.share_ratio_target");
                settings.SmoothConnects = this._keyValueStore.Get<bool>("bt.net.smooth_connects");
                settings.SslListen = this._keyValueStore.Get<int>("bt.net.ssl_listen_port");
                settings.StopTrackerTimeout = this._keyValueStore.Get<int>("bt.net.stop_tracker_timeout");
                settings.StrictEndGameMode = this._keyValueStore.Get<bool>("bt.net.strict_end_game_mode");
                settings.StrictSuperSeeding = this._keyValueStore.Get<bool>("bt.net.strict_super_seeding");
                settings.SuggestMode = this._keyValueStore.Get<int>("bt.net.suggest_mode");
                settings.SupportMerkleTorrents = this._keyValueStore.Get<bool>("bt.net.support_merkle_torrents");
                settings.SupportShareMode = this._keyValueStore.Get<bool>("bt.net.support_share_mode");
                settings.TorrentConnectBoost = this._keyValueStore.Get<int>("bt.net.torrent_connect_boost");
                settings.TrackerBackoff = this._keyValueStore.Get<int>("bt.net.tracker_backoff");
                settings.TrackerCompletionTimeout = this._keyValueStore.Get<int>("bt.net.tracker_completion_timeout");
                settings.TrackerMaximumResponseLength =
                    this._keyValueStore.Get<int>("bt.net.tracker_maximum_response_length");
                settings.TrackerReceiveTimeout = this._keyValueStore.Get<int>("bt.net.tracker_receive_timeout");
                settings.UdpTrackerTokenExpiry = this._keyValueStore.Get<int>("bt.net.udp_tracker_token_expiry");
                settings.UnchokeInterval = this._keyValueStore.Get<int>("bt.net.unchoke_interval");
                settings.UnchokeSlotsLimit = this._keyValueStore.Get<int>("bt.net.unchoke_slots_limit");
                settings.UploadRateLimit = this._keyValueStore.Get<int>("bt.net.upload_rate_limit");
                settings.UpnpIgnoreNonRouters = this._keyValueStore.Get<bool>("bt.net.upnp_ignore_nonrouters");
                settings.UrlSeedPipelineSize = this._keyValueStore.Get<int>("bt.net.urlseed_pipeline_size");
                settings.UrlSeedTimeout = this._keyValueStore.Get<int>("bt.net.urlseed_timeout");
                settings.UrlSeedWaitRetry = this._keyValueStore.Get<int>("bt.net.urlseed_wait_retry");
                settings.UseDhtAsFallback = this._keyValueStore.Get<bool>("bt.net.use_dht_as_fallback");
                settings.UseParoleMode = this._keyValueStore.Get<bool>("bt.net.use_parole_mode");
                settings.UtpConnectTimeout = this._keyValueStore.Get<int>("bt.net.utp_connect_timeout");
                settings.UtpDynamicSocketBuffer = this._keyValueStore.Get<bool>("bt.net.utp_dynamic_sock_buf");
                settings.UtpFinResends = this._keyValueStore.Get<int>("bt.net.utp_fin_resends");
                settings.UtpGainFactor = this._keyValueStore.Get<int>("bt.net.utp_gain_factor");
                settings.UtpLossMultiplier = this._keyValueStore.Get<int>("bt.net.utp_loss_multiplier");
                settings.UtpMinTimeout = this._keyValueStore.Get<int>("bt.net.utp_min_timeout");
                settings.UtpNumResends = this._keyValueStore.Get<int>("bt.net.utp_num_resends");
                settings.UtpSynResends = this._keyValueStore.Get<int>("bt.net.utp_syn_resends");
                settings.UtpTargetDelay = this._keyValueStore.Get<int>("bt.net.utp_target_delay");
                settings.WholePiecesThreshold = this._keyValueStore.Get<int>("bt.net.whole_pieces_threshold");

                // other
                settings.DisableHashChecks = this._keyValueStore.Get<bool>("bt.disable_hash_checks");
                settings.IgnoreResumeTimestamps = this._keyValueStore.Get<bool>("bt.ignore_resume_timestamps");
                settings.NoRecheckIncompleteResume = this._keyValueStore.Get<bool>("bt.no_recheck_incomplete_resume");
                settings.TickInterval = this._keyValueStore.Get<int>("bt.tick_interval");

                // diskio
                settings.AllowReorderedDiskOperations =
                    this._keyValueStore.Get<bool>("bt.diskio.allow_reordered_disk_operations");
                settings.CacheBufferChunkSize = this._keyValueStore.Get<int>("bt.diskio.cache_buffer_chunk_size");
                settings.CacheExpiry = this._keyValueStore.Get<int>("bt.diskio.cache_expiry");
                settings.CacheSize = this._keyValueStore.Get<int>("bt.diskio.cache_size");
                settings.CoalesceReads = this._keyValueStore.Get<bool>("bt.diskio.coalesce_reads");
                settings.CoalesceWrites = this._keyValueStore.Get<bool>("bt.diskio.coalesce_writes");
                settings.DefaultCacheMinAge = this._keyValueStore.Get<int>("bt.diskio.default_cache_min_age");
                settings.DiskIOReadMode = this._keyValueStore.Get<int>("bt.diskio.read_mode");
                settings.DiskIOWriteMode = this._keyValueStore.Get<int>("bt.diskio.write_mode");
                settings.ExplicitCacheInterval = this._keyValueStore.Get<int>("bt.diskio.explicit_cache_interval");
                settings.ExplicitReadCache = this._keyValueStore.Get<bool>("bt.diskio.explicit_read_cache");
                settings.FileChecksDelayPerBlock = this._keyValueStore.Get<int>("bt.diskio.file_checks_delay_per_block");
                settings.FilePoolSize = this._keyValueStore.Get<int>("bt.diskio.file_pool_size");
                settings.GuidedReadCache = this._keyValueStore.Get<bool>("bt.diskio.guided_read_cache");
                settings.LockDiskCache = this._keyValueStore.Get<bool>("bt.diskio.lock_disk_cache");
                settings.LockFiles = this._keyValueStore.Get<bool>("bt.diskio.lock_files");
                settings.LowPrioDisk = this._keyValueStore.Get<bool>("bt.diskio.low_prio_disk");
                settings.MaxQueuedDiskBytes = this._keyValueStore.Get<int>("bt.diskio.max_queued_disk_bytes");
                settings.MaxQueuedDiskBytesLowWatermark =
                    this._keyValueStore.Get<int>("bt.diskio.max_queued_disk_bytes_low_watermark");
                settings.MaxSparseRegions = this._keyValueStore.Get<int>("bt.diskio.max_sparse_regions");
                settings.OptimisticDiskRetry = this._keyValueStore.Get<int>("bt.diskio.optimistic_disk_retry");
                settings.OptimizeHashingForSpeed = this._keyValueStore.Get<bool>("bt.diskio.optimize_hashing_for_speed");
                settings.ReadCacheLineSize = this._keyValueStore.Get<int>("bt.diskio.read_cache_line_size");
                settings.ReadJobEvery = this._keyValueStore.Get<int>("bt.diskio.read_job_every");
                settings.UseDiskCachePool = this._keyValueStore.Get<bool>("bt.diskio.use_disk_cache_pool");
                settings.UseDiskReadAhead = this._keyValueStore.Get<bool>("bt.diskio.use_disk_read_ahead");
                settings.UseReadCache = this._keyValueStore.Get<bool>("bt.diskio.use_read_cache");
                settings.VolatileReadCache = this._keyValueStore.Get<bool>("bt.diskio.volatile_read_cache");
                settings.WriteCacheLineSize = this._keyValueStore.Get<int>("bt.diskio.write_cache_line_size");

                this._session.SetSettings(settings);
            }
        }
    }
}