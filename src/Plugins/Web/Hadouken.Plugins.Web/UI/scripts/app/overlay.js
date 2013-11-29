define([ 'jquery' ], function ( $ ) {
    function Overlay(target) {
        this.target = target;
    }

    Overlay.prototype.show = function() {
        var html = $('<div class="overlay"><div class="message"><i class="icon-refresh loading"></i></div></div>');
        $(this.target).prepend(html);
    };

    Overlay.prototype.hide = function() {
        $(this.target).find('.overlay').remove();
    };

    return Overlay;
});