define(['rpcClient', 'eventListener', 'page', 'handlebars', '/plugins/core.torrents/js/app/dialogs/addTorrentsDialog.js'], function(RpcClient, EventListener, Page, Handlebars, AddTorrentsDialog) {
    function TorrentsListPage() {
        Page.call(this, '/plugins/core.torrents/list.html', '/torrents');
        
        this.rpc = new RpcClient();
        this.eventListener = new EventListener();
    }

    Page.derive(TorrentsListPage);

    TorrentsListPage.prototype.load = function () {
        // Load template
        var templateHtml = this.content.find('#tmpl-torrent-list-item').html();
        this.template = Handlebars.compile(templateHtml);
        
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
        
        // Get a list of all torrents and add them to the page
        this.rpc.call('torrents.list', function(result) {
            for (var i = 0; i < result.length; i++) {
                that.torrentAdded(result[i]);
            }
            
            // Connect the event listener
            that.eventListener.connect(function () {
                that.setupTimer(1);
            });
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
        var row = this.template({ torrent: torrent });
        this.content.find('#tbody-torrents-list').append($(row));
    };

    TorrentsListPage.prototype.torrentRemoved = function(infoHash) {
        console.log('removed: ' + infoHash);
    };

    return TorrentsListPage;
});