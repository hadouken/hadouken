define () ->
  class Bootstrapper
    constructor: ->
      console.log('creating bootstrapper')

    load: ->
      console.log('load')

  return Bootstrapper