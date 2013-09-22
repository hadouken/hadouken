var Dialog = new Class({
    url: '',
    element: null,
    
    initialize: function(url) {
        this.url = url;
    },
    
    preload: function(callback) {
        new Request({
            url: this.url,
            method: 'get',
            onSuccess: function (html) {
                this.element = new Element('div', {
                    'class': 'modal-container',
                    html: html
                });
                
                callback();
            }.bind(this)
        }).send();
    },
    
    render: function () {
        $(document.body).adopt($(this.element));
    },
    
    show: function() {
        $("overlay").show();
        this.element.show();
    },
    
    close: function() {
        this.element.hide();
        $("overlay").hide();
    },
    
    load: Function.from(),
    
    unload: Function.from()
});

var Dialogs = {
    currentDialog: null,
    
    show: function(dialog) {
        if (this.currentDialog != null) {
            this.currentDialog.hide();
            this.currentDialog.element.destroy();
            this.currentDialog = null;
        }

        this.currentDialog = dialog;

        this.currentDialog.preload(function() {
            this.currentDialog.render();
            this.currentDialog.load();
            this.currentDialog.show();
        }.bind(this));
    }
}