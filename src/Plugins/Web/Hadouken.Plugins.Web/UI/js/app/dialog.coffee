define ['jquery', 'bootstrap'], ($) ->
  class Dialog
    content: ''

    constructor: (@url) ->

    show: =>
      $.get @url, (html) =>
        @content = $(html)
        
        @content.on 'shown.bs.modal', () =>
          @onShow()

        @content.modal()

        that = @

        @content.on 'hidden.bs.modal', () ->
          $(@).remove()
          that.onClosed()

    close: =>
        @content.modal 'hide'

    onShow: ->

    onClosed: ->

  return Dialog