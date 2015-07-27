
Configuring AutoLabel
=====================

Overview
--------

The AutoLabel extension can automatically set a label on added torrents so you
don't have to. It matches a regular expression against either the torrent name
or any of the torrent trackers and then sets the configured label.


Enabling AutoLabel
----------------

.. code:: javascript

  {
    "extensions":
    {
      "autolabel":
      {
        "enabled": true
      }
    }
  }


Matching a torrent
------------------

The simplest way to auto label a torrent is to match the name against a regular
expression. This example matches any torrents starting with `debian` and labels
them as *debian*.

.. code:: javascript

  {
    "extensions":
    {
      "autolabel":
      {
        "enabled": true,
        "rules":
        [
          {
            "field": "name",
            "pattern": "^debian.*$",
            "label": "debian"
          }
        ]
      }
    }
  }


Matching trackers
-----------------

It is equally easy to match against any of the trackers in the torrent file.
This will label a torrent as *debian* if one of the trackers starts with
`http://bttracker.debian.org`.

.. code:: javascript

  {
    "extensions":
    {
      "autolabel":
      {
        "enabled": true,
        "rules":
        [
          {
            "field": "tracker",
            "pattern": "^http://bttracker\\.debian\\.org.*$",
            "label": "debian"
          }
        ]
      }
    }
  }
