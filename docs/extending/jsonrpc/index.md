# JSONRPC methods

This is a list of all the JSONRPC methods that ships with Hadouken (including the core plugins). You can call any of these via the WCF gateway you have access to as a plugin.

To call these over HTTP, you need the `HttpJsonRpc` plugin loaded.

## Core methods
- [`core.getAuthInfo`](core.getAuthInfo.md)
- [`core.multiCall`](core.multiCall.md)
- [`core.setAuth`](core.setAuth.md)
- [`events.publish`](events.publish.md)
- [`plugins.load`](plugins.load.md)
- [`plugins.unload`](plugins.unload.md)
- [`plugins.list`](plugins.list.md)

## Config methods
- [`config.delete`](config.delete.md)
- [`config.get`](config.get.md)
- [`config.getMany`](config.getMany.md)
- [`config.getSection`](config.getSection.md)
- [`config.set`](config.set.md)
- [`config.setMany`](config.setMany.md)

## Torrents methods
- [`torrents.addFile`](torrents.addFile.md)
- [`torrents.addMagnetLink`](torrents.addMagnetLink.md)
- [`torrents.addUrl`](torrents.addUrl.md)
- [`torrents.details`](torrents.details.md)
- [`torrents.list`](torrents.list.md)
- [`torrents.pause`](torrents.pause.md)
- [`torrents.start`](torrents.start.md)
- [`torrents.stop`](torrents.stop.md)