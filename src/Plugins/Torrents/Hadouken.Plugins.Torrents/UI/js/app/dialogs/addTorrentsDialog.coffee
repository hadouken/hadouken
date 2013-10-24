define ['dialog'], (Dialog) ->
  class AddTorrentsDialog extends Dialog
    constructor: ->
      super('/plugins/core.torrents/dialogs/addTorrents.html')

    onShow: =>
      @setupEvents()

    setupEvents: =>
      @content.find('#btn-add-torrents').on 'click', (e) =>
        e.preventDefault()
        @addTorrents()

    addTorrents: =>
      @disableAdd()
      console.log('adding torrents')

    disableAdd: =>
      @content.find('#btn-add-torrents').attr 'disabled', true

    enableAdd: =>
      @content.find('#btn-add-torrents').attr 'disabled', false