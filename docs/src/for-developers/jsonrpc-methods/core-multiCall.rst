core.multiCall
==============

Overview
--------

Enables API users to call multiple methods in one request. Takes an object
with method names as keys and their arguments as value and returns a
corresponding object with method names and results.


Example
~~~~~~~

.. code:: javascript

  {
    "method": "core.multiCall",
    "params":
    {
      "misc.getTags": [ ],
      "torrent.getFiles": [ "276425dfd2531546a36af83ee0b45264b54b0bd1" ]
    }
  }

Returns,

.. code:: javascript

  {
    "misc.getTags":
    [
      "foo",
      "bar"
    ],
    "torrent.getFiles":
    [
      {
        "index": 0,
        "path": "debian-8.1.0-amd64-netinst.iso",
        "progress": 258998272,
        "size": 258998272
      }
    ]
  }
