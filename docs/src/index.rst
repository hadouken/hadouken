
Hadouken documentation
======================

Overview
--------

Hadouken is a cross-platform headless BitTorrent client. It runs as a Linux
daemon/Windows Service and provides a JSONRPC API over HTTP to control it. In
addition to running headless, Hadouken also has a powerful extension system
that gives developers freedom to extend it in various ways.

.. note:: In case you find errors in this documentation you can help by sending
          `pull requests <https://github.com/hadouken/hadouken>`_!


Features
--------

* No GUI.
* Highly configurable.
* Low memory usage.
* JSONRPC API over HTTP.
* Automatically add torrents from user-configurable directories.
* Move completed torrents based on user-configurable rules.
* Send push notifications via Pushbullet.
* Launch executables on various events, such as when a torrent finishes.


Downloads
---------

Hadouken is still in beta and only Windows binaries are provided. These can be
downloaded from `the build server <https://builds.nullreferenceexception.se/>`_.


Installation
------------

Installation instructions will vary depending on your platform. See the
documentation for your specific platform.

.. toctree::
   :maxdepth: 1

   installing/ubuntu
   installing/windows


Documentation
-------------

.. toctree::
   :maxdepth: 1

   configuration
   jsonrpc
