pluginFactory = (callback) ->
  js =
    torrentsListPage: '/plugins/core.torrents/js/app/torrentsListPage.coffee?n'

  require ['jquery', 'pageManager', js.torrentsListPage], ($, PageManager, TorrentsListPage) ->
    console.log 'loading plugin'

    pm = PageManager.getInstance()
    pm.addPage(new TorrentsListPage())

    a = $('<li><a href="#/torrents">Torrents</a></li>')
    $('#main-menu').append(a)

    callback()

return pluginFactory