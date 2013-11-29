define(['page'], function(Page) {
    function TorrentsListPage() {
        Page.call(this, '/plugins/core.torrents/list.html', '/torrents');
    }

    TorrentsListPage.prototype = new Page();
    TorrentsListPage.prototype.constructor = TorrentsListPage;

    TorrentsListPage.prototype.load = function() {
        console.log('loading torrents list');
    };

    TorrentsListPage.prototype.unload = function() {
        console.log('unloading torrents list');
    };

    return TorrentsListPage;
});