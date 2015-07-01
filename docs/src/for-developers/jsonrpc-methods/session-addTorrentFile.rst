session.addTorrentFile
======================

Overview
--------

Adds a torrent file to the session. The file should be passed as a `base64`
encoded string. A second parameter can change the save path and other
properties of the torrent.

The call returns the info hash of the added torrent which can be used to
locate it in the session.

Example
~~~~~~~

.. code:: javascript

  {
    "method": "session.addTorrentFile",
    "params":
    [
      "<base64 encoded torrent file>",
      {
        "savePath": "C:/Downloads"
      }
    ]
  }

Returns,

.. code:: javascript

  "<info hash>"
