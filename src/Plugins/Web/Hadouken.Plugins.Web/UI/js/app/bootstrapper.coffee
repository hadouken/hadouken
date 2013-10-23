define ['eventListener', 'pageManager', 'settingsPage' ], (EventListener, PageManager, SettingsPage) ->
  class Bootstrapper
    eventListener: new EventListener()
    pageManager: PageManager.getInstance()

    constructor: ->

    init: =>
      eventListener.addHandler('web.connected', () => load())
      eventListener.connect()

    load: =>
      @loadPages()
      @loadPlugins()

    loadPages: =>
      @pageManager.addPage(new SettingsPage)

    loadPlugins: ->
      console.log('loading plugins')

  return Bootstrapper