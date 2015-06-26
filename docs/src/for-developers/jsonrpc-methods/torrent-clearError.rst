torrent.clearError
==================

Overview
--------

Clears the error for a torrent in the `error` state. This will make the
session re-activate the torrent. Pass the torrent info hash as argument.

Example
~~~~~~~

.. code:: javascript

  {
    "method": "torrent.clearError",
    "params": ["276425dfd2531546a36af83ee0b45264b54b0bd1"]
  }

Returns,

.. code:: javascript

  true
