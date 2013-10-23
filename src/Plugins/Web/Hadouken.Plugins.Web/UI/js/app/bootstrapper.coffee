define ['eventListener', 'pageManager'], (EventListener, PageManager) ->
  class Bootstrapper
    eventListener: new EventListener()
    pageManager: PageManager.getInstance()

    constructor: ->

    init: ->
      eventListener.addHandler('web.connected', () => @load())
      eventListener.connect()

    load: ->
      @loadPages()
      @loadPlugins()

    loadPages: ->
      console.log('loading pages')

    loadPlugins: ->
      console.log('loading plugins')

  return Bootstrapper