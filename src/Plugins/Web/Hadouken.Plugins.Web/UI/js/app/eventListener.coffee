define ['jquery', 'jquery.signalr'], ($) ->
  class EventListener
    connection: $.hubConnection('/events')
    proxy: null
    handlers: {}

    constructor: ->
      console.log('EventListener')

    connect: ->
      @proxy = @connection.createHubProxy('events');
      @connection.start().done(() => @publishEvent({name: 'web.connected'}))

    disconnect: ->
      @connection.stop()
      @clearHandlers()

    addHandler: (name, callback) ->
      @handlers[name] = [] if not @handlers[name]?
      @handlers[name].push(callback)

    publishEvent: (event) ->
      return unless @handlers[event.name]?

      for handler in @handlers[event.name]
        handler(event.data)

    clearHandlers: ->
      for key of @handlers
        for handler, i in @handlers[key]
          delete @handlers[key][i]
      
  return EventListener
      