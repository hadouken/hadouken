webui.getPeers
==============

Overview
--------

Returns the peers associated with the torrent identified by the info hash
given in the first parameter. The *peers* array always has n*2 items where
*n* is the info hash and *n+1* is the peers array.


Example
~~~~~~~

.. code:: javascript

  {
    "method": "webui.getPeers",
    "params": [ "<info hash>" ]
  }

Returns,

.. code:: javascript

  {
    "peers": [
      "<info hash>",
      [
        "SE", // two-letter ISO3166 country code
        "127.0.0.1", // ip address
        "", // reverse dns entry
        true, // utp
        6881, // port
        "libTorrent", // client name
        "HPL", // peer flags (based on uTorrent)
        870, // progress (divide by 1000 to get percentage)
        10, // download rate
        14, // upload rate
        3, // incoming requests in queue
        7, // outgoing requests in queue
        -1, // waited
        56, // uploaded bytes
        13, // downloaded bytes
        0, // hash fail count
        -1, // peer download
        -1, // max up
        -1, // max down
        -1, // queued
        100, // last active
        -1 // relevance
      ]
    ]
  }
