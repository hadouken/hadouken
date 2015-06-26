session.getStatus
=================

Overview
--------

Gets an object with status properties for the current BitTorrent session.

Example
~~~~~~~

.. code:: javascript

  {
    "method": "session.getStatus",
    "params": []
  }

Returns,

.. code:: javascript

  {
    "allowedUploadSlots": 8,
    "dhtDownloadedBytes": 0,
    "dhtDownloadRate": 0,
    "dhtGlobalNodes": 1,
    "dhtNodeCache": 0,
    "dhtNodes": 0,
    "dhtTorrents": 0,
    "dhtTotalAllocations": 0,
    "dhtUploadedBytes": 0,
    "dhtUploadRate": 0,
    "diskReadQueue": 0,
    "diskWriteQueue": 0,
    "downBandwidthBytesQueue": 0,
    "downBandwidthQueue": 0,
    "hasIncomingConnections": false,
    "ipOverheadDownloadedBytes": 0,
    "ipOverheadDownloadRate": 0,
    "ipOverheadUploadedBytes": 0,
    "ipOverheadUploadRate": 0,
    "isListening": true,
    "isPaused": false,
    "listenPort": 6882,
    "numPeers": 0,
    "numUnchoked": 0,
    "optimisticUnchokeCounter": 20,
    "payloadDownloadRate": 0,
    "payloadUploadedBytes": 0,
    "payloadUploadRate": 0,
    "sslListenPort": 4434,
    "trackerDownloadedBytes": 0,
    "trackerDownloadRate": 0,
    "trackerUploadedBytes": 0,
    "trackerUploadRate": 0,
    "totalDownloadedBytes": 0,
    "totalDownloadRate": 0,
    "totalFailedBytes": 0,
    "totalRedundantBytes": 0,
    "totalUploadedBytes": 0,
    "totalUploadRate": 0,
    "unchokeCounter": -71,
    "upBandwidthBytesQueue": 0,
    "upBandwidthQueue": 0
  }
