define ['eventListener'], (EventListener) ->
  class Bootstrapper
    eventListener = new EventListener()

    constructor: ->
      @even

    load: ->
      eventListener.connect()

  return Bootstrapper