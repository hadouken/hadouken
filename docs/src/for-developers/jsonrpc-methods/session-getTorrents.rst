session.getTorrents
=================

Overview
--------

Gets an object with information about the torrents in the Hadouken instance.
The object keys are the torrents info hash.

Example
~~~~~~~

.. code:: javascript

  {
    "method": "session.getTorrents",
    "params": []
  }

Returns,

.. code:: javascript

  {
    "276425dfd2531546a36af83ee0b45264b54b0bd1":
    {
      "name": "debian-8.1.0-amd64-netinst.iso",
      "infoHash": "276425dfd2531546a36af83ee0b45264b54b0bd1",
      "progress": 0.0020280000753700733,
      "savePath": "C:/Downloads",
      "downloadRate": 274864,
      "uploadRate": 12238,
      "downloadedBytes": 1702240,
      "downloadedBytesTotal": 1690882,
      "uploadedBytes": 16809,
      "uploadedBytesTotal": 0,
      "numPeers": 28,
      "numSeeds": 26,
      "totalSize": 258998272,
      "state": 3,
      "isFinished": false,
      "isMovingStorage": false,
      "isPaused": false,
      "isSeeding": false,
      "isSequentialDownload": false,
      "queuePosition": 0
    }
  }
