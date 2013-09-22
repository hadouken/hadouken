var Page = new Class({
    url: '',
    html: '',
    
    initialize: function(url) {
        this.url = url;
    },
    
    preload: function(callback) {
        new Request({            
            url: this.url,
            method: 'get',
            onSuccess: function(html) {
                this.html = html;
                callback();
            }.bind(this)
        }).send();
    },
    
    render: function(target) {
        $(target).empty();
        $(target).set('html', this.html);
    },
    
    load: Function.from(),
    
    unload: Function.from()
});

var Pages = {
    currentPage: null,
    
    navigateTo: function(page) {
        if (this.currentPage != null) {
            this.currentPage.unload();
            this.currentPage = null;
        }

        this.currentPage = page;

        this.currentPage.preload(function() {
            this.currentPage.render('page-container');
            this.currentPage.load();
        }.bind(this));
    }
};