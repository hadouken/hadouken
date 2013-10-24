js =
  torrentsListPage: '/plugins/core.torrents/js/app/torrentsListPage.coffee?n'

require ['jquery', js.torrentsListPage], ($, TorrentsListPage) ->
  p = new TorrentsListPage()
  p.load()