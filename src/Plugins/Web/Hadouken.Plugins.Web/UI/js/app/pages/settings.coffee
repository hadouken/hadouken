define ['jquery', 'page', 'rpc'], ($, Page, RpcClient) ->
  class SettingsPage extends Page
    rpc: new RpcClient()

    constructor: ->
      super('/settings.html', [ '/settings' ])

    load: =>
      @loadGeneral()
      @loadPlugins()

    loadGeneral: ->
      @content.find('#btn-change-auth').on 'click', (e) ->
        e.preventDefault()
        console.log('opening dialog')

    loadPlugins: ->
      keys = [ 'plugins.repositoryUrl', 'plugins.enableUpdateChecking' ]

      @rpc.callParams 'config.getMany', keys, (result) =>
        @content.find('#repositoryUrl').val(result['plugins.repositoryUrl'])
        @content.find('#enableUpdateChecking').attr('checked', result['plugins.enableUpdateChecking'])

        @content.find('#btn-save-plugin-settings').on 'click', (e) ->
          console.log('saving plugin settings')

        # render all plugins

    unload: -> console.log('SettingsPage::unload')

  return SettingsPage