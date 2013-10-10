///<reference path="libs.d.ts"/>

///<reference path="Framework/Bootstrapper.ts"/>
///<reference path="Framework/Events/EventListener.ts"/>
///<reference path="Framework/Plugins/PluginEngine.ts"/>

// include UI files
///<reference path="Framework/UI/Dialog.ts"/>
///<reference path="Framework/UI/Page.ts"/>
///<reference path="Framework/UI/PageManager.ts"/>

$(document).ready(function () {
    // Register Handlebars templates
    Handlebars.registerHelper('fileSize', function (size) {
        var i = Math.floor(Math.log(size) / Math.log(1024));
        return (size / Math.pow(1024, i)).toFixed(2) + ' ' + ['B', 'kB', 'MB', 'GB', 'TB'][i];
    });

    var eventListener = new Hadouken.Events.EventListener();
    var pluginEngine = new Hadouken.Plugins.PluginEngine();
    var pageManager = Hadouken.UI.PageManager.getInstance();

    new Hadouken.Bootstrapper()
        .init(eventListener, pluginEngine, pageManager);
});