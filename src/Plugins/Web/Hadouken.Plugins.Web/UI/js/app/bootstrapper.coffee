define ['eventListener', 'pageManager', 'pluginEngine', 'settingsPage' ], (EventListener, PageManager, PluginEngine, SettingsPage) ->
  class Bootstrapper
    eventListener: new EventListener()
    pageManager: PageManager.getInstance()
    pluginEngine: PluginEngine.getInstance()

    constructor: ->

    init: =>
      eventListener.addHandler('web.connected', () => load())
      eventListener.connect()

    load: =>
      @loadPages()
      @loadPlugins()

    loadPages: =>
      @pageManager.addPage(new SettingsPage)

    loadPlugins: =>
      @pluginEngine.load () ->
        console.log('plugins loaded')

  return Bootstrapper