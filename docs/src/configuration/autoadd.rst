
Configuring AutoAdd
===================

Overview
--------

The AutoAdd extension monitors a list of directories and automatically adds any
torrent files it finds. You can provide a regular expression
(ECMAScript syntax) as well as specify save path and tags for the torrent it
matches.


Enabling AutoAdd
----------------

.. code:: javascript

  {
    "extensions":
    {
      "autoadd":
      {
        "enabled": true
      }
    }
  }


Monitoring a folder
-------------------

This shows the simplest configuration needed to monitor a folder. The default
pattern will add any files with a :file:`.torrent` extension. If you do not
specify a save path, the default save path will be used.

.. code:: javascript

  {
    "extensions":
    {
      "autoadd":
      {
        "enabled": true,
        "folders":
        [
          { "path": "C:/Torrents" }
        ]
      }
    }
  }


Matching with patterns
----------------------

One of the more powerful features of AutoAdd is the ability to match torrent
files with a regular expression. You can, for example, match any Debian files
and save them to their own folder. The pattern below will match all file names
starting with `debian-` and ending with a `.torrent` extension, and save them
to :file:`C:/Downloads/Debian ISOs`.

.. code:: javascript

  {
    "extensions":
    {
      "autoadd":
      {
        "enabled": true,
        "folders":
        [
          {
            "path":     "C:/Torrents",
            "pattern":  "^debian-.*\\.torrent$",
            "savePath": "C:/Downloads/Debian ISOs"
          }
        ]
      }
    }
  }


Adding tags
-----------

The AutoAdd extension also supports tagging torrents. Tags provide a simple
categorization system for torrents, and other extensions can use tags to apply
their own set of rules. The configuration below will monitor a folder and add
two tags to each torrent it finds.

.. code:: javascript

  {
    "extensions":
    {
      "autoadd":
      {
        "enabled": true,
        "folders":
        [
          {
            "path": "C:/Torrents",
            "tags": [ "tag1", "tag2" ]
          }
        ]
      }
    }
  }
