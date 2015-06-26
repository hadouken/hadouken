torrent.forceRecheck
====================

Overview
--------

Forces a re-check of the files associated with the torrent. Pass a torrent
info hash as argument.

Example
~~~~~~~

.. code:: javascript

  {
    "method": "torrent.forceRecheck",
    "params": ["276425dfd2531546a36af83ee0b45264b54b0bd1"]
  }

Returns,

.. code:: javascript

  true
