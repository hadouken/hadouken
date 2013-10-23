define ['jquery', 'jquery.signalr'], ($, signalR) ->
  class EventListener
    connection: $.hubConnection('/events')
    proxy: null

    constructor: ->
      console.log('EventListener')

    connect: =>
      @proxy = @connection.createHubProxy('events');
      @connection.start().done(() => @publishEvent({name: 'web.connected'}))

    disconnect: =>
      @connection.stop()

    publishEvent: (event) ->
      console.log(event.name)

  return EventListener
      