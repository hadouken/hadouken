var session = require("bittorrent").session;

exports.rpc = {
    name: "session.getStatus",
    method: function() {
        var status = session.getStatus();

        return {
            allowedUploadSlots:        status.allowedUploadSlots,
            dhtDownloadedBytes:        status.dhtDownloadedBytes,
            dhtDownloadRate:           status.dhtDownloadRate,
            dhtGlobalNodes:            status.dhtGlobalNodes,
            dhtNodeCache:              status.dhtNodeCache,
            dhtNodes:                  status.dhtNodes,
            dhtTorrents:               status.dhtTorrents,
            dhtTotalAllocations:       status.dhtTotalAllocations,
            dhtUploadedBytes:          status.dhtUploadedBytes,
            dhtUploadRate:             status.dhtUploadRate,
            diskReadQueue:             status.diskReadQueue,
            diskWriteQueue:            status.diskWriteQueue,
            downBandwidthBytesQueue:   status.downBandwidthBytesQueue,
            downBandwidthQueue:        status.downBandwidthQueue,
            hasIncomingConnections:    status.hasIncomingConnections,
            ipOverheadDownloadedBytes: status.ipOverheadDownloadedBytes,
            ipOverheadDownloadRate:    status.ipOverheadDownloadRate,
            ipOverheadUploadedBytes:   status.ipOverheadUploadedBytes,
            ipOverheadUploadRate:      status.ipOverheadUploadRate,
            numPeers:                  status.numPeers,
            numUnchoked:               status.numUnchoked,
            optimisticUnchokeCounter:  status.optimisticUnchokeCounter,
            payloadDowloadedBytes:     status.payloadDowloadedBytes,
            payloadDownloadRate:       status.payloadDownloadRate,
            payloadUploadedBytes:      status.payloadUploadedBytes,
            payloadUploadRate:         status.payloadUploadRate,
            trackerDownloadedBytes:    status.trackerDownloadedBytes,
            trackerDownloadRate:       status.trackerDownloadRate,
            trackerUploadedBytes:      status.trackerUploadedBytes,
            trackerUploadRate:         status.trackerUploadRate,
            totalDownloadedBytes:      status.totalDownloadedBytes,
            totalDownloadRate:         status.totalDownloadRate,
            totalFailedBytes:          status.totalFailedBytes,
            totalRedundantBytes:       status.totalRedundantBytes,
            totalUploadedBytes:        status.totalUploadedBytes,
            totalUploadRate:           status.totalUploadRate,
            unchokeCounter:            status.unchokeCounter,
            upBandwidthBytesQueue:     status.upBandwidthBytesQueue,
            upBandwidthQueue:          status.upBandwidthQueue
        };
    }
};
