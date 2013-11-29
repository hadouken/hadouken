define(function() {
    function SettingsPage() {
        this.routes = [
            '/settings'
        ];
    }

    SettingsPage.prototype.load = function() {
        console.log('loading settings page');
    };

    SettingsPage.prototype.unload = function() {
        console.log('unloading settings page');
    };

    return SettingsPage;
});