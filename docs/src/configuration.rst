
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


Extensions
``````````

.. toctree::
   :maxdepth: 1

   configuration/autoadd
   configuration/automove
   configuration/launcher
   configuration/pushbullet


BitTorrent configuration
------------------------

Anonymous mode
``````````````

Activating anonymous mode will make Hadouken try to hide its identity to a
certain degree. The peer ID will no longer include the fingerprint, the user
agent when announcing to trackers will be an empty string. It will also try to
not leak other identifying information, such as local listen ports, your IP,
etc.

.. note:: Activating anonymous mode may have an impact on your ability to
          connect to private trackers, which uses the peer ID and user agent
          to identify white-listed clients.

.. code:: javascript

  {
    "bittorrent":
    {
      "anonymousMode": true
    }
  }


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


Storage allocation
``````````````````

There are two modes in which files can be allocated on disk, *full allocation*
or *sparse allocation*. Sparse allocation is the recommended, and default,
setting.

* In sparse allocation mode, sparse files are used, and pieces are downloaded
  directly where they belong.

* In full allocation mode, the entire file is filled with zeros before anything
  is downloaded. The files are allocated on demand, the first time anything is
  written to them. This avoids heavily fragmented files.

By setting the *sparse* flag to false, Hadouken will use full allocation.
Changing this setting will only affect new torrents.

.. code:: javascript

  {
    "bittorrent":
    {
      "storage":
      {
        "sparse": false
      }
    }
  }
