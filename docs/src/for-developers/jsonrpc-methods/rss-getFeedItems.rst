rss.getFeedItems
================

Overview
--------

Gets a list of the in the feed specified by the URL passed as first
argument. The feed must be configured - ie. this method does not retrieve
items for unmanaged feeds.

Example
~~~~~~~

.. code:: javascript

  {
    "method": "rss.getFeedItems",
    "params": ["http://rss-feed-url.com/feed.rss"]
  }

Returns,

.. code:: javascript

  [
    {
      "uuid": "unique identifier",
      "url": "url to download item"
    }
  ]
