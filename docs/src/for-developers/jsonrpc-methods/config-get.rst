config.get
==========

Overview
--------

Gets an object with the configuration for this Hadouken instance. Takes an
optional `key` parameter to return the value for that specific configuration
property.

Example
~~~~~~~

*Get a specific configuration property*,

.. code:: javascript

  {
    "method": "config.get",
    "params": ["bittorrent.dht.enabled"]
  }

Returns,

.. code:: javascript

  {
    "bittorrent.dht.enabled": true
  }
