define ['jquery'], ($) ->
  class Page
    content: ''

    constructor: (@url, @routes) ->

    init: =>
      $.get(@url, (html) =>
        @content = $(html)
        $('#page-container').empty().append(@content)

        @load()
      )

    load: ->

    unload: ->

  return Page