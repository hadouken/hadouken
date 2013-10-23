class Plugin
  load: -> console.log('load')

  unload: -> console.log('unload')

return new Plugin