window.Hadouken = {}
Hadouken.Events = {}
Hadouken.Plugins = {}
Hadouken.UI = {}

requirejs.config({
  baseUrl: 'js/lib'
  paths:
    bootstrapper: '../app/bootstrapper.coffee?n'
    eventListener: '../app/eventListener.coffee?n'
    pageManager: '../app/pageManager.coffee?n'
    page: '../app/page.coffee?n'
    settingsPage: '../app/pages/settings.coffee?n'
    rpc: '../app/rpc.coffee?n'
    pluginEngine: '../app/pluginEngine.coffee?n'
    dialog: '../app/dialog.coffee?n'
    overlay: '../app/overlay.coffee?n'
})

requirejs ['jquery', 'bootstrap', 'overlay', 'bootstrapper'], ($, t, Overlay, Bootstrapper) ->
  ol = new Overlay()
  ol.show('body')

  bs = new Bootstrapper()
  bs.init(ol)