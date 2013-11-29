define(['page'], function(Page) {
    function SettingsPage() {
        Page.call(this, '/settings.html', '/settings');
    }

    SettingsPage.prototype = new Page();
    SettingsPage.prototype.constructor = SettingsPage;

    SettingsPage.prototype.load = function() {
        console.log('loading settings page');
    };

    SettingsPage.prototype.unload = function() {
        console.log('unloading settings page');
    };

    return SettingsPage;
});