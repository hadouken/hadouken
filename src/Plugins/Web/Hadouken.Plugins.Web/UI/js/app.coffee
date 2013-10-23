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
})

requirejs ['jquery', 'bootstrap', 'bootstrapper'], ($, t, Bootstrapper) ->
  bs = new Bootstrapper()
  bs.load()