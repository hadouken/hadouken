The TorrentHandle object
========================

Overview
--------

A handle to a torrent in the current BitTorrent session.


Methods
-------

void clearError()
~~~~~~~~~~~~~~~~~

If the torrent is in an error state (i.e. :code:`error`property is non-empty),
this will clear the error and start the torrent again.


void flushCache()
~~~~~~~~~~~~~~~~~

Instructs the session to flush all the disk caches for this torrent and close
all file handles. This is done asynchronously and you will be notified that
it's completed through the :code:`torrent.cacheFlushed` event.


void forceRecheck()
~~~~~~~~~~~~~~~~~~~

Puts the torrent back in a state where it assumes to have no resume data. All
peers will be disconnected and the torrent will stop announcing to the
tracker. The torrent will be added to the checking queue, and will be checked
(all the files will be read and compared to the piece hashes). Once the check
is complete, the torrent will start connecting to peers again, as normal.


Number[] getFileProgress()
~~~~~~~~~~~~~~~~~~~~~~~~~~

Returns an array with the number of bytes downloaded for each file in the
torrent. The progress values are ordered the same as the files in the
:doc:`TorrentInfo <torrentinfo>` object.


:doc:`PeerInfo[] <peerinfo>` getPeers()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Gets an array of :doc:`PeerInfo <peerinfo>` objects representing
the currently connected peers for the torrent.


:doc:`TorrentStatus <torrentstatus>` getStatus()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Returns an object with information about the status of this torrent.


:doc:`TorrentInfo <torrentinfo>` getTorrentInfo()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Gets the :doc:`TorrentInfo <torrentinfo>` for the torrent, or :code:`null`
if no metadata exists for the torrent. Metadata may be missing if the torrent
added without a :file:`.torrent` file.


:doc:`AnnounceEntry[] <announceentry>` getTrackers()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Gets an array of :doc:`AnnounceEntry <announceentry>` objects representing
the trackers for the torrent.


Boolean havePiece(Number pieceIndex)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Returns :code:`true` if this piece has been completely downloaded, and :code:`false`
otherwise.


void moveStorage(String path)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Initiates a move of the torrents files to the destination given as argument.
The result of the move operation can be monitored with the
`torrent.storageMoved` and `torrent.storageMoveFailed` events emitted by the
session.


void pause()
~~~~~~~~~~~~

Pauses the torrent.


void queueBottom()
~~~~~~~~~~~~~~~~~~

Moves the torrent to the bottom of the download queue.


void queueDown()
~~~~~~~~~~~~~~~~

Moves the torrent down one step in the download queue.


void queueTop()
~~~~~~~~~~~~~~~

Moves the torrent to the top of the download queue.


void queueUp()
~~~~~~~~~~~~~~

Moves the torrent up one step in the download queue.


void readPiece(Number pieceIndex)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Initiates a piece read request. The result of the operation can be monitored
with the `torrent.pieceRead` event emitted by the session.


void renameFile(Number fileIndex, String name)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Renames the file identified by the :code:`fileIndex` parameter to the name
passed as :code:`name`. The result of the operation can be monitored with the
`file.renamed` and `file.renameFailed` events emitted by the session.


void resume()
~~~~~~~~~~~~~

Resumes the torrent.


void saveResumeData()
~~~~~~~~~~~~~~~~~~~~~

Initiates a request to save the resume data for this torrent.


void setPriority(Number priority)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Sets the priority of the torrent. This has an effect on bandwidth/memory
rates.


Properties
----------

String infoHash
~~~~~~~~~~~~~~~

Gets the info hash for the torrent.


Boolean isValid
~~~~~~~~~~~~~~~

Gets a value indicating whether this is a valid torrent. Should always be
checked before interacting with a torrent.


Number queuePosition
~~~~~~~~~~~~~~~~~~~~

Gets the queue position for the torrent.


Number maxConnections
~~~~~~~~~~~~~~~~~~~~~

Gets or sets the maximum number of connections for this torrent.


Number maxUploads
~~~~~~~~~~~~~~~~~

Gets or sets the maximum number of upload slots for the torrent.


Boolean resolveCountries
~~~~~~~~~~~~~~~~~~~~~~~~

Gets or sets a value indicating whether the country should be resolved
for peers. This requires GEOIP support.


Boolean sequentialDownload
~~~~~~~~~~~~~~~~~~~~~~~~~~

Gets or sets a value indicating whether files in this torrent should be
downloaded sequentially.


Boolean uploadMode
~~~~~~~~~~~~~~~~~~

Gets or sets a value indicating whether the torrent is in upload mode.


Number uploadLimit
~~~~~~~~~~~~~~~~~~

Gets or sets the upper speed limit for uploads.
