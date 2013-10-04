///<reference path="Framework/Bootstrapper.ts"/>

$(document).ready(function () {
    new Hadouken.Bootstrapper().init();
});

/*
// Real setup below
window.addEvent('domready', function () {
    $("overlay").show();

    Hadouken.init();

    var host = location.hostname;
    var port = parseInt(location.port) + 1;

    var signalrConnection = jQuery.hubConnection('http://' + host + ':' + port);
    var eventsProxy = signalrConnection.createHubProxy('events');

    eventsProxy.on('publishEvent', function(event) {
        if (event.name == "plugin.loaded" || event.name == "plugin.unloaded")
            location.reload();

        console.log(event.name);
    });

    signalrConnection.start().done(function() { console.log('connected') });

    $("overlay").hide();
});
*/