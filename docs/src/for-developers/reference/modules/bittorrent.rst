The BitTorrent module
=====================

Overview
--------

The :code:`bittorrent` module contains all BitTorrent related functions and
classes for Hadouken.

.. code:: javascript

  var bt = require("bittorrent");


Properties
----------

:doc:`Session <bittorrent/session>` session
~~~~~~~

Gets the BitTorrent session instance. Use the session to add, remove and
find torrents.

.. code:: javascript

  var session = require("bittorrent").session;
