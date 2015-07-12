session.getTorrents
=================

Overview
--------

Gets an object with information about the torrents in the Hadouken instance.
The object keys are the torrents info hash.

The method takes an optional filter argument which can be used to filter the
result by tags. If no filter is given (ie. undefined) then all torrents will
be returned.

The example shows how to filter torrents based on the foo tag. Multiple tags
can be specified and only torrents having all tags will be returned.


Example
~~~~~~~

.. code:: javascript

  {
    "method": "session.getTorrents",
    "params": [ { "tags": [ "foo" ] } ]
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
      "queuePosition": 0,
      "tags": [ "foo", "bar" ]
    }
  }
