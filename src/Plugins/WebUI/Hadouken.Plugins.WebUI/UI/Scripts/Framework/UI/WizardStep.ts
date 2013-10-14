module Hadouken.UI {
    export class WizardStep {
        private _url: string;

        public name: string;
        public content: any;

        constructor(url: string, name: string) {
            this._url = url;
            this.name = name;
        }

        public load(callback: any): void {
            console.log('loading step: ' + this.name);

            $.get(this._url, (html) => {
                this.content = $(html);
                callback();
            });
        }

        public onloaded(): void { }

        public saveData(callback: any): void { }

        public loadData(callback: any): void { }
    }
}