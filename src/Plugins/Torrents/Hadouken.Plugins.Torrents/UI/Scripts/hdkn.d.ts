declare module Hadouken.Events {
    class EventListener {
        addHandler(name: string, callback: { (data: any): void; }): void;
        connect(): void;
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

declare module Hadouken.Http {
    class JsonRpcClient {
        constructor(url: string);
        call(method: string, callback: { (result: any): void; }): void;
        callParams(method: string, params: any, callback: { (result: any): void; }): void;
    }
}