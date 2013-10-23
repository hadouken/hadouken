define ['eventListener'], (EventListener) ->
  class Bootstrapper
    eventListener = new EventListener()

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