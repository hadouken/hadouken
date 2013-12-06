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
        this.eventListener.subscribe('torrent.added', function (e) { that.torrentAdded(e); });
        this.eventListener.subscribe('torrent.removed', function (e) { that.torrentRemoved(e); });

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
        
        this.rpc.call('torrents.list', function (torrents) {
            that.loadTorrents(torrents);
            
            clearTimeout(that.timer);
            that.setupTimer(1000);
        });
    };

    TorrentsListPage.prototype.loadTorrents = function(torrents) {
        if (typeof torrents === 'undefined' || torrents === null || torrents.length === 0) {
            return;
        }
    };

    TorrentsListPage.prototype.torrentAdded = function(torrent) {
        console.log('added: ' + torrent.name);
    };

    TorrentsListPage.prototype.torrentRemoved = function(infoHash) {
        console.log('removed: ' + infoHash);
    };

    return TorrentsListPage;
});