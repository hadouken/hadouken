The JSONRPC API
===============

JSONRPC is a standard defined at jsonrpc.org. It is a simple RPC protocol and
most languages has at least a way to encode/decode JSON data and send it over
HTTP, which makes it a safe bet for a remote API.

The JSONRPC API can be accessed at :file:`http://localhost:7070/api`. Adjust
accordingly.

.. note:: All examples are shown with only a method name and request/response
          data - the JSONRPC object container has been left out.

Methods
-------

.. toctree::
   :maxdepth: 1

   jsonrpc-methods/core-getSystemInfo
   jsonrpc-methods/webui-addTorrent
   jsonrpc-methods/webui-getFiles
   jsonrpc-methods/webui-getPeers
