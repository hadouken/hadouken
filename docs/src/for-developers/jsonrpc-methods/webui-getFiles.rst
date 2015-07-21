webui.getFiles
==============

Overview
--------

Returns the files associated with the torrent identified by the info hash
given in the first parameter. The *files* array always has n*2 items where
*n* is the info hash and *n+1* is the files array.


Example
~~~~~~~

.. code:: javascript

  {
    "method": "webui.getFiles",
    "params": [ "<info hash>" ]
  }

Returns,

.. code:: javascript

  {
    "files": [
      "<info hash>",
      [
        "file.iso", // path to file relative to the save path of the torrent
        100, // file size in bytes
        80, // downloaded bytes
        1, // priority
        -1, //first piece
        -1, // num piece
        -1, // streamable
        -1, // encoded rate
        -1, // duration
        -1, // width
        -1, // height
        -1, // stream eta
        -1  // streamability
      ]
    ]
  }
