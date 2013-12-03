var js = {
    pages: {
        torrentsList: '/plugins/core.torrents/js/app/pages/torrentsListPage.js',
        torrentDetails: '/plugins/core.torrents/js/app/pages/torrentDetailsPage.js'
    }
};

define(['jquery', 'pageManager', js.pages.torrentsList, js.pages.torrentDetails], function ($, PageManager, TorrentsListPage, TorrentDetailsPage) {
    function TorrentsPlugin() {
    }

    TorrentsPlugin.prototype.load = function() {
        var pageManager = PageManager.getInstance();
        pageManager.addPage(new TorrentsListPage());
        pageManager.addPage(new TorrentDetailsPage());

        var menuItem = $('<li><a href="#/torrents">Torrents</a></li>');
        $('#main-menu').append(menuItem);
    };

    TorrentsPlugin.prototype.unload = function() {

    };

    TorrentsPlugin.prototype.configure = function() {
        console.log('tjoho');
    };

    return TorrentsPlugin;
});