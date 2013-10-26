define ['jquery'], ($) ->
  class Overlay
    target: null

    constructor: (@icon = 'icon-refresh loading') ->

    show: (t) =>
      @target = $(t);
      
      html = $('<div class="overlay"><div class="message"><i></i></div></div>')
      html.find('i').addClass @icon

      @target.prepend(html)

    hide: =>
      @target.find('.overlay').remove()

  return Overlay