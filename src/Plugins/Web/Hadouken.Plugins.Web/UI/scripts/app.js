require.config({
    baseUrl: 'scripts/app',
    
    paths: {
        jquery: '../lib/jquery',
        'jquery.signalr': '../lib/jquery.signalr',
        signals: '../lib/signals',
        hasher: '../lib/hasher',
        crossroads: '../lib/crossroads'
    }
});

require(['bootstrapper'], function (Bootstrapper) {
    // Lets bootstrap this behemoth
    var bootstrapper = new Bootstrapper();
    bootstrapper.init();
});