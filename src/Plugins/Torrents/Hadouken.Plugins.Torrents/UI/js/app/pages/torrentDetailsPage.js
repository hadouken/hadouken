define(['rpcClient', 'eventListener', 'page'], function(RpcClient, EventListener, Page) {
    function TorrentDetailsPage() {
        Page.call(this, '/plugins/core.torrents/details.html', '/torrents/{id}');

        this.rpc = new RpcClient();
        this.eventListener = new EventListener();
    }

    Page.derive(TorrentDetailsPage);

    TorrentDetailsPage.prototype.load = function (id) {
        var that = this;
        this.rpc.callParams('torrents.details', id, function (torrent) {
            that.update(torrent);
            that.timer = setTimeout(function() {
                that.fetchTorrent(id);
            }, 1000);
        });
    };

    TorrentDetailsPage.prototype.fetchTorrent = function(id) {
        var that = this;

        this.rpc.callParams('torrents.details', id, function(torrent) {
            clearTimeout(that.timer);

            that.update(torrent);

            that.timer = setTimeout(function() {
                that.fetchTorrent(id);
            }, 1000);
        });
    };

    TorrentDetailsPage.prototype.update = function(torrent) {
        this.content.find('#torrent-details-name').text(torrent.name);
        this.content.find('#files-count').text(torrent.files.length);

        var progress = torrent.progress | 0;
        this.content.find('#torrent-details-progress').width(progress + '%');

        this.content.find('#torrent-savepath').text(torrent.savePath);
        this.content.find('#torrent-size').text(torrent.size);
        this.content.find('#torrent-label').text(torrent.label);
        this.content.find('#torrent-state').text(torrent.state);

        var progressText = '';

        switch(torrent.state) {
            case 'Downloading':
            case 'Hashing':
                progressText = ' (' + progress + '%)';
                break;
        }

        this.content.find('#torrent-state-progress').text(progressText);

    };

    TorrentDetailsPage.prototype.unload = function() {
        clearTimeout(this.timer);
    };

    return TorrentDetailsPage;
});