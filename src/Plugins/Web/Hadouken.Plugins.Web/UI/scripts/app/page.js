define(['jquery'], function($) {
    function Page(url, route) {
        this.url = url;
        this.route = route;
        this.content = '';
    }

    Page.prototype.init = function (args) {
        var that = this;
        
        $.get(this.url, function(html) {
            that.content = $('<div>' + html + '</div>');
            $('#page-container').empty().append(that.content);

            that.load.apply(that, args);
        });
    };

    Page.prototype.load = function() {
    };

    Page.prototype.unload = function() {
    };

    return Page;
});