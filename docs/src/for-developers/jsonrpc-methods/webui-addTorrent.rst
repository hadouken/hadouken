webui.addTorrent
================

Overview
--------

Adds a torrent to the session, with the specified save path, label, tags and
trackers. This method can add both a file and URL. Files must be base64
encoded.

The method takes three arguments, *type*, *data* and *params*.

- *type* - the source type. Can be *file* or *url*.
- *data* - the base64 encoded torrent file, or a URL to a torrent file.
- *params* - an object with properties describing the torrent to add, eg.
  save path, tags, label etc.

  - *label*
  - *filePriorities* - if you want to pre-set the priorities for files in
    this torrent, set this array to their specific priorities.
  - *savePath* - a zero-based index specifying which save path to use. Index
    0 represents the default save path, and indices higher than one represents
    entries in the *bittorrent.downloadDirectories* array (savePath=1 is the
    first entry in that array).
  - *subPath* - a sub-directory of the save path.
  - *tags* - a string array of tags for this torrent.
  - *trackers* - a string array of extra trackers for this torrent.


Example
~~~~~~~

*Add a base64 encoded torrent file*,

.. code:: javascript

  {
    "method": "webui.addTorrent",
    "params": [
      "file",
      "<base64 encoded data>",
      {
        "label": "software",
        "savePath": 0,
        "subPath": "linux isos",
        "tags": [ "debian", "linux", "oss" ]
      }
    ]
  }

Returns,

*If you add a URL, the info hash is not known and you will receive
*undefined* instead of an info hash.

.. code:: javascript

  "<infoHash>"
