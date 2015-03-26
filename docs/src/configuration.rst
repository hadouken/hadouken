
Configuring Hadouken
====================

Overview
--------

Hadouken is configured by editing the :file:`config.json` file. JSON is a
simple structured format and is easily hand-edited using your favourite
text editor.

See the list below for where your configuration file is.

* Windows: :file:`C:/ProgramData/Hadouken/config.json`

.. warning:: The configuration examples below only shows the JSON you need to
             change in order for the setting to have effect. Hadouken *will*
             fail to start if :file:`config.json` contains invalid JSON.


BitTorrent configuration
------------------------

Disabling DHT
`````````````

.. code:: javascript

  {
    "bittorrent":
    {
      "dht":
      {
        "enabled": false
      }
    }
  }

.. note:: The `routers` are ignored if DHT is disabled.
