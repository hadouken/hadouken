define ['signals', 'hasher', 'crossroads'], (s, hasher, crossroads) ->
  class PageManager
    @instance: null
    current: null

    constructor: ->
      if PageManager.instance then throw new Error()

    @getInstance: ->
      if not PageManager.instance?
        PageManager.instance = new PageManager()
        PageManager.instance.init()

      return PageManager.instance

    init: ->
      if location.hash == '' then hasher.setHash('dashboard')

      hasher.initialized.add((n, o) => @parseHash(n, o))
      hasher.changed.add((n, o) => @parseHash(n, o))
      hasher.init()

      console.log('pagemanager init')

    parseHash: (newHash, oldHash) ->
      crossroads.parse(newHash)

    addPage: (page) =>
      for r in page.routes
        crossroads.addRoute(r, () => @route(page))

    route: (page) ->
      @current.unload() if @current?
      @current = page

      page.init(arguments)

  return PageManager