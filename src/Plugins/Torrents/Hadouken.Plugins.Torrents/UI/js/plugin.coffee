class Plugin
  pageManager: Hadouken.UI.PageManager.getInstance()

  load: =>
    console.log @pageManager.current

  unload: -> console.log('unload')

return new Plugin