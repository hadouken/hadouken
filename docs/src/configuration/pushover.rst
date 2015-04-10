
Configuring Pushover
====================

Overview
--------

The Pushover extension integrates with `Pushover`_ to provide push
notifications for various platforms. You can configure which events will send
push notifications.

.. _Pushover: https://pushover.net


Enabling Pushover
-----------------

.. code:: javascript

  {
    "extensions":
    {
      "pushover":
      {
        "enabled": true
      }
    }
  }


Configuration
-------------

To use the extension you have to register at `Pushover`_. This will give you
a *user key*. You also need to register an application to get an *API token*.

.. _Pushover: https://pushover.net

.. code:: javascript

  {
    "extensions":
    {
      "pushover":
      {
        "enabled": true,
        "user":  "YOUR-PUSHOVER-USER-KEY"
        "token": "YOUR-PUSHOVER-API-TOKEN",
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
