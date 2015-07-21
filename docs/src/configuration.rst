
Configuring Hadouken
====================

Overview
--------

Hadouken is configured by editing the :file:`hadouken.json` file. JSON is a
simple structured format and is easily hand-edited using your favourite
text editor.

See the list below for where your configuration file is.

* Windows (installed): :file:`C:/ProgramData/Hadouken/hadouken.json`
* Windows (command line): :file:`%CWD%/hadouken.json`

.. note:: Before making changes, stop Hadouken. Otherwise, Hadouken will
          overwrite your changes.

.. warning:: The configuration examples below only shows the JSON you need to
             change in order for the setting to have effect. Hadouken *will*
             fail to start if :file:`hadouken.json` contains invalid JSON.


In depth
````````

.. toctree::
   :maxdepth: 1

   configuration/autoadd
   configuration/automove
   configuration/launcher
   configuration/pushbullet
   configuration/pushover
   configuration/rss


BitTorrent configuration
------------------------

Port
````

By default, Hadouken will use port *6881* for BitTorrent communications.

.. code:: javascript

   {
     "bittorrent":
     {
       "listenPort": 6881
     }
   }


Activating GeoIP location
`````````````````````````

GeoIP is activated by downloading and extracting the
`MaxMind GeoLite Country database <http://geolite.maxmind.com/download/geoip/database/GeoLiteCountry/GeoIP.dat.gz>`_.

.. code:: javascript

   {
     "bittorrent":
     {
       "geoIpFile": "C:/Data/GeoIP.dat"
     }
   }


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


Seed goals
``````````

Each torrent in Hadouken can be paused or removed when it reaches the
user-specified seed goals. If no default options are specified, the seed
goal is set to `2.0` however no action is configured.

To pause torrents when they have been seeded 200% or for 5 hours (18 000
seconds), use the configuration below.

.. note:: Only torrents added after the configuration change will get the
          new default options. Each torrent remembers its own options.

.. code:: javascript

   {
     "bittorrent":
     {
       "defaultOptions":
       {
         "seedRatio": 2.0,
         "seedTime": 18000,
         "seedAction": "pause"
       }
     }
   }

Available actions are,

 * `pause`
 * `remove`


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


HTTP configuration
------------------

Authentication
``````````````

To configure your username and password, the keys *http.auth.basic.userName*
and *http.auth.basic.password* are used.

.. code:: javascript

  {
    "http":
    {
      "auth":
      {
        "basic":
        {
          "userName": "YOUR-USERNAME",
          "password": "YOUR-PASSWORD"
        }
      }
    }
  }


Changing port
`````````````

By default, the HTTP server will listen on port *7070*. This can be changed
from the installer or the configuration file. The example below will change
the listen port to *8880*.

.. code:: javascript

  {
    "http":
    {
      "port": 8880
    }
  }


Enabling HTTPS
``````````````

By default, HTTPS is disabled. However, enabling it is as easy as generating
a private key file and adding the required configuration.

.. code:: javascript

   {
     "http":
     {
       "ssl":
       {
         "enabled": true,
         "privateKeyFile": "C:/Keys/my-private-key.pem",
         "privateKeyPassword": "my-password"
       }
     }
   }

.. note:: Using a private key which is not trusted by the client computer may
          generate warnings and errors in the client browser. To avoid
          problems, add the public key to your client system.


Custom root path
````````````````

To support advanced proxy scenarios, Hadouken supports customization of the
root path for the HTTP server. The default behavior is to serve requests from
the root `/`.

The example below will change this to let you serve requests from `/hadouken`,
which means you will reach the API at `/hadouken/api` and the GUI at
`/hadouken/gui`.

.. code:: javascript

  {
    "http":
    {
      "root": "/hadouken"
    }
  }
