torrent.getTrackers
===================

Overview
--------

Gets the trackers associated with a torrent. Pass the torrent info hash as
argument.

Example
~~~~~~~

.. code:: javascript

  {
    "method": "torrent.getTrackers",
    "params": ["276425dfd2531546a36af83ee0b45264b54b0bd1"]
  }

Returns,

.. code:: javascript

  [
    {
      "isUpdating": false,
      "isVerified": true,
      "message": "",
      "tier": 0,
      "url": "http://linuxtracker.org:2710/a720ac99ca4d616384b76dcaf8520435/announce"
    },
    {
      "isUpdating": false,
      "isVerified": true,
      "message": "",
      "tier": 0,
      "url": "http://bttracker.debian.org:6969/announce"
    }
  ]
