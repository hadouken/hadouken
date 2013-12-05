define(['jquery'], function($) {
    function WizardStep(name, url) {
        this.name = name;
        this.url = url;
        this.content = null;
    }

    WizardStep.prototype.load = function(callback) {
        if (this.content !== null) {
            callback();
        } else {
            var that = this;
            
            $.get(this.url, function(html) {
                that.content = $(html);
                callback();
            });
        }
    };

    WizardStep.prototype.save = function (callback) {
        callback();
    };

    return WizardStep;
});