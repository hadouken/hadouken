define ['dialog', 'rpc'], (Dialog, RpcClient) ->
  class AddTorrentsDialog extends Dialog
    rpc: new RpcClient()

    constructor: ->
      super('/plugins/core.torrents/dialogs/add-torrents.html')

    onShow: =>
      @setupEvents()

    setupEvents: =>
      @content.find('#btn-add-torrents').on 'click', (e) =>
        e.preventDefault()
        @addTorrents()

    addTorrents: =>
      @disableAdd()
      
      fileInput = @content.find('#torrent-files')[0]
      reader = new FileReader()
      filesAdded = 0

      reader.onload = (e) =>
        data = [ e.target.result.split(',')[1], '', '' ]

        @rpc.callParams 'torrents.addFile', data, =>
          filesAdded += 1

          if filesAdded == fileInput.files.length
            @enableAdd()
            @content.modal('hide')

      for file in fileInput.files
        reader.readAsDataURL(file)

    disableAdd: =>
      @content.find('#btn-add-torrents').attr 'disabled', true

    enableAdd: =>
      @content.find('#btn-add-torrents').attr 'disabled', false

  return AddTorrentsDialog