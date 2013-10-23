requirejs.config({
  baseUrl: 'js/lib'
  paths:
    bootstrapper: '../app/bootstrapper.coffee?n'
    eventListener: '../app/eventListener.coffee?n'
    pageManager: '../app/pageManager.coffee?n'
})

requirejs ['jquery', 'bootstrapper'], ($, Bootstrapper) ->
  bs = new Bootstrapper()
  bs.load()