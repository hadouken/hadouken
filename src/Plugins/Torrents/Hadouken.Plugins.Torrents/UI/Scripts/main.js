console.log("Loading Torrents plugin");

var TorrentsListPage = new Class({
    Extends: Page,
    
    request: null,
    
    initialize: function() {
        this.parent("/plugins/core.torrents/list.html");
    },

    load: function() {
        console.log("TorrentsListPage::load()");

        this.request = new Request.JSON({            
            method: 'post',
            url: '/jsonrpc',
            initialDelay: 100,
            delay: 2000,
            limit: 15000,
            onSuccess: function (data) {
                this.handleResponse(data.result);
            }.bind(this)
        });

        this.request.startTimer(JSON.encode({
            id: 'torrents-id',
            jsonrpc: '2.0',
            method: 'torrents.list',
            params: null
        }));
    },
    
    unload: function() {
        console.log("TorrentsListPage::unload()");

        this.request.stopTimer();
    },
    
    handleResponse: function(torrentsList) {
        console.log(torrentsList);
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