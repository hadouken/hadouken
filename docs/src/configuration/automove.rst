
Configuring AutoMove
====================

Overview
--------

The AutoMove extension can be used to move finished torrents to specific
directories depending on basic rules, such as matching the torrent name against
a regular expression (ECMAScript syntax) or checking that a torrent has a
specific set of tags.


Enabling AutoMove
-----------------

.. code:: javascript

  {
    "extensions":
    {
      "automove":
      {
        "enabled": true
      }
    }
  }


Pattern-based moving
--------------------

The pattern filter matches a torrent name against a regular expression pattern
and moves the torrent to the path specified in `path`. We specify to use the
filter `pattern` and provide the filter with some data. In this case, the
filter expects a pattern and a field.

.. code:: javascript

  {
    "extensions":
    {
      "automove":
      {
        "enabled": true,
        "rules":
        [
          {
            "path": "C:/Downloads/Debian ISOs",
            "filter": "pattern",
            "data":
            {
              "pattern": "^debian-.*",
              "field": "name"
            }
          }
        ]
      }
    }
  }


Tag-based moving
----------------

The tag filter will check a torrent against a list of tags. If the torrent has
all the tags, it will move it to the path specified in `path`. In the example
below, only torrents which have been tagged with both `debian` and `iso` will
be matched and moved.

This can be used together with the AutoAdd extension to set up advanced
matching and moving rules.

.. code:: javascript

  {
    "extensions":
    {
      "automove":
      {
        "enabled": true,
        "rules":
        [
          {
            "path": "C:/Downloads/Debian ISOs",
            "filter": "tags",
            "data": [ "debian", "iso" ]
          }
        ]
      }
    }
  }
