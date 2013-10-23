define ['page'], (Page) ->
  class SettingsPage extends Page
    constructor: ->
      super('/settings.html', [ '/settings' ])

    load: -> console.log('SettingsPage::load')

    unload: -> console.log('SettingsPage::unload')

  return SettingsPage