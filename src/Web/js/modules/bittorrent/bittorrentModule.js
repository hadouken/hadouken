'use strict';

angular.module('hadouken.bittorrent', [
    'ui.router',
    'hadouken.messaging',
    'hadouken.bittorrent.controllers.torrentAdd',
    'hadouken.bittorrent.controllers.torrentDetails',
    'hadouken.bittorrent.controllers.torrentList',
    'hadouken.bittorrent.controllers.torrentMove',
    'hadouken.bittorrent.controllers.settings'
])
.config(['$stateProvider', function ($stateProvider) {
    $stateProvider
        .state('ui.bittorrentList', {
            controller: 'BitTorrent.TorrentListController',
            url: '/torrents/list',
            templateUrl: 'views/bittorrent/list.html'
        });
}]);