///<reference path="../UI/WizardStep.ts"/>

module Hadouken.Plugins {
    export class Plugin {
        load(): void { }

        configure(): void { }

        initialConfiguration(): Hadouken.UI.WizardStep {
            return null;
        }

        unload(): void { }
    }
}