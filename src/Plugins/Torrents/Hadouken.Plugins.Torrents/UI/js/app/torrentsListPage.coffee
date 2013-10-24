js =
  addTorrentsDialog: '/plugins/core.torrents/js/app/dialogs/addTorrentsDialog.coffee?n'

define ['page', 'rpc', 'eventListener', js.addTorrentsDialog], (Page, RpcClient, EventListener, AddTorrentsDialog) ->
  class TorrentsListPage extends Page
    rpc: new RpcClient()
    eventListener: new EventListener()

    constructor: ->
      super('/plugins/core.torrents/list.html', [ '/torrents' ])

    load: =>
      @eventListener.addHandler 'web.connected', () =>
        @loadTemplates()
        @setupUI()
        @setupEvents()
        @setupTorrents

    loadTemplates: ->

    setupUI: =>
      @content.find('#btn-show-add-torrents').on 'click', (e) =>
        e.preventDefault()
        new AddTorrentsDialog().show

      that = @

      @content.find('#tbody-torrents-list').on 'click', '.btn-torrent-start', (e) ->
        e.preventDefault()
        id = $(@).closest('tr').attr('data-torrent-id')
        that.startTorrent(id)

    setupEvents: =>
      @eventListener.addHandler 'torrent.added', (t) => @torrentAdded(t)
      @eventListener.addHandler 'torrent.paused', (t) => @updateRow(t)
      @eventListener.addHandler 'torrent.started', (t) => @updateRow(t)
      @eventListener.addHandler 'torrent.stopped', (t) => @updateRow(t)
      @eventListener.addHandler 'torrent.removed', (id) => @torrentRemoved(id)

    setupTorrents: =>
      @rpc.call 'torrents.list', (torrents) =>
        for torrent in torrents
          @torrentAdded(torrent)

        @setupIntervalTimer()

    setupIntervalTimer: =>
      callback = => @request()
      @timer = setTimeout(callback, 1000)

    request: =>
      @rpc.call 'torrents.list', (torrents) =>
        @setupIntervalTimer()
        @update(torrents)

    update: (torrents) =>
      for torrent in torrents
        @torrentUpdated(torrent)

    torrentAdded: (torrent) =>
      @addRow(torrent)
      @updateRow(torrent)

    torrentRemoved: (id) =>
      @removeRow(id)

    torrentUpdated: (torrent) =>
      @updateRow(torrent)

    addRow: (torrent) =>
      row = @templates['tmpl-torrent-list-item']({torrent: torrent})
      @content.find('#tbody-torrents-list').append($(row))

    removeRow: (id) =>
      @content.find("#tbody-torrents-list > tr[data-torrent-id=\"#{id}\"]").remove()

    updateRow: (torrent) =>
      row = @content.find("#tbody-torrents-list > tr[data-torrent-id=\"#{torrent.id}\"]")
      progress = torrent.progress | 0

      row.find('.progress-bar').css('width', "#{progress}%")
      row.find('.state').text(torrent.state)

      if torrent.state == 'Downloading'
        row.find('.state-progress').text("#{progress}%")
      else
        row.find('.state-progress').text('')

      row.find('.btn-torrent-start').attr('disabled', @canStart(torrent.state))
      row.find('.btn-torrent-stop').attr('disabled', @canStop(torrent.state))
      row.find('.btn-torrent-pause').attr('disabled', @canPause(torrent.state))

    canStart: -> false

    canStop: -> false

    canPause: -> false

    unload: -> console.log('Unload torrents list page')

  return TorrentsListPage