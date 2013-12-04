define(['jquery', 'bootstrap'], function($) {
    function Dialog(url) {
        this.url = url;
    }

    Dialog.prototype.show = function() {
        var that = this;
        
        $.get(this.url, function (html) {
            that.content = $(html);

            $('body').append(that.content);

            that.load(function () {
                that.content.modal();
                that.content.on('hidden.bs.modal', function() {
                    that.content.remove();
                });
            });
        });
    };

    Dialog.prototype.load = function (callback) {
        callback();
    };

    Dialog.prototype.close = function() {
        this.content.modal('hide');
    };

    return Dialog;
});