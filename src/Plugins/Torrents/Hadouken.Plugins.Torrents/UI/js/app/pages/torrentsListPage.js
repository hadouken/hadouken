define(['rpcClient', 'eventListener', 'page', 'handlebars', '/plugins/core.torrents/js/app/dialogs/addTorrentsDialog.js'], function(RpcClient, EventListener, Page, Handlebars, AddTorrentsDialog) {
    function TorrentsListPage() {
        Page.call(this, '/plugins/core.torrents/list.html', '/torrents');
        
        this.rpc = new RpcClient();
        this.eventListener = new EventListener();
    }

    Page.derive(TorrentsListPage);

    TorrentsListPage.prototype.load = function () {
        var that = this;

        // Register template helpers
        Handlebars.registerHelper('progress', function(torrent) {
            that.progressStatus(torrent);
        });

        // Load template
        var templateHtml = this.content.find('#tmpl-torrent-list-item').html();
        this.template = Handlebars.compile(templateHtml);
        
        
        // Event handlers
        this.content.find('#btn-show-add-torrents').on('click', function(e) {
            e.preventDefault();

            var dlg = new AddTorrentsDialog();
            dlg.show();
        });

        this.content.find('#tbody-torrents-list').on('click', '.btn-torrent-start', function () {
            var torrentId = $(this).parents('tr').attr('data-torrent-id');
            that.startTorrent(torrentId);
        });

        this.content.find('#tbody-torrents-list').on('click', '.btn-torrent-pause', function () {
            var torrentId = $(this).parents('tr').attr('data-torrent-id');
            that.pauseTorrent(torrentId);
        });

        this.content.find('#tbody-torrents-list').on('click', '.btn-torrent-stop', function () {
            var torrentId = $(this).parents('tr').attr('data-torrent-id');
            that.stopTorrent(torrentId);
        });

        this.content.find('#tbody-torrents-list').on('click', '.btn-torrent-remove', function () {
            var torrentId = $(this).parents('tr').attr('data-torrent-id');
            that.removeTorrent(torrentId, false);
        });

        this.content.find('#tbody-torrents-list').on('click', '.btn-torrent-remove-data', function () {
            var torrentId = $(this).parents('tr').attr('data-torrent-id');
            that.removeTorrent(torrentId, true);
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

    TorrentsListPage.prototype.progressStatus = function(torrent) {
        if (torrent.state === 'Downloading' || torrent.state == 'Hashing') {
            var progress = torrent.progress | 0;
            return ' (' + progress + '%)';
        }

        return '';
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

        for (var i = 0; i < torrents.length; i++) {
            this.updateRow(torrents[i]);
        }
    };

    TorrentsListPage.prototype.updateRow = function (torrent) {
        var row = this.content.find('tr[data-torrent-id=' + torrent.id + ']');
        var progress = this.progressStatus(torrent);

        row.find('.state').text(torrent.state);
        row.find('.state-progress').text(progress);
        row.find('.progress-bar').width(torrent.progress + '%');

        row.find('.btn-torrent-start').attr('disabled', !this.canStart(torrent));
        row.find('.btn-torrent-pause').attr('disabled', !this.canPause(torrent));
        row.find('.btn-torrent-stop').attr('disabled', !this.canStop(torrent));
    };

    TorrentsListPage.prototype.torrentAdded = function(torrent) {
        var row = this.template({ torrent: torrent });
        this.content.find('#tbody-torrents-list').append($(row));
    };

    TorrentsListPage.prototype.torrentRemoved = function(infoHash) {
        this.content.find('tr[data-torrent-id=' + infoHash + ']').remove();
    };

    TorrentsListPage.prototype.startTorrent = function(infoHash) {
        this.rpc.callParams('torrents.start', infoHash, function() {});
    };

    TorrentsListPage.prototype.pauseTorrent = function(infoHash) {
        this.rpc.callParams('torrents.pause', infoHash, function() {});
    };

    TorrentsListPage.prototype.stopTorrent = function(infoHash) {
        this.rpc.callParams('torrents.stop', infoHash, function() {});
    };

    TorrentsListPage.prototype.removeTorrent = function(infoHash, removeData) {
        this.rpc.callParams('torrents.remove', [infoHash, removeData], function() {});
    };

    TorrentsListPage.prototype.canStart = function(torrent) {
        switch(torrent.state) {
            case 'Stopped':
            case 'Paused':
                return true;
        }

        return false;
    };

    TorrentsListPage.prototype.canPause = function(torrent) {
        switch(torrent.state) {
            case 'Downloading':
            case 'Seeding':
                return true;
        }

        return false;
    };

    TorrentsListPage.prototype.canStop = function(torrent) {
        switch(torrent.state) {
            case 'Downloading':
            case 'Seeding':
            case 'Paused':
                return true;
        }

        return false;
    };

    return TorrentsListPage;
});