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
                this.content.on('hidden.bs.modal', function () {
                    $(this).remove();
                });

                // Load data for first step
                var firstStep = this.steps[0];

                firstStep.load(() => {
                    firstStep.loadData(() => {
                        this.content.modal();
                        this.step(1);
                    });
                });
            });
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

            // Get the current, previous and next step
            var step = this.steps[this._currentStep];
            var prevStep = null;
            var nextStep = null;

            if (this._currentStep + 1 < this.steps.length)
                nextStep = this.steps[this._currentStep + 1];

            if (this._currentStep - 1 >= 0)
                prevStep = this.steps[this._currentStep - 1];

            // Find the modal body and append the step HTML
            var body = this.content.find('.modal-body');
            body.empty().append(step.content);

            // Finish button
            var finishButton = this.content.find('.btn-finish');
            finishButton.off('click');

            // Update next-button
            var nextButton = this.content.find('.btn-next-step');
            nextButton.off('click');

            if (nextStep === null) {
                nextButton.hide();
                finishButton.show();
                finishButton.on('click', () => this.save());
            } else {
                nextButton.show();
                finishButton.hide();
                nextButton.find('.next-step-title').text(nextStep.name);
            }

            nextButton.on('click', (e) => {
                e.preventDefault();

                nextStep.load(() => {
                    if (nextStep.isDataLoaded) {
                        this.step(1);
                    } else {
                        nextButton.attr('disabled', true);

                        nextStep.loadData(() => {
                            nextStep.isDataLoaded = true;
                            nextButton.attr('disabled', false);
                            this.step(1);
                        });
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

                prevStep.load(() => {
                    this.step(-1);
                });
            });
        }

        private save(): void {
            var overlay = new Hadouken.UI.Overlay('icon-refresh loading');
            overlay.show(this.content.find('.modal-body'));

            var savedSteps = 0;

            var saveCallback = () => {
                savedSteps += 1;

                if (savedSteps == this.steps.length) {
                    overlay.hide();
                    this.close();
                }
            };

            for (var i = 0; i < this.steps.length; i++) {
                var step = this.steps[i];
                step.saveData(saveCallback);
            }
        }
    }
}