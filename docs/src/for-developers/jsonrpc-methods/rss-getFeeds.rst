rss.getFeeds
============

Overview
--------

Gets an object with information about all the managed feeds in the current
Hadouken instance. The object keys can be used as identifiers for the
`rss.getFeedItems` method.

Example
~~~~~~~

.. code:: javascript

  {
    "method": "rss.getFeeds",
    "params": []
  }

Returns,

.. code:: javascript

  {
    "http://my-rss.com/feed":
    {
      "url": "http://my-rss.com/feed",
      "title": "My awesome RSS feed",
      "description": "",
      "lastUpdate": 12341234,
      "nextUpdate": 1800,
      "isUpdating": false,
      "items": 10
    }
  }
