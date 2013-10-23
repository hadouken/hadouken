define ['jquery', 'rpc'], ($, RpcClient) ->
  class PluginEngine
    @instance: null

    rpc: new RpcClient()
    plugins: {}

    constructor: ->
      if PluginEngine.instance then throw new Error()

    @getInstance: ->
      if not PluginEngine.instance?
        PluginEngine.instance = new PluginEngine()

      return PluginEngine.instance

    load: (callback) =>
      @rpc.call 'plugins.list', (p) =>
        for plugin in p
          console.log(plugin.name)
          @plugins[plugin.name] = plugin

        @loadPluginScripts(callback)

    loadPluginScripts: (callback) =>
      files = 0
      requestCallback = () =>
        files += 1
        if files == Object.keys(@plugins).length then callback()

      for key of @plugins
        plugin = @plugins[key]

        if plugin.name == 'core.web'
          requestCallback()
          continue

        $.getScript("/plugins/#{plugin.name}/boot.coffee")
          .done(requestCallback)
          .fail(requestCallback)        

  return PluginEngine