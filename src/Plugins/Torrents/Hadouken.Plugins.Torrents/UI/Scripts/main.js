console.log("Loading Torrents plugin");

var AddTorrentDialog = new Class({
    Extends: Dialog,

    initialize: function() {
        this.parent('/plugins/core.torrents/dialogs/add.html');
    },

    load: function() {
        this.element.getElement('#btn-add-torrents').addEvent('click', function (e) {
            e.preventDefault();
            
            var reader = new FileReader();
            reader.onload = function(ev) {
                // Data
                var d = [
                    ev.target.result.split(',')[1],
                    '',
                    ''
                ];

                // Add file
                var rpcClient = new JsonRpcClient();
                rpcClient.call("torrents.addFile", d, function(result) {
                    console.log("Added!");
                });
            };

            reader.readAsDataURL(this.element.getElement("#torrent-files").files[0]);
        }.bind(this));
    }
});

var TorrentsListPage = new Class({
    Extends: Page,
    
    request: null,
    torrents: {},
    templates: {
        torrentsListItem: Handlebars.compile('<tr data-id="{{torrent.id}}">' +
                              '<td>{{torrent.name}}</td>' +
                              '<td>Stopped</td>' +
                              '<td>{{fileSize torrent.size}}</td>' +
                              '<td>' +
                                  '<div class="pull-right"><button type="button" class="btn btn-primary btn-xs"><i class="glyphicon glyphicon-info-sign"></i></button></div>' +
                              '</td>' +
                          '</tr>')
    },
    
    initialize: function() {
        this.parent("/plugins/core.torrents/list.html");
    },

    load: function() {
        console.log("TorrentsListPage::load()");

        Handlebars.registerHelper('fileSize', function(size) {
            var i = Math.floor(Math.log(size) / Math.log(1024));
            return (size / Math.pow(1024, i)).toFixed(2) * 1 + '' + ['B', 'kB', 'MB', 'GB', 'TB'][i];
        });

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

        if (response.result == null || response.result === undefined)
            return;

        for (var i = 0; i < response.result.length; i++) {
            var torrent = response.result[i];
            var element = $$('tr[data-id=' + torrent.id + ']');
            
            if (element == null || element.length == 0) {

                // do stuff
                var itemHtml = this.templates.torrentsListItem({ torrent: torrent });
                var els = Elements.from(itemHtml);

                $("tbody-torrents-list").adopt(els);

            } else {
                console.log("row exists");
            }

            this.torrents[torrent.id] = torrent;
        }
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