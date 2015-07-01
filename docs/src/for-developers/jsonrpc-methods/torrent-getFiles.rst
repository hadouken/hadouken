torrent.getFiles
================

Overview
--------

Gets the files associated with a torrent. Pass the torrent info hash as
argument.

Example
~~~~~~~

.. code:: javascript

  {
    "method": "torrent.getFiles",
    "params": ["276425dfd2531546a36af83ee0b45264b54b0bd1"]
  }

Returns,

.. code:: javascript

  [
    {
      "index": 0,
      "path": "debian-8.1.0-amd64-netinst.iso",
      "progress": 258998272,
      "size": 258998272
    }
  ]
