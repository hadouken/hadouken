
Configuring RSS
===============

Overview
--------

Hadouken can monitor and download torrents from various RSS feeds and also
filter any items against regular expressions.


Monitoring a feed
-----------------

The simplest RSS configuration will monitor and download all items from a
specified feed.

.. code:: javascript

  {
    "feeds":
    [
      "url":    "http://some-rss.net/feed",
      "filter": "*"
    ]
  }


RegExp filters
--------------

Each feed can be configured with a regular expression (ECMAScript syntax) to
include and exclude any items. The following filter will include items with
`720p` in the name, and exclude `NUKED` items.

The exclude filter is optional.

.. code:: javascript

  {
    "feeds":
    [
      "url":    "http://some-rss.net/feed",
      "filter": [ "regex", "720p", "NUKED" ]
    ]
  }


Feed options
------------

There are a few options you can configure for each feed, such as save path and
TTL (poll rate).

Save path
~~~~~~~~~

.. code:: javascript

  {
    "feeds":
    [
      "url": "http://some-rss.net/feed",
      "options":
      {
        "savePath": "C:/Downloads/from-some-rss"
      }
    ]
  }

TTL
~~~

Use this option with care - it is not nice to hammer feeds just to get an early
start. The `ttl` value indicates the poll rate in minutes.

.. code:: javascript

  {
    "feeds":
    [
      "url": "http://some-rss.net/feed",
      "ttl": 2
    ]
  }

Dry-run
~~~~~~~

The dry-run option will stop any torrents from getting added to Hadouken and
instead output information about the item in the log file. This can be used to
debug regular expression filters.

.. code:: javascript

  {
    "feeds":
    [
      "url": "http://some-rss.net/feed",
      "options":
      {
        "dryRun": true
      }
    ]
  }
