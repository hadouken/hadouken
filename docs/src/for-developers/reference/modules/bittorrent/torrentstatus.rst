The TorrentStatus object
========================

Overview
--------

An object with status values for a specific torrent.

Properties
----------

String name
~~~~~~~~~~~

Gets the name of the torrent, or null if no metadata exists.


Number progress
~~~~~~~~~~~~~~~

Gets the progress as a decimal value [0-1].


String savePath
~~~~~~~~~~~~~~~

Gets the save path. This includes the torrent name for multi-file torrents.


Number downloadRate
~~~~~~~~~~~~~~~~~~~

Gets the download rate.


Number uploadRate
~~~~~~~~~~~~~~~~~

Gets the upload rate.


Number downloadedBytes
~~~~~~~~~~~~~~~~~~~~~~

Gets the number of downloaded bytes for the torrent since Hadouken was
started.


Number downloadedBytesTotal
~~~~~~~~~~~~~~~~~~~~~~~~~~~

Gets the total number of downloaded bytes for the torrent. This is persisted
across restarts.


Number uploadedBytes
~~~~~~~~~~~~~~~~~~~~

Gets the number of uploaded bytes for the torrent since Hadouken was
started.


Number uploadedBytesTotal
~~~~~~~~~~~~~~~~~~~~~~~~~

Gets the total number of uploaded bytes for the torrent. This is persisted
across restarts.


Number numPeers
~~~~~~~~~~~~~~~

Gets the number of currently connected peers for the torrent.


Number numSeeds
~~~~~~~~~~~~~~~

Gets the number of currently connected seeds for the torrent.


Number state
~~~~~~~~~~~~

Gets the current state.


Boolean isFinished
~~~~~~~~~~~~~~~~~~

Returns :code:`true` if all pieces with priority > 0 are downloaded.


Boolean isMovingStorage
~~~~~~~~~~~~~~~~~~~~~~~

Returns :code:`true` if the torrent is currently being moved from one location
to another.


Boolean isPaused
~~~~~~~~~~~~~~~~

Returns :code:`true` if the torrent is currently paused.


Boolean isSeeding
~~~~~~~~~~~~~~~~~

Returns :code:`true` if all pieces have been downloaded.


Boolean isSequentialDownload
~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Returns :code:`true` if the torrent is downloading pieces sequentially.


Boolean hasMetadata
~~~~~~~~~~~~~~~~~~~

Returns :code:`true` if the torrent has downloaded its torrent file.


Boolean needSaveResume
~~~~~~~~~~~~~~~~~~~~~~

Returns :code:`true` if the torrent needs to save its resume data.
