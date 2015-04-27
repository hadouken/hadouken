var logger = require("logger").get("core");

var eventTranslator = {
    // libtorrent alert name       hadouken name
    "block_downloading_alert":     "peer.blockDownloading",
    "block_finished_alert":        "peer.blockFinished",
    "block_timeout_alert":         "peer.blockTimeout",
    "cache_flushed_alert":         "torrent.cacheFlushed",
    "dht_bootstrap_alert":         "dht.bootstrap",
    "dht_reply_alert":             "dht.reply",
    "external_ip_alert":           "externalAddress",
    "file_completed_alert":        "file.completed",
    "file_error_alert":            "file.error",
    "file_renamed_alert":          "file.renamed",
    "file_rename_failed_alert":    "file.renameFailed",
    "hash_failed_alert":           "torrent.hashFailed",
    "incoming_connection_alert":   "incomingConnection",
    "lsd_peer_alert":              "peer.lsd",
    "metadata_failed_alert":       "torrent.metadataFailed",
    "metadata_received_alert":     "torrent.metadataReceived",
    "peer_ban_alert":              "peer.ban",
    "peer_connect_alert":          "peer.connect",
    "peer_disconnected_alert":     "peer.disconnected",
    "peer_error_alert":            "peer.error",
    "peer_snubbed_alert":          "peer.snubbed",
    "peer_unsnubbed_alert":        "peer.unsnubbed",
    "performance_alert":           "torrent.performanceWarning",
    "piece_finished_alert":        "piece.finished",
    "request_dropped_alert":       "peer.requestDropped",
    "scrape_reply_alert":          "tracker.scrapeReply",
    "scrape_failed_alert":         "tracker.scrapeFailed",
    "state_changed_alert":         "torrent.stateChanged",
    "stats_alert":                 "torrent.stats",
    "storage_moved_alert":         "torrent.moved",
    "storage_moved_failed_alert":  "torrent.moveFailed",
    "tick":                        "tick",
    "torrent_added_alert":         "torrent.added",
    "torrent_checked_alert":       "torrent.checked",
    "torrent_deleted_alert":       "torrent.deleted",
    "torrent_delete_failed_alert": "torrent.deleteFailed",
    "torrent_error_alert":         "torrent.error",
    "torrent_finished_alert":      "torrent.finished",
    "torrent_need_cert_alert":     "torrent.needCertificate",
    "torrent_paused_alert":        "torrent.paused",
    "torrent_resumed_alert":       "torrent.resumed",
    "torrent_removed_alert":       "torrent.removed",
    "trackerid_alert":             "tracker.id",
    "tracker_announce_alert":      "tracker.announce",
    "tracker_error_alert":         "tracker.error",
    "tracker_reply_alert":         "tracker.reply",
    "tracker_warning_alert":       "tracker.warning",
    "unwanted_block_alert":        "peer.unwantedBlock",
    "url_seed_alert":              "torrent.urlSeed"
};

exports.getEventName = function(alertName) {
    if(eventTranslator[alertName]) {
        return eventTranslator[alertName];
    }

    logger.warn("Alert not translated: " + alertName);
    return alertName;
};
