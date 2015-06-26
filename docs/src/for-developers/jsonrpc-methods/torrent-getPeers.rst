torrent.getPeers
================

Overview
--------

Gets the peers associated with a torrent. Pass the torrent info hash as
argument.

Example
~~~~~~~

.. code:: javascript

  {
    "method": "torrent.getPeers",
    "params": ["276425dfd2531546a36af83ee0b45264b54b0bd1"]
  }

Returns,

.. code:: javascript

  [
    {
      "country": "",
      "ip": "192.73.238.114",
      "port": 51413,
      "connectionType": 0,
      "client": "Transmission 2.84",
      "progress": 1,
      "downloadRate": 352795,
      "uploadRate": 12354,
      "downloadedBytes": 6043127,
      "uploadedBytes": 0
    },
    {
      "country": "",
      "ip": "58.7.57.117",
      "port": 51413,
      "connectionType": 0,
      "client": "Transmission 2.52",
      "progress": 1,
      "downloadRate": 17107,
      "uploadRate": 2119,
      "downloadedBytes": 179029,
      "uploadedBytes": 0
    }
  ]
