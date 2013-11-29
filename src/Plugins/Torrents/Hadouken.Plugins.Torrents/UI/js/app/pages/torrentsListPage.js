define(function() {
    function TorrentsListPage() {
        this.routes = [
            '/torrents'
        ];
    }

    TorrentsListPage.prototype.load = function() {
        console.log('loading torrents list');
    };

    TorrentsListPage.prototype.unload = function() {
        console.log('unloading torrents list');
    };

    return TorrentsListPage;
});