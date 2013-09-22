console.log("Loading Torrents plugin");

var AddTorrentDialog = new Class({
    Extends: Dialog,

    initialize: function() {
        this.parent('/plugins/core.torrents/dialogs/add.html');
    },

    load: function() {
        this.element.getElement('#btn-submit').addEvent('click', function(e) {
            alert("add torrents woho!");
        });
    }
});

var TorrentsListPage = new Class({
    Extends: Page,
    
    request: null,
    
    initialize: function() {
        this.parent("/plugins/core.torrents/list.html");
    },

    load: function() {
        console.log("TorrentsListPage::load()");

        this.setupRequestTimer();
        this.setupButtons();
    },
    
    unload: function() {
        console.log("TorrentsListPage::unload()");

        this.request.stopTimer();
    },
    
    setupRequestTimer: function() {
        this.request = new Request.JSON({            
            method: 'post',
            url: '/jsonrpc',
            initialDelay: 100,
            delay: 2000,
            limit: 15000,
            onSuccess: function (data) {
                this.handleResponse(data);
            }.bind(this)
        });

        this.request.startTimer(JSON.encode({
            id: 'torrents-id',
            jsonrpc: '2.0',
            method: 'torrents.list',
            params: null
        }));
    },
    
    setupButtons: function() {
        $("btn-add-torrents").addEvent('click', function(e) {
            e.preventDefault();

            Dialogs.show(new AddTorrentDialog());
        });
    },
    
    handleResponse: function(response) {
        // If response is error response, show error
        // else, handle torrents in response.result
    }
});

try {

    MainMenu.addItem(
        new MenuItem("Torrents", {
            onClick: function() {
                Pages.navigateTo(new TorrentsListPage());
            }
        })
    );

} catch(e) {
    console.log(e);
}