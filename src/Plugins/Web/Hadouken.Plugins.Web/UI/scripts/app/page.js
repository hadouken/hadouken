define(['jquery'], function($) {
    function Page(url, route) {
        this.url = url;
        this.route = route;
        this.content = '';
    }

    Page.prototype.init = function () {
        var that = this;
        
        $.get(this.url, function(html) {
            that.content = $(html);
            $('#page-container').empty().append(that.content);

            that.load();
        });
    };

    Page.prototype.load = function() {
    };

    Page.prototype.unload = function() {
    };

    return Page;
});