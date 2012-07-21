var WebUI = 
{
    tables:
    {
        torrents:
        {
            obj: new dxSTable(),
            columns:
            [
                { text: "Name",                 width: "200px",         id: "Name",              type: TYPE_STRING },
                { text: "Status",               width: "100px",         id: "State",             type: TYPE_STRING },
                { text: "Size",                 width: "60px",          id: "Size",              type: TYPE_NUMBER },
                { text: "Progress",             width: "100px",         id: "Progress",          type: TYPE_PROGRESS },
                { text: "Downloaded",           width: "100px",         id: "DownloadedBytes",   type: TYPE_NUMBER },
                { text: "Uploaded",             width: "100px",         id: "UploadedBytes",     type: TYPE_NUMBER },
                { text: "Ratio",                width: "60px",          id: "Ratio",             type: TYPE_NUMBER },
                { text: "DL Speed",             width: "60px",          id: "DownloadSpeed",     type: TYPE_NUMBER },
                { text: "UL Speed",             width: "60px",          id: "UploadSpeed",       type: TYPE_NUMBER },
                { text: "ETA",                  width: "60px",          id: "ETA",               type: TYPE_NUMBER },
                { text: "Label",                width: "60px",          id: "Label",             type: TYPE_STRING },
                { text: "Peers",                width: "60px",          id: "Peers_Info",        type: TYPE_NUMBER },
                { text: "Seeds",                width: "60px",          id: "Seeders_Info",        type: TYPE_NUMBER },
                { text: "Created on",           width: "100px",         id: "CreatedOn",         type: TYPE_NUMBER }
            ],
            container: "torrents",
            format: Formatter.torrents,
            ondelete: function() { WebUI.remove(); },
            onselect: function(e, id) { WebUI.selectTorrent(e, id); },
            ondblclick: function(obj) { WebUI.showTorrentDetails(obj.id); return false; }
        }
    },
    
    configured: false,
    firstLoad: true,
    settings:
    {
        "webui.fls.view":               0, 
        "webui.show_cats":              1, 
        "webui.show_dets":              1, 
        "webui.needmessage":            1, 
        "webui.reqtimeout":             30000,
        "webui.confirm_when_deleting":  1,
        "webui.alternate_color":        0,
        "webui.update_interval":        3000,
        "webui.hsplit":                 0.88,
        "webui.vsplit":                 0.5,
        "webui.effects":                0,
        "webui.fullrows":               0,
        "webui.no_delaying_draw":       1,
        "webui.search":                 -1,
        "webui.speedlistdl":            "100,150,200,250,300,350,400,450,500,750,1000,1250",
        "webui.speedlistul":            "100,150,200,250,300,350,400,450,500,750,1000,1250",
        "webui.ignore_timeouts":        0,
        "webui.retry_on_error":         120,
        "webui.closed_panels":          {},
        "webui.timeformat":             0,
        "webui.dateformat":             0,
        "webui.log_autoswitch":         1
    },
    updateTimer: null,
    interval: 1500,
    timer: new Timer(),
    
    torrents: {},
    labels: {},
    
    init: function()
    {
        // load plugins
        
        this.getUISettings();
        
        if(!this.configured)
        {
            this.config({});
        }
        
        this.assignEvents();
        this.resize();
        this.update();
    },
    
    setStatusUpdate: function()
    {
        document.title = "Hadouken";
        
        
    },
    
    assignEvents: function()
    {
        window.onresize = WebUI.resize;
        
        $(document).bind("dragstart",function(e) { return(false); } );
        $(document).bind("selectstart",function(e) { return(e.fromTextCtrl); });
    },
    
    config: function(data)
    {
        this.addSettings(data);
        
        $.each(this.tables, function(ndx, table)
        {
            // array with column widths
            var width = WebUI.settings["webui.tables." + ndx +".columnsWidth"];
            
            // array deciding if columns are enabled or disabled
            var enabled = WebUI.settings["webui.tables." + ndx + ".columnsEnabled"];
            
            $.each(table.columns, function(i, column)
            {
                if(width && i < width.length && width[i] > 4)
                {
                    column.width = width[i];
                }
                
                if(enabled && i < enabled.length)
                {
                    column.enabled = enabled[i];
                }
            });
            
            // callbacks
            table.obj.format = table.format;
            table.obj.onresize = WebUI.save;
            table.obj.onmove = WebUI.save;
            table.obj.ondblclick = table.ondblclick;
            table.obj.onselect = table.onselect;
            table.obj.ondelete = table.ondelete;
            
            // config
            table.obj.colorEvenRows = WebUI.settings["webui.alternate_color"];
            table.obj.maxRows = WebUI.settings["webui.fullrows"];
            table.obj.noDelayingDraw = WebUI.settings["webui.no_delaying_draw"];
            
            if($type(WebUI.settings["webui." + ndx + ".sindex"]))
                table.obj.sIndex = WebUI.settings["webui." + ndx + ".sindex"];
                
            if($type(WebUI.settings["webui." + ndx + ".rev"]))
                table.obj.reverse = WebUI.settings["webui." + ndx + ".rev"];
            
            if($type(WebUI.settings["webui." + ndx + ".sindex2"]))
                table.obj.secIndex = WebUI.settings["webui." + ndx + ".sindex2"];
                
            if($type(WebUI.settings["webui." + ndx + ".rev2"]))
                table.obj.secRev = WebUI.settings["webui." + ndx + ".rev2"];
                
            if($type(WebUI.settings["webui." + ndx + ".colorder"]))
                table.obj.colOrder = WebUI.settings["webui." + ndx + ".colorder"];
                
            table.obj.onsort = function()
            {
                if((this.sIndex   != WebUI.settings["webui." + this.prefix + ".sindex"]) ||
                   (this.reverse  != WebUI.settings["webui." + this.prefix + ".rev"]) ||
                   (this.secIndex != WebUI.settings["webui." + this.prefix + ".sindex2"]) ||
                   (this.secRev   != WebUI.settings["webui." + this.prefix + ".rev2"]))
                {
                    WebUI.save();
                }
            }
        });
        
        // do something with to each table perhaps...
        
        $.each(this.tables, function(ndx,table)
        {
            table.obj.create($$(table.container), table.columns, ndx);
        });

        
        this.configured = true;
    },
    
    addSettings: function(data)
    {
        $.each(data, function(key, value)
        {
            console.log(key + ": " + value);
        });
        
        $.extend(this.settings, data);
    },
    
    getUISettings: function()
    {
        Network.getJson("/api/config?g=webui.", [ this.config, this ], false);
    },
    
    remove: function()
    {
        console.log("remove torrent");
    },
    
    selectTorrent: function(e, id)
    {
        console.log("selectTorrent");
    },
    
    showTorrentDetails: function(id)
    {
        console.log("show torrent details");
    },
    
    getTorrents: function()
    {        
        if(this.updateTimer)
            window.clearTimeout(this.updateTimer);
        
        this.timer.start();
        
        if(this.updateTimer)
            window.clearTimeout(this.updateTimer);
            
        Network.getJson("/api/torrents", [ this.addTorrents, this ], true);
    },
    
    addTorrents: function(data)
    {
        var table = this.getTable("torrents");
        var totalUp = 0;
        var totalDown = 0;
        var tArray = [];
        
        $.each(data.torrents, function(hash, torrent)
        {
            totalUp += torrent.UploadedBytes;
            totalDown += torrent.DownloadedBytes;
            
            var statusInfo = WebUI.getStatusIcon(torrent);
            var label = WebUI.getLabels(hash, torrent);
            
            if(!$type(WebUI.torrents[hash]))
            {
                WebUI.labels[hash] = label;
                
                table.addRowById(torrent, hash, statusInfo[0], {label : label});
                
                tArray.push(hash);
                
                WebUI.filterByLabel(hash);
            }
            else 
            {
                // update existing torrent
                table.setIcon(hash, statusInfo[0]);
                
                for( var prop in torrent)
                {
                    table.setValueById(hash, prop, torrent[prop]);
                }
            }
            
            torrent._updated = true;
        });
        
        $.extend(this.torrents, data.torrents);
        
        // set speed values
        
        var wasRemoved = false;
        
        // clear tegs?
        
        $.each(this.torrents, function(hash, torrent)
        {
            if(!torrent._updated)
            {
                delete WebUI.torrents[hash];
                
                // decrement labels
                
                delete WebUI.labels[hash];
                
                table.removeRow(hash);
                wasRemoved = true;
            }
        });
        
        this.loadTorrents();
    },
    
    getStatusIcon: function(torrent)
    {
        var state = torrent.State;
        var progress = iv(torrent.Progress);
        
        var icon = "Status_" + state, status = state;
        
        
        
        return [ icon, status ];
    },
    
    getLabels: function(id, torrent)
    {
        if(!$type(this.labels[id]))
            this.labels[id] = "";
        
        var lbl = torrent.Label;
        
        if(lbl == "")
        {
            lbl += "-_-_-nlb-_-_-";
            
            if(this.labels[id].indexOf("-_-_-nlb-_-_-") == -1)
            {
                this.labels["-_-_-nlb-_-_-"]++;
            }
        }
        else
        {
            if(this.labels[id].indexOf("-_-_-nlb-_-_-") > -1)
            {
                this.labels["-_-_-nlb-_-_-"]--;
            }
        }
        
        lbl = "-_-_-" + lbl + "-_-_-";

        if(torrent.Progress < 100.0)
        {
            lbl += "-_-_-dls-_-_-";
            
            if(this.labels[id].indexOf("-_-_-dls-_-_-") == -1)
            {
                this.labels["-_-_-dls-_-_-"]++;
            }
            
            if(this.labels[id].indexOf("-_-_-com-_-_-") > -1)
            {
                this.labels["-_-_-com-_-_-"]--;
            }
        }
        else
        {
            lbl += "-_-_-com-_-_-";
            
            if(this.labels[id].indexOf("-_-_-com-_-_-") ==- 1)
            {
                this.labels["-_-_-com-_-_-"]++;
            }
            
            if(this.labels[id].indexOf("-_-_-dls-_-_-") >- 1)
            {
                this.labels["-_-_-dls-_-_-"]--;
            }
        }
        
        return lbl;
    },
    
    filterByLabel: function(id)
    {
    
    },
    
    resize: function()
    {
        var ww = $(window).width();
        var wh = $(window).height();
        var w = Math.floor(ww * (1 - WebUI.settings["webui.hsplit"])) - 5;
        var th = ($("#t").is(":visible") ? $("#t").height() : -1)+$("#StatusBar").height()+12;

        $("#statusbar").width(ww);
        
        WebUI.resizeTop( w, Math.floor(wh * (WebUI.settings["webui.show_dets"] ? WebUI.settings["webui.vsplit"] : 1))-th-7 );
    },
    
    resizeTop: function(w, h)
    {
        this.getTable("torrents").resize(undefined, h); 
    },
    
    update: function()
    {
        WebUI.getTorrents();
    },
    
    loadTorrents: function()
    {
        var table = this.getTable("torrents");
        table.refreshRows();        
        table.Sort();
        
        this.setInterval();
        
        // this.updateDetails();
    },
    
    getTable: function(prefix)
    {
        return($type(this.tables[prefix]) ? this.tables[prefix].obj : null);
    },

    
    show: function()
    {
        if($("#cover").is(":visible"))
        {
            $("#cover").hide();
            WebUI.resize();
            //theTabs.show("lcont");
        }
    },
    
    save: function()
    {
    
    },
    
    resetInterval: function()
    {
        this.timer.stop();
        
        if(this.updateTimer)
            window.clearTimeout(this.updateTimer);
            
        this.interval = iv(this.settings["webui.update_interval"]);
        this.updateTimer = window.setTimeout(this.update, this.interval);
    },
    
    setInterval: function(force)
    {
        this.timer.stop();
        
        if(this.updateTimer)
            window.clearTimeout(this.updateTimer);
            
        if(this.interval == -1)
        {
            this.interval = iv(this.settings["webui.update_interval"]) + this.timer.interval * 4;
        }
        else
        {
            this.interval = iv((this.interval + iv(this.settings["webui.update_interval"]) + this.timer.interval * 4) / 2);
        }
        
        this.updateTimer = window.setTimeout(this.update, this.interval);
    },
    
    showAddTorrent: function()
    {
        Dialogs.toggle("dlgAddTorrent");
    }
};