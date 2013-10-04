///<reference path="Framework/Bootstrapper.ts"/>
///<reference path="Framework/Events/EventListener.ts"/>
///<reference path="Framework/Plugins/PluginEngine.ts"/>

// include UI files
///<reference path="Framework/UI/Dialog.ts"/>
///<reference path="Framework/UI/Page.ts"/>

$(document).ready(function () {
    var eventListener = new Hadouken.Events.EventListener();
    var pluginEngine = new Hadouken.Plugins.PluginEngine();

    new Hadouken.Bootstrapper()
        .init(eventListener, pluginEngine);
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