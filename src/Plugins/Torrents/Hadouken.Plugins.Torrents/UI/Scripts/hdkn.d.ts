declare module Hadouken.Events {
    class EventListener {
        addHandler(name: string, callback: { (data: any): void; }): void;
        clearHandlers(): void;
        connect(): void;
        disconnect(): void;
        sendEvent(name: string, data: any): void;
    }
}

declare module Hadouken.UI {
    class Page {
        content: any;

        constructor(url: string, routes: Array<string>);
        init(): void;
        load(): void;
    }

    class PageManager {
        constructor();
        public static getInstance(): PageManager;
        addPage(page: any): void;
    }

    class Dialog {
        constructor(url: string);
        show();
        onShow();

        getContent(): any;
    }
}

declare module Hadouken.Plugins {
    class Plugin {
        load(): void;
        unload(): void;
        configure(): void;
    }

    class PluginEngine {
        public static getInstance(): PluginEngine;
        public setPlugin(id: string, plugin: Plugin): void;
    }
}

declare module Hadouken.Http {
    class JsonRpcClient {
        constructor(url: string);
        call(method: string, callback: { (result: any): void; }): void;
        callParams(method: string, params: any, callback: { (result: any): void; }): void;
    }
}