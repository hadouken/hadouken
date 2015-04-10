
Hadouken documentation
======================

Overview
--------

Hadouken is a cross-platform headless BitTorrent client. It runs as a Linux
daemon/Windows Service and provides a JSONRPC API over HTTP to control it. In
addition to running headless, Hadouken also has a powerful extension system
that gives developers freedom to extend it in various ways.

Hadouken runs on the following operating systems,

* Windows 7, 8, 8.1 and 10
* Windows Server 2008 R2, 2012, 2012 R2
* Debian 7.8
* Ubuntu 14.04 LTS

Furthermore, it also runs on the following devices,

* Raspberry Pi 2 Model B

.. note:: In case you find errors in this documentation you can help by sending
          `pull requests <https://github.com/hadouken/hadouken>`_!


Features
--------

* No GUI. Hadouken runs as a Windows Service/Linux daemon.
* Highly configurable, a single JSON text file to configure all aspects of
  Hadouken.
* Low memory footprint making it ideal for low-powered devices such as the
  Raspberry Pi.
* JSONRPC API over HTTP giving third-party developers complete freedom to
  integrate Hadouken with any kind of system.
* Automatically monitor directories for torrent files and add them based on
  regular expression matching, giving powerful abilities for sorting and tagging
  torrents.
* Move completed torrents matching specific regular expressions or having the
  correct set of tags.
* Send push notifications to your devices via `Pushbullet <https://www.pushbullet.com>`_
  or `Pushover <https://pushover.net>`_.
* Launch executables on various events, such as when a torrent finishes.
* *Experimental* JavaScript extension exposing a Node-like API to enable
  developers to write extensions in JS.
* Unattended installations to give domain administrators the ability to set up
  Hadouken clusters with ease.


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

   getting-started
   migrating
   configuration
