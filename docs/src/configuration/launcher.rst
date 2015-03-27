
Configuring Launcher
====================

Overview
--------

The Launcher extension provides an easy way to launch executables on various
Hadouken events. It can be used to start command-line tools that provide some
service, for example to unpack RAR'ed torrents when they finish.

.. warning:: Executables launched will run with the same privileges as the user
             which runs Hadouken.


Enabling Launcher
-----------------

.. code:: javascript

  {
    "extensions":
    {
      "launcher":
      {
        "enabled": true
      }
    }
  }


Launching an executable
-----------------------

The executable you launch will recieve three arguments passed to its command
line. These are, in order,

* The hex-encoded info hash.
* The torrents name.
* The save path.

Each entry in the *apps* array is another array with two fields. The first
field is a string with the *event name*, and the second is a string with a path
to the application to launch.

The example below will launch :file:`C:/Apps/some-app.bat` every time a torrent
is added.

.. code:: javascript

  {
    "extensions":
    {
      "launcher":
      {
        "enabled": true,
        "apps":
        [
          [ "torrent.added", "C:/Apps/some-app.bat" ]
        ]
      }
    }
  }


Available events
----------------

* `torrent.added`
* `torrent.finished`
