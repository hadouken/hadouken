module Hadouken.UI {
    export class Overlay {
        private _target: any;

        constructor(public icon: string) { }

        public show(target: any): void {
            this._target = target;

            var html = $('<div class="overlay"><div class="message"><i></i></div></div>');
            html.find('i').addClass(this.icon);

            this._target.prepend(html);
        }

        public hide() {
            this._target.find('.overlay').remove();
        }
    }
}