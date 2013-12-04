var js = {
    dialogs: {
        configure: '/plugins/core.torrents/js/app/dialogs/configureDialog.js'
    },
    pages: {
        torrentsList: '/plugins/core.torrents/js/app/pages/torrentsListPage.js',
        torrentDetails: '/plugins/core.torrents/js/app/pages/torrentDetailsPage.js'
    }
};

define(['jquery', 'pageManager', js.pages.torrentsList, js.pages.torrentDetails, js.dialogs.configure], function ($, PageManager, TorrentsListPage, TorrentDetailsPage, ConfigureDialog) {
    function TorrentsPlugin() {
    }

    TorrentsPlugin.prototype.load = function() {
        var pageManager = PageManager.getInstance();
        pageManager.addPage(new TorrentsListPage());
        pageManager.addPage(new TorrentDetailsPage());

        var menuItem = $('<li id="menuitem-torrents-list"><a href="#/torrents">Torrents</a></li>');
        $('#main-menu').append(menuItem);
    };

    TorrentsPlugin.prototype.unload = function() {
        $('#menuitem-torrents-list').remove();
    };

    TorrentsPlugin.prototype.configure = function() {
        var dialog = new ConfigureDialog();
        dialog.show();
    };

    return TorrentsPlugin;
});