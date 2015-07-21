
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

* A powerful embedded web interface.
* Highly configurable, a single JSON text file to configure all aspects of
  Hadouken.
* Low memory footprint making it ideal for low-powered devices such as the
  Raspberry Pi.
* JSONRPC API over HTTP giving third-party developers complete freedom to
  integrate Hadouken with any kind of system.
* Automatically monitor directories for torrent files and add them based on
  regular expression matching, giving powerful abilities for sorting and tagging
  torrents.
* Advanced RSS feed monitoring capabilities making subscribing to various feeds
  a breeze.
* Move completed torrents matching specific regular expressions or having the
  correct set of tags.
* Send push notifications to your devices via `Pushbullet <https://www.pushbullet.com>`_
  or `Pushover <https://pushover.net>`_.
* Launch executables on various events, such as when a torrent finishes.
* A powerful JavaScript API making it easy to customize and extend Hadouken
  with plugins.
* Unattended installations to give domain administrators the ability to set up
  Hadouken clusters with ease.


Downloads
---------

Hadouken can be downloaded from
`the release feed <https://github.com/hadouken/hadouken/releases>`_. Binaries
are provided for Windows. For other platforms you need to build it yourself.

Installation
------------

Installation instructions will vary depending on your platform. See the
documentation for your specific platform.

.. toctree::
   :maxdepth: 1

   installing/windows


Building Hadouken
-----------------

.. toctree::
   :maxdepth: 1

   building/ubuntu
   building/windows


Documentation
-------------

.. toctree::
   :maxdepth: 1

   getting-started
   migrating
   configuration
   for-developers
