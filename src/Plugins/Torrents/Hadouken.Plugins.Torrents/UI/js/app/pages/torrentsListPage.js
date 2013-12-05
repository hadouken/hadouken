define(['rpcClient', 'eventListener', 'page'], function(RpcClient, EventListener, Page) {
    function TorrentsListPage() {
        Page.call(this, '/plugins/core.torrents/list.html', '/torrents');
        
        this.rpc = new RpcClient();
        this.eventListener = new EventListener();
    }

    Page.derive(TorrentsListPage);

    TorrentsListPage.prototype.load = function () {
        // Setup events
        this.eventListener.subscribe('torrent.added', this.torrentAdded);
        this.eventListener.subscribe('torrent.removed', this.torrentRemoved);

        // Connect the event listener
        var that = this;
        this.eventListener.connect(function() {
            that.setupTimer(1);
        });
    };

    TorrentsListPage.prototype.unload = function () {
        this.eventListener.disconnect();
        clearTimeout(this.timer);
    };

    TorrentsListPage.prototype.setupTimer = function (milliseconds) {
        var that = this;

        this.timer = setTimeout(function() {
            that.fetchTorrents();
        }, milliseconds);
    };

    TorrentsListPage.prototype.fetchTorrents = function () {
        var that = this;
        
        this.rpc.call('torrents.list', function(torrents) {
            clearTimeout(that.timer);
            that.setupTimer(1000);
        });
    };

    TorrentsListPage.prototype.loadTorrents = function(torrents) {

    };

    return TorrentsListPage;
});