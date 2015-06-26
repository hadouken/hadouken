The BitTorrent session
======================

Overview
--------

The BitTorrent session object contains methods for adding, removing and
finding torrents, as well as other BitTorrent related functions for DHT, etc.


Methods
-------

:code:`void addDhtRouter(String host, Number port)`
~~~~~~~~~~~~~~~~~~

Add a DHT router to the session.


:code:`FeedHandle addFeed(FeedSettings settings)`
~~~~~~~~~~~~~~~~~~

Add a RSS feed to the session. Control the added feed by modifying the
:code:`FeedSettings` argument.


void addTorrent(:doc:`AddTorrentParams <addtorrentparams>` params)
~~~~~~~~~~~~~~~~~~

Add a torrent to the session.


:doc:`TorrentHandle <torrenthandle>` findTorrent(String infoHash)
~~~~~~~~~~~~~~~~~~

Find a torrent with the given info hash. If no torrent is found, the
:code:`isValid` property on the :doc:`TorrentHandle <torrenthandle>` will return false.


void removeTorrent(:doc:`TorrentHandle <torrenthandle>` handle, Boolean removeData)
~~~~~~~~~~~~~~~~~~

Removes a torrent from the session. If the :code:`removeData` argument is
:code:`true`, the data files associated with the torrent are removed as well.
