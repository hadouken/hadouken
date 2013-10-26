define ['eventListener', 'pageManager', 'pluginEngine', 'settingsPage' ], (EventListener, PageManager, PluginEngine, SettingsPage) ->
  class Hadouken.Bootstrapper
    eventListener: new EventListener()
    pageManager: PageManager.getInstance()
    pluginEngine: PluginEngine.getInstance()
    overlay: null

    constructor: ->

    init: (o) =>
      @overlay = o

      @eventListener.addHandler('web.connected', () => @load())
      @eventListener.connect()

    load: =>
      @loadPages()
      @loadPlugins()

    loadPages: =>
      @pageManager.addPage(new SettingsPage)

    loadPlugins: =>
      @pluginEngine.load () =>
        @overlay.hide()

  return Hadouken.Bootstrapper