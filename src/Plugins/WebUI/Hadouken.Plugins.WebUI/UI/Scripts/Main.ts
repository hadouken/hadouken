///<reference path="Framework/Bootstrapper.ts"/>
///<reference path="Framework/Events/EventListener.ts"/>
///<reference path="Framework/Plugins/PluginEngine.ts"/>

// include UI files
///<reference path="Framework/UI/Dialog.ts"/>
///<reference path="Framework/UI/Page.ts"/>
///<reference path="Framework/UI/PageManager.ts"/>

$(document).ready(function () {
    var eventListener = new Hadouken.Events.EventListener();
    var pluginEngine = new Hadouken.Plugins.PluginEngine();
    var pageManager = Hadouken.UI.PageManager.getInstance();

    new Hadouken.Bootstrapper()
        .init(eventListener, pluginEngine, pageManager);
});