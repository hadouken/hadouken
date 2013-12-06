define(['rpcClient', 'eventListener', 'page', '/plugins/core.torrents/js/app/dialogs/addTorrentsDialog.js'], function(RpcClient, EventListener, Page, AddTorrentsDialog) {
    function TorrentsListPage() {
        Page.call(this, '/plugins/core.torrents/list.html', '/torrents');
        
        this.rpc = new RpcClient();
        this.eventListener = new EventListener();
    }

    Page.derive(TorrentsListPage);

    TorrentsListPage.prototype.load = function () {
        var that = this;
        
        // Event handlers
        this.content.find('#btn-show-add-torrents').on('click', function(e) {
            e.preventDefault();

            var dlg = new AddTorrentsDialog();
            dlg.show();
        });
        
        // Setup events
        this.eventListener.subscribe('torrent.added', function (e) { that.torrentAdded(e.data); });
        this.eventListener.subscribe('torrent.removed', function (e) { that.torrentRemoved(e.data); });

        // Connect the event listener
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