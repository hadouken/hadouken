///<reference path="WizardStep.ts"/>

module Hadouken.UI {
    export class Wizard {
        private _currentStep: number = -1;

        private steps: Array<WizardStep> = [];
        public content: any;

        constructor(public title: string) { }

        public show(): void {
            // Load the html for the wizard
            $.get('/wizards/wizard.html', (html) => {
                this.content = $(html);
                this.content.find('.wizard-title').text(this.title);

                this.loadSteps(() => {
                    this.content.modal();
                    this.nextStep();
                });
            });
        }

        private loadSteps(callback: any): void {
            var loadedSteps = 0;

            var loadCallback = () => {
                loadedSteps += 1;

                if (loadedSteps == this.steps.length) {
                    callback();
                }
            };

            for (var i = 0; i < this.steps.length; i++) {
                var step = this.steps[i];
                step.load(loadCallback);
            }
        }

        public close(): void {
            this.content.modal('hide');
        }

        public addStep(step: WizardStep): void {
            this.steps.push(step);
        }

        private nextStep(): void {
            this._currentStep += 1;

            // If this is the last step, request the step to save its data, then close the dialog.
            if (this.steps.length > this._currentStep) {
            }

            var body = this.content.find('.modal-body');
            body.empty();
            body.append(this.steps[this._currentStep].content);

            // Update button
            var nextButton = this.content.find('.btn-next-step');
            nextButton.off('click');

        }

        private prevStep(): void { }
    }
}