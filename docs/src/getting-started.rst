
Getting started
===============

Overview
--------

Hadouken does not provide its own GUI. This is handled by third-party
applications that integrate with the JSONRPC API provided by Hadouken. The
"official" remote client for Hadouken is a web application hosted at
remote.hdkn.net_ and is a simple HTML/JS application that relies on CORS to
make requests to Hadouken.

This means that no traffic is proxied through any third-party server. Every
request is made directly from the browser to your Hadouken instance. Neat.


Client configuration
--------------------

For any new browser you use with remote.hdkn.net_ you must configure the client
so it knows where your Hadouken instance is available. Click the big blue
*Configure* button or the gears icon in the upper right corner of the
navigation bar to get to the configuration page.

Adjust the *Hadouken URL* and *Authentication type* to reflect the settings in
your Hadouken instance. Click the *Test connection* button to test. If you
provided valid settings, a text will notify you of the version of Hadouken it
found.

Hit *Save* and we're done configuring.


Adding torrents
---------------

To add torrents, head over to the main torrents view by clicking *Hadouken
Remote* in the navigation bar.

The blue *Add* button in the Torrents headline opens up a dialog letting you
choose from adding files or entering a URL or magnet link.

.. _remote.hdkn.net: http://remote.hdkn.net
