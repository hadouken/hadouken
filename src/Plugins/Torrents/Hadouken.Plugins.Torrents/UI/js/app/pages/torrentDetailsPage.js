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
            that.content.find('#torrent-details-name').text(torrent.name);
        });
    };

    TorrentDetailsPage.prototype.unload = function() {
        console.log('unloading details');
    };

    return TorrentDetailsPage;
});