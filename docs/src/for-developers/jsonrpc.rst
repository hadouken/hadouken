The JSONRPC API
===============

JSONRPC is a standard defined at jsonrpc.org. It is a simple RPC protocol and
most languages has at least a way to encode/decode JSON data and send it over
HTTP, which makes it a safe bet for a remote API.

The JSONRPC API can be accessed at :file:`http://localhost:7070/api`. Adjust
accordingly.

.. note:: All examples are shown with only a method name and request/response
          data - the JSONRPC object container has been left out.

Methods
-------

.. toctree::
   :maxdepth: 1

   jsonrpc-methods/config-get
   jsonrpc-methods/core-getSystemInfo
   jsonrpc-methods/rss-getFeedItems
   jsonrpc-methods/rss-getFeeds
   jsonrpc-methods/session-addTorrentFile
   jsonrpc-methods/session-addTorrentUri
   jsonrpc-methods/session-getStatus
   jsonrpc-methods/session-getTorrents
   jsonrpc-methods/session-pause
   jsonrpc-methods/session-removeTorrent
   jsonrpc-methods/session-resume
   jsonrpc-methods/torrent-clearError
   jsonrpc-methods/torrent-forceRecheck
   jsonrpc-methods/torrent-getFiles
   jsonrpc-methods/torrent-getPeers
   jsonrpc-methods/torrent-getTrackers
