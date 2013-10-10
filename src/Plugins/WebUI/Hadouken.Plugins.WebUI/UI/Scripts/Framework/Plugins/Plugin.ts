module Hadouken.Plugins {
    export class Plugin {
        load(): void { }

        configure(): void { }

        loadFirstTimeSetup(container: any): void { }

        saveFirstTimeSetup(container: any, callback: { (): void; }): void { }

        unload(): void { }
    }
}