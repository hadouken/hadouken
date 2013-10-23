requirejs.config({
  baseUrl: 'js/lib'
  paths:
    bootstrapper: '../app/bootstrapper.coffee?n'
})

requirejs ['jquery', 'bootstrapper'], ($, Bootstrapper) ->
  bs = new Bootstrapper()
  bs.load()