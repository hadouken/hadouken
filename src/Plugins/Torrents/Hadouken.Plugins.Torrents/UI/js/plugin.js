(function() {
    function Factory(callback) {
        var js = {
            pages: {
                torrentsList: '/plugins/core.torrents/js/app/pages/torrentsListPage.js',
                torrentDetails: '/plugins/core.torrents/js/app/pages/torrentDetailsPage.js'
            }
        };

        require(['jquery', 'pageManager', js.pages.torrentsList, js.pages.torrentDetails], function ($, PageManager, TorrentsListPage, TorrentDetailsPage) {
            var pageManager = PageManager.getInstance();
            pageManager.addPage(new TorrentsListPage());
            pageManager.addPage(new TorrentDetailsPage());

            var menuItem = $('<li><a href="#/torrents">Torrents</a></li>');
            $('#main-menu').append(menuItem);

            callback();
        });
    }

    return Factory;
})();