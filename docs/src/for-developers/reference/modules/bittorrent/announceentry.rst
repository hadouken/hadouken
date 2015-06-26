The AnnounceEntry object
========================

Overview
--------

Represents a tracker for a torrent.

Properties
----------

Boolean isUpdating
~~~~~~~~~~~~~~~~~~

Gets a value indicating if the torrent is currently waiting for a response
from this tracker.


Boolean isVerified
~~~~~~~~~~~~~~~~~~

Gets a value indicating if the torrent has received a valid response from
this tracker.


String message
~~~~~~~~~~~~~~

Contains the warning/error message (if any), otherwise empty.


Number tier
~~~~~~~~~~~

The tier this tracker belongs to.


String url
~~~~~~~~~~

The URL to the tracker as it appeared in the :file:`.torrent` file.
