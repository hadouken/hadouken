define(['wizardStep'], function(WizardStep) {
    function ConfigureStep() {
        WizardStep.call(this, 'Core', '/wizards/configure-core.html');
    }

    ConfigureStep.prototype = new WizardStep();
    ConfigureStep.prototype.constructor = ConfigureStep;

    return ConfigureStep;
});