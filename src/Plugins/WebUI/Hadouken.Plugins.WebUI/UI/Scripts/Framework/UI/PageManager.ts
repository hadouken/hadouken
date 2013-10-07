///<reference path="Page.ts"/>

module Hadouken.UI {
    export class PageManager {
        private static _instance: PageManager = null;
        private _pages: Array<Page> = [];

        constructor() {
            if (PageManager._instance) {
                throw new Error();
            }

            PageManager._instance = this;
        }

        private setup(): void {
        }

        public init(): void {
            hasher.initialized.add(crossroads.parse, crossroads);
            hasher.changed.add(crossroads.parse, crossroads);
            hasher.init();
        }

        public static getInstance(): PageManager {
            if (PageManager._instance === null) {
                PageManager._instance = new PageManager();
                PageManager._instance.setup();
            }

            return PageManager._instance;
        }

        addPage(page: Hadouken.UI.Page): void {
            for (var i = 0; i < page.routes.length; i++) {
                var route = page.routes[i];
                crossroads.addRoute(route, () => {
                    page.init();
                });
            }

            this._pages.push(page);
        }
    }
}