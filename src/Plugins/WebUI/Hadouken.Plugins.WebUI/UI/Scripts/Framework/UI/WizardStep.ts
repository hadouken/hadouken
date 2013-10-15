module Hadouken.UI {
    export class WizardStep {
        private _url: string;

        public name: string;
        public content: any = null;
        public isDataLoaded: boolean = false;

        constructor(url: string, name: string) {
            this._url = url;
            this.name = name;
        }

        public load(callback: any): void {
            console.log('loading step: ' + this.name);

            if (this.content !== null) {
                this.onshown();

                callback();
            } else {
                $.get(this._url, (html) => {
                    this.content = $(html);
                    this.onshown();

                    callback();
                });
            }
        }

        public onshown(): void { }

        public saveData(callback: any): void { }

        public loadData(callback: any): void { }
    }
}