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

                this.loadSteps(() => {
                    this.content.modal();

                    // Load data for first step
                    this.steps[0].loadData(() => {
                        this.step(1);
                    });
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

        private updateUI(): void {
            this.content.find('.wizard-title').text(this.title);
            this.content.find('.step-number').text(this._currentStep + 1);
            this.content.find('.step-total').text(this.steps.length);
        }

        private step(stepIncrease: number): void {
            this._currentStep += stepIncrease;
            this.updateUI();

            // If this is the last step, close the dialog.
            if (this._currentStep >= this.steps.length) {
                this.close();
                return;
            }

            var step = this.steps[this._currentStep];
            var prevStep = null;
            var nextStep = null;

            console.log(this._currentStep);
            console.log(this.steps.length);

            if (this._currentStep + 1 < this.steps.length)
                nextStep = this.steps[this._currentStep + 1];

            if (this._currentStep - 1 >= 0)
                prevStep = this.steps[this._currentStep - 1];

            var body = this.content.find('.modal-body');
            body.empty().append(step.content);

            // Update next-button
            var nextButton = this.content.find('.btn-next-step');
            nextButton.off('click');

            if (nextStep === null) {
                nextButton.find('i').hide();
                nextButton.find('.next-step-title').hide();
                nextButton.find('.next-text').text('Finish');
            } else {
                nextButton.find('i').show();
                nextButton.find('.next-step-title').show();
                nextButton.find('.next-step-title').text(nextStep.name);
                nextButton.find('.next-text').text('Next:');
            }

            nextButton.on('click', (e) => {
                e.preventDefault();

                var overlay = new Hadouken.UI.Overlay('icon-refresh loading');
                overlay.show(this.content.find('.modal-body'));

                step.saveData(() => {
                    if (nextStep != null) {
                        nextStep.loadData(() => {
                            overlay.hide();
                            this.step(1);
                        });
                    } else {
                        overlay.hide();
                        this.step(1);
                    }
                });
            });

            // Update previous-button
            var prevButton = this.content.find('.btn-prev-step');
            prevButton.off('click');

            if (prevStep === null) {
                prevButton.hide();
            } else {
                prevButton.show();
                prevButton.find('.prev-step-title').text(prevStep.name);
            }

            prevButton.on('click', (e) => {
                e.preventDefault();

                var overlay = new Hadouken.UI.Overlay('icon-refresh loading');
                overlay.show(this.content.find('.modal-body'));

                step.saveData(() => {
                    if (prevStep != null) {
                        prevStep.loadData(() => {
                            overlay.hide();
                            this.step(-1);
                        });
                    } else {
                        overlay.hide();
                        this.step(-1);
                    }
                });
            });
        }
    }
}