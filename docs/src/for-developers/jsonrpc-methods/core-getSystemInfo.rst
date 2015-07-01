core.getSystemInfo
==================

Overview
--------

Gets an object with information about this Hadouken instance. It contains
the Git commitish and branch, libtorrent version and Hadouken version.

Example
~~~~~~~

.. code:: javascript

  {
    "method": "core.getSystemInfo",
    "params": []
  }

Returns,

.. code:: javascript

  {
    "commitish": "e51736c",
    "branch": "develop",
    "versions":
    {
      "libtorrent": "1.0.5.0",
      "hadouken": "5.0.0"
    }
  }
