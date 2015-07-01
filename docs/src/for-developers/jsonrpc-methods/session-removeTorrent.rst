session.removeTorrent
=====================

Overview
--------

Removes the torrent identified by the info hash in the first argument.
Optionally pass `true` as the second argument to remove the data associated
with the torrent.

Example
~~~~~~~

.. code:: javascript

  {
    "method": "session.removeTorrent",
    "params": ["276425dfd2531546a36af83ee0b45264b54b0bd1", false]
  }

Returns,

.. code:: javascript

  true
