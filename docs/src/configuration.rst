
Configuring Hadouken
====================

Overview
--------

Hadouken is configured by editing the :file:`config.json` file. JSON is a
simple structured format and is easily hand-edited using your favourite
text editor.

See the list below for where your configuration file is.

* Windows: :file:`C:/ProgramData/Hadouken/hadouken.json`

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


HTTP configuration
------------------

Authentication
``````````````
Hadouken supports three modes of authentication, *none*, *HTTP Basic* and
*Token*. The installer supports the configuration of all three modes.

To activate *Token* authentication, set the `http.auth.type` setting to
*token* and then supply a token.

.. code:: javascript

  {
    "http":
    {
      "auth":
      {
        "type": "token",
        "token": "YOUR-TOKEN-HERE"        
      }
    }
  }

To activate *HTTP Basic* authentication, set the `http.auth.type` setting to
*basic* and then provide a username and password.

.. code:: javascript

  {
    "http":
    {
      "auth":
      {
        "type": "basic",
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
which means you will reach the API at `/hadouken/api`.

.. code:: javascript

  {
    "http":
    {
      "root": "/hadouken"
    }
  }
