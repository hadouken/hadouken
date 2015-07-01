The PeerInfo object
===================

Overview
--------

An object representing a peer for a specific torrent handle.

Properties
----------

String country
~~~~~~~~~~~~~~

Gets the two-letter country code for the peer. GEOIP must be enabled.


String ip
~~~~~~~~~

Gets the IP address of the peer.


Number port
~~~~~~~~~~~

Gets the IP port of the peer.


Number connectionType
~~~~~~~~~~~~~~~~~~~~~

Gets the type of connection.


String client
~~~~~~~~~~~~~

Gets the BitTorrent client of the peer.


Number progress
~~~~~~~~~~~~~~~

Gets the download progress of the peer.


Number downloadRate
~~~~~~~~~~~~~~~~~~~

Gets the estimated download rate of the peer.


Number uploadRate
~~~~~~~~~~~~~~~~~

Gets the estimated upload rate of the peer.


Number downloadedBytes
~~~~~~~~~~~~~~~~~~~~~~

Gets the number of bytes downloaded from the peer.


Number uploadedBytes
~~~~~~~~~~~~~~~~~~~~

Gets the number of bytes uploaded to the peer.
