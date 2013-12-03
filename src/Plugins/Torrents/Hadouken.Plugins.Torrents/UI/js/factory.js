(function() {
    function Factory(callback) {
        var plugin = '/plugins/core.torrents/js/app/torrentsPlugin.js';

        require([plugin], function (TorrentsPlugin) {
            callback(new TorrentsPlugin());
        });
    }

    return Factory;
})();