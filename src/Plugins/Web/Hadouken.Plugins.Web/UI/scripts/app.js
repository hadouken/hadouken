require.config({
    baseUrl: 'scripts/app',
    
    paths: {
        jquery: '../lib/jquery',
        bootstrap: '../lib/bootstrap',
        'jquery.signalr': '../lib/jquery.signalr',
        signals: '../lib/signals',
        hasher: '../lib/hasher',
        crossroads: '../lib/crossroads',
        handlebars: '../lib/handlebars'
    },
    
    shim: {
        bootstrap: {
            deps: ['jquery']
        },
        handlebars: {
            exports: 'Handlebars'
        }
    }
});

require(['bootstrapper'], function (Bootstrapper) {
    // Lets bootstrap this behemoth
    var bootstrapper = new Bootstrapper();
    bootstrapper.init();
});