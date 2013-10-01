var Page = new Class({
    url: '',
    
    initialize: function(url) {
        this.url = url;
    },
    
    preload: function(callback) {
        new Request.HTML({            
            url: this.url,
            method: 'get',
            update: $('page-container'),
            onSuccess: function() {
                callback();
            }.bind(this)
        }).send();
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
            this.currentPage.load();
        }.bind(this));
    }
};