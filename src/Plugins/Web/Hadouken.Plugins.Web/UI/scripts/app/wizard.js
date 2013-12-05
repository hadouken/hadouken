define(['jquery', 'bootstrap', 'overlay'], function($, $bs, Overlay) {
    function Wizard(title) {
        this.title = title;
        this.currentStep = -1;
        this.steps = [];
        this.content = null;
    }

    Wizard.prototype.show = function () {
        var that = this;
        
        $.get('/wizard.html', function(html) {
            that.content = $(html);
            that.content.on('hidden.bs.modal', function() {
                that.content.remove();
            });

            var firstStep = that.steps[0];

            firstStep.load(function() {
                that.content.modal();
                that.step(1);
            });
        });
    };

    Wizard.prototype.close = function() {
        this.content.modal('hide');
    };

    Wizard.prototype.update = function() {
        this.content.find('.wizard-title').text(this.title);
        this.content.find('.step-number').text(this.currentStep + 1);
        this.content.find('.step-total').text(this.steps.length);
    };

    Wizard.prototype.step = function(inc) {
        this.currentStep += inc;
        this.update();
        
        if (this.currentStep >= this.steps.length) {
            this.close();
            return;
        }

        var step = this.steps[this.currentStep];
        var prevStep = null;
        var nextStep = null;
        
        if (this.currentStep + 1 < this.steps.length) {
            nextStep = this.steps[this.currentStep + 1];
        }
        
        if (this.currentStep - 1 >= 0) {
            prevStep = this.steps[this.currentStep - 1];
        }

        var body = this.content.find('.modal-body');
        body.empty().append(step.content);
        
        var that = this;
        
        // Finish
        var finishButton = this.content.find('.btn-finish');
        finishButton.off('click');
        
        // Update next button
        var nextButton = this.content.find('.btn-next-step');
        nextButton.off('click');
        
        if (nextStep === null) {
            nextButton.hide();
            finishButton.show();
            finishButton.on('click', function(e) {
                e.preventDefault();
                that.save();
            });
        } else {
            nextButton.show();
            finishButton.hide();
            nextButton.find('.next-step-title').text(nextStep.name);
        }

        nextButton.on('click', function(e) {
            e.preventDefault();

            nextStep.load(function() {
                that.step(1);
            });
        });
        
        // Update previous button
        var prevButton = this.content.find('.btn-prev-step');
        prevButton.off('click');
        
        if (prevStep === null) {
            prevButton.hide();
        } else {
            prevButton.show();
            prevButton.find('.prev-step-title').text(prevStep.name);
        }

        prevButton.on('click', function(e) {
            e.preventDefault();

            prevStep.load(function() {
                that.step(-1);
            });
        });
    };

    Wizard.prototype.save = function() {
        var overlay = new Overlay(this.content.find('.modal-body'));
        overlay.show();

        var savedSteps = 0;
        var that = this;
        
        var saveCallback = function() {
            savedSteps += 1;
            
            if (savedSteps === that.steps.length) {
                overlay.hide();
                that.close();
            }
        };
        
        for (var i = 0; i < this.steps.length; i++) {
            var step = this.steps[i];
            step.save(saveCallback);
        }
    };

    return Wizard;
});