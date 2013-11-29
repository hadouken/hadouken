(function() {
    function Factory(callback) {
        var js = {
            pages: {
                torrentsList: '/plugins/core.torrents/js/app/pages/torrentsListPage.js'
            }
        };

        require(['jquery', 'pageManager', js.pages.torrentsList], function ($, PageManager, TorrentsListPage) {
            var pageManager = PageManager.getInstance();
            pageManager.addPage(new TorrentsListPage());

            var menuItem = $('<li><a href="#/torrents">Torrents</a></li>');
            $('#main-menu').append(menuItem);

            callback();
        });
    }

    return Factory;
})();