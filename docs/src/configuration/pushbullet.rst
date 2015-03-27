
Configuring Pushbullet
======================

Overview
--------

The Pushbullet extension integrates with `Pushbullet`_ to provide push
notifications for various platforms. You can configure which events will send
push notifications.

.. _Pushbullet: https://www.pushbullet.com


Enabling Pushbullet
-------------------

.. code:: javascript

  {
    "extensions":
    {
      "pushbullet":
      {
        "enabled": true
      }
    }
  }


Configuration
-------------

To use the extension you have to register at `Pushbullet`_. This will get you
a token which you put in the configuration. The configuration below will push
notifications when Hadouken loads, any time a torrent is added and any time a
torrent finishes downloading.

.. _Pushbullet: https://www.pushbullet.com

.. code:: javascript

  {
    "extensions":
    {
      "pushbullet":
      {
        "enabled": true,
        "token": "YOUR-PUSHBULLET-AUTH-TOKEN",
        "enabledEvents":
        [
          "hadouken.loaded",
          "torrent.added",
          "torrent.finished"
        ]
      }
    }
  }


Available events
----------------

* `hadouken.loaded`
* `torrent.added`
* `torrent.finished`
