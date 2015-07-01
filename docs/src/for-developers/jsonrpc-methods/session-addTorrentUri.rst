session.addTorrentUri
=====================

Overview
--------

Adds a URL to a torrent file to the session. The URL is passed as the first
parameter. A second parameter can change the save path and other properties of
the torrent.

The call returns nothing since the info hash is not known at the time.

.. note:: The URL can also be a magnet link.

Example
~~~~~~~

.. code:: javascript

  {
    "method": "session.addTorrentUri",
    "params":
    [
      "http://my-awesome-torrent.com/file.torrent",
      {
        "savePath": "C:/Downloads"
      }
    ]
  }

Returns,

.. code:: javascript

  undefined
