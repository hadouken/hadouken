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
    labels:
    {
        "-_-_-all-_-_-": 0,
        "-_-_-dls-_-_-": 0,
        "-_-_-com-_-_-": 0,
        "-_-_-act-_-_-": 0,
        "-_-_-iac-_-_-": 0,
        "-_-_-nlb-_-_-": 0,
        "-_-_-err-_-_-": 0
    },
    cLabels: {},
    actLbl: "-_-_-all-_-_-",
    tegs: {},
    
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
            var width = WebUI.settings["webui." + ndx +".colwidth"];
            
            // array deciding if columns are enabled or disabled
            var enabled = WebUI.settings["webui." + ndx + ".colenabled"];
            
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
            switch(value)
            {
                case "true":
                case "auto":
                case "on":
                    data[key] = 1;
                    break;
                    
                case "false":
                    data[key] = 0;
                    break;
            }
            
            data[key] = JSON.parse(data[key]);
        });
        
        $.extend(this.settings, data);
        
        this.loadSettings();
    },
    
    loadSettings: function()
    {
        
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
        var table = this.getTable("torrents");
        var hash = table.getFirstSelected();
        
        if((table.selCount == 1) && hash)
        {
            // show etails
        }
        else
        {
            //WebUI.dID = "";
            //WebUI.clearDetails();
        }
        
        if(e.which == 3)
        {
            WebUI.createMenu(e, id);
            ContextMenu.show(e.clientX, e.clientY);
        }
    },
    
    createMenu: function(e, id)
    {
        ContextMenu.clear();
        
        var table = this.getTable("torrents");
        
        if(table.selCount > 1)
        {
            ContextMenu.add([ "Start", "WebUI.start()" ]);
            ContextMenu.add([ "Pause", "WebUI.pause()" ]);
            ContextMenu.add([ "Stop", "WebUI.stop()" ]);
        }
        else
        {
            ContextMenu.add(["Start", this.isTorrentCommandEnabled("start",id) ? "WebUI.start()" : null]);
            ContextMenu.add(["Pause", this.isTorrentCommandEnabled("pause",id) ? "WebUI.pause()" : null]);
            ContextMenu.add(["Stop", this.isTorrentCommandEnabled("stop",id) ? "WebUI.stop()" : null]);
        }
        
        ContextMenu.add([CMENU_SEP]);
        
        var _labels = [];
        
        for(var lbl in this.cLabels)
        {
            if((table.selCount == 1) && (this.torrents[id].Label == lbl))
            {
                _labels.push([CMENU_SEL, lbl + " "]);
            }
            else
            {
                _labels.push([ lbl + " ", (table.selCount > 1) || this.isTorrentCommandEnabled("setlabel", id) ? "WebUI.setLabel('" + addslashes(lbl) + "')" : null ]);
            }
        }
        
        if(_labels.length > 0)
        {
            _labels.push([CMENU_SEP]);
        }
        
        _labels.push(["New label", (table.selCount > 1) || this.isTorrentCommandEnabled("setlabel", id) ? "WebUI.newLabel()" : null ]);
        _labels.push(["Remove label", (table.selCount > 1) || this.isTorrentCommandEnabled("remlabel", id) ? "WebUI.removeLabel()" : null ]);
        
        ContextMenu.add([CMENU_CHILD, "Labels", _labels]);
        
    },
    
    isTorrentCommandEnabled: function(action, hash)
    {
        var ret = true;
        var state = this.torrents[hash].State;
        
        switch(action)
        {
            case "start":
                ret = (state == "Stopped" || state == "Paused");
                break;
            
            case "stop":
                ret = (state == "Downloading" || state == "Paused" || state == "Seeding");
                break;
                
            case "pause":
                ret = (state == "Downloading" || state == "Seeding");
                break;
                
            case "remlabel":
                ret = (!this.torrents[hash].Label == null || !this.torrents[hash].Label == "");
                break;
        }
        
        return ret;
    },
    
    // return selected torrent hashes which can perform the action
    getHashes: function(action)
    {
        var h = "";
        var res = [];
        
        var table = this.getTable("torrents");
        var selectedRows = table.rowSel;
        
        for(var k in selectedRows)
        {
            if((selectedRows[k] == true) && this.isTorrentCommandEnabled(action, k))
            {
                res.push(k);
            }
        }
        
        return res;
    },
    
    stop: function()
    {
        this.performAction("stop", { action: "stop" });
    },
    
    start: function()
    {
        this.performAction("start", { action: "start" });
    },
    
    pause: function()
    {
        this.performAction("pause", { action: "pause" });
    },
    
    performAction: function(action, dataObj)
    {
        var hashes = this.getHashes(action);
        
        if(hashes.length > 0)
        {
            var data = {};
            
            for(var h in hashes)
            {
                data[hashes[h]] = dataObj;
            }
            
            Network.putJson("/api/torrents", data, function(resp)
            {
                WebUI.update();
            });
        }
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
                var oldTorrent = WebUI.torrents[hash];
                
                if(label != WebUI.labels[hash])
                {
                    WebUI.labels[hash] = label;
                    table.setAttr(hash, { label: label });
                    WebUI.filterByLabel(hash);
                }
                
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
        
        this.clearTegs();
        
        $.each(this.torrents, function(hash, torrent)
        {
            if(!torrent._updated)
            {
                delete WebUI.torrents[hash];
                
                // decrement labels
                if(WebUI.labels[hash].indexOf("-_-_-nlb-_-_-") >= -1)
                    WebUI.labels["-_-_-nlb-_-_-"]--;
                
                if(WebUI.labels[hash].indexOf("-_-_-com-_-_-") >= -1)
                    WebUI.labels["-_-_-com-_-_-"]--;
                    
                if(WebUI.labels[hash].indexOf("-_-_-dls-_-_-") >= -1)
                    WebUI.labels["-_-_-dls-_-_-"]--;
                    
                if(WebUI.labels[hash].indexOf("-_-_-act-_-_-") >= -1)
                    WebUI.labels["-_-_-act-_-_-"]--;
                    
                if(WebUI.labels[hash].indexOf("-_-_-iac-_-_-") >= -1)
                    WebUI.labels["-_-_-err-_-_-"]--;
                    
                if(WebUI.labels[hash].indexOf("-_-_-err-_-_-") >= -1)
                    WebUI.labels["-_-_-err-_-_-"]--;
                
                WebUI.labels["-_-_-all-_-_-"]--;
                
                delete WebUI.labels[hash];
                
                table.removeRow(hash);
                wasRemoved = true;
            }
            else
            {
                torrent._updated = false;
                WebUI.updateTegs(torrent);
            }
        });
        
        this.loadLabels(data.labels);
        this.updateLabels(wasRemoved);
        
        this.loadTorrents();
    },
    
    setTeg: function(str)
    {
        str = $.trim(str);
        
        if(str != "")
        {
            for(var id in this.tegs)
            {
                
            }
        }
    },
    
    clearTegs: function()
    {
        for(var id in this.tegs)
        {
            this.tegs[id].cnt = 0;
        }
    },
    
    updateTeg: function(id)
    {
        var teg = this.tegs[id];
        var str = teg.val.toLowerCase();
        
        $.each(this.torrents, function(hash, torrent)
        {
            if(torrent.Name.ToLowerCase().indexOf(str) >= -1)
            {
                teg.cnt++;
            }
        });
        
        var counter = $("#" + id + "-c");
        
        if(counter.text() != teg.cnt)
        {
            counter.text(teg.cnt);
            $("#" + id).attr("title", teg.val + " (" + teg.cnt + ")");
        }
    },
    
    updateTegs: function(torrent)
    {
        var str = torrent.Name.toLowerCase();
        
        for(var id in this.tegs)
        {
            var teg = this.tegs[id];
            
            if(str.indexOf(teg.val.toLowerCase()) >= -1)
            {
                teg.cnt++;
            }
        }
    },
    
    removeTeg: function(id)
    {
        delete this.tegs[id];
        
        $($$(id)).remove();
        
        this.actLbl = "";
        this.switchLabel($$("-_-_-all-_-_-"));
    },
    
    getStatusIcon: function(torrent)
    {
        var state = torrent.State;
        var progress = iv(torrent.Progress);
        var complete = torrent.Complete;
        
        var icon = "Status_" + state, status = state;
        
        if(state == "Stopped" && complete)
        {
            icon = "Status_Completed";
            status = "Completed";
        }
        
        return [ icon, status ];
    },
    
    labelContextMenu: function(e)
    {
        // if right click
        if(e.which == 3)
        {
            var table = WebUI.getTable("torrents");
            table.clearSelection();
            
            WebUI.switchLabel(this);
            
            table.fillSelection();
            
            var id = table.getFirstSelected();
            
            if(id)
            {
                WebUI.createMenu(null, id);
                ContextMenu.show(e.x, e.y);
            }
            else
            {
                ContextMenu.hide();
            }
        }
        else
        {
            WebUI.switchLabel(this);
        }
        
        return false;
    },
    
    loadLabels: function(d)
    {
        var p = $("#lbll");
        var temp = [];
        var keys = [];
        
        for(var lbl in d)
        {
            keys.push(lbl);
        }
        keys.sort();
        
        for(var i = 0; i < keys.length; i++)
        {
            var lbl = keys[i];
            
            this.labels["-_-_-" + lbl + "-_-_-"] = d[lbl];
            this.cLabels[lbl] = 1;
            
            temp["-_-_-" + lbl + "-_-_-"] = true;
            
            if(!$$("-_-_-" + lbl + "-_-_-"))
            {
                p.append($("<li>").
                    attr("id", "-_-_-" + lbl + "-_-_-").
                    addClass("lbl_" + lbl).
                    html(escapeHTML(lbl) + "&nbsp;(<span id=\"-_-_-" + lbl + "-_-_-c\">" + d[lbl] + "</span>)").
                    mouseclick(WebUI.labelContextMenu).addClass("cat"));
            }
        }
        
        var actDeleted = false;
        
        p.children().each(function(ndx, val)
        {
            var id = val.id;
            
            if(id && !$type(temp[id]))
            {
                $(val).remove();
                
                delete WebUI.labels[id];
                delete WebUI.cLabels[id.substr(5, id.length - 10)];
                
                if(WebUI.actLabel == id)
                {
                    actDeleted = true;
                }
            }
        });
        
        if(actDeleted)
        {
            this.actLabel = "";
            this.switchLabel($$("-_-_-all-_-_-"));
        }
    },
    
    getLabels: function(id, torrent)
    {
        if(!$type(this.labels[id]))
            this.labels[id] = "";
        
        var lbl = torrent.Label;
        
        if(lbl == "" || lbl == null )
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
        
        if((torrent.DownloadSpeed >= 1024) || (torrent.UploadSpeed >= 1024))
        {
            lbl += "-_-_-act-_-_-";
            
            if(this.labels[id].indexOf("-_-_-act-_-_-") == -1)
            {
                this.labels["-_-_-act-_-_-"]++;
            }
            
            if(this.labels[id].indexOf("-_-_-iac-_-_-") > -1)
            {
                this.labels["-_-_-iac-_-_-"]--;
            }
        }
        else
        {
            lbl += "-_-_-iac-_-_-";
            
            if(this.labels[id].indexOf("-_-_-iac-_-_-") == -1)
            {
                this.labels["-_-_-iac-_-_-"]++;
            }
            
            if(this.labels[id].indexOf("-_-_-act-_-_-") > -1)
            {
                this.labels["-_-_-act-_-_-"]--;
            }
        }
        
        if(torrent.State == "Error")
        {
            lbl += "-_-_-err-_-_-";
            
            if(this.labels[id].indexOf("-_-_-err-_-_-") == -1)
            {
                this.labels["-_-_-err-_-_-"]++;
            }
        }
        else
        {
            if(this.labels[id].indexOf("-_-_-err-_-_-") > -1)
            {
                this.labels["-_-_-err-_-_-"]--;
            }
        }
        
        lbl += "-_-_-all-_-_-";
        
        if(this.labels[id] == "")
        {
            this.labels["-_-_-all-_-_-"]++;
        }
        
        return lbl;
    },
    
    removeLabel: function()
    {
        this.performAction("setlabel", { label: "" });
    },
    
    setLabel: function(lbl)
    {
        this.performAction("setlabel", { label: lbl });
    },
    
    newLabel: function()
    {
        var table = this.getTable("torrents");
        var s = "New label";
        
        if(table.selCount == 1)
        {
            var k = table.getFirstSelected();
            var lbl = this.torrents[k].Label;
            
            if(lbl != "")
            {
                s = this.torrents[k].Label;
            }
        }
        
        $("#txtLabel").val(s);
        
        Dialogs.show("dlgLabel");
    },
    
    createLabel: function()
    {
        var lbl = $.trim($("#txtLabel").val());
        lbl = lbl.replace(/\"/g, "'");
        
        if(lbl != "")
        {
            this.performAction("setlabel", { label: lbl });
        }
    },
    
    updateLabels: function(wasRemoved)
    {
        for(var k in this.labels)
        {
            if(k.substr(0,5) == "-_-_-")
            {
                $($$(k + "c")).text(this.labels[k]);
            }
        }
        
        for(var id in this.tegs)
        {
            var counter = $("#" + id + "-c");
            var teg = this.tegs[id];
            
            if(counter.text() != teg.cnt)
            {
                counter.text(teg.cnt);
                $("#" + id).attr("title", teg.val + " (" + teg.cnt + ")");
            }
        }
    },
    
    switchLabel: function(obj)
    {
        if(obj.id != this.actLbl)
        {
            if((this.actLbl != "") && $$(this.actLbl))
            {
                $($$(this.actLbl)).removeClass("sel");
            }
            
            $(obj).addClass("sel");
            
            this.actLbl = obj.id;
            
            var table = this.getTable("torrents");
            table.scrollTo(0);
            
            for(var k in this.torrents)
            {
                this.filterByLabel(k);
            }
            
            table.clearSelection();
            
            // dID details something
            
            table.refreshRows();
        }
    },
    
    filterByLabel: function(id)
    {
        var table = this.getTable("torrents");
        
        if($($$(this.actLbl)).hasClass("teg"))
        {
            var teg = this.tegs[this.actLbl];
            
            if(teg)
            {
                if(table.getValueById(id, "Name").toLowerCase().indexOf(teg.val.toLowerCase()) > -1)
                {
                    table.unhideRow(id);
                }
                else
                {
                    table.hideRow(id);
                }
            }
        }
        else
        {
            if(table.getAttr(id, "label").indexOf(this.actLbl) > -1)
            {
                table.unhideRow(id);
            }
            else
            {
                table.hideRow(id);
            }
        }
    },
    
    resize: function()
    {
        var ww = $(window).width();
        var wh = $(window).height();
        var w = Math.floor(ww * (1 - WebUI.settings["webui.hsplit"])) - 5;
        var th = ($("#toolbar").is(":visible") ? $("#toolbar").height() : -1)+$("#statusbar").height()+12;

        $("#statusbar").width(ww);
                
        WebUI.resizeLeft(w, wh - th);
        w = ww - w;
        
        w -= 11;
        
        WebUI.resizeTop(w, Math.floor(wh * (WebUI.settings["webui.show_dets"] ? WebUI.settings["webui.vsplit"] : 1)) - th - 7);
        
        if(WebUI.settings["webui.show_dets"])
        {
            WebUI.resizeBottom(w, Math.floor(wh * (1 - WebUI.settings["webui.vsplit"])));
        }
        
        $("#HDivider").height(wh - th + 2);
    },
    
    resizeLeft: function(w, h)
    {
        if(w !== null)
        {
            $("#categories").width(w);
            $("#VDivider").width = $(window).width() - w - 10;
        }
        
        if(h !== null)
        {
            $("#categories").height(h);
        }
    },
    
    resizeTop: function(w, h)
    {
        this.getTable("torrents").resize(w, h); 
    },
    
    resizeBottom: function(w, h)
    {
        if(w !== null)
        {
            $("#details").width(w);
            w -= 8;
        }
        
        if(h !== null)
        {
            $("#details").height(h);
            h -= ($("#tabbar").height());
            
            $("#tdcont").height(h);
            h -= 2;
        }
        
        if(WebUI.configured)
        {
            // resize other tables and speedgraph
        }
    },
    
    setHSplitter: function()
    {
        var r = 1 - ($("#categories").width() + 5) / $(window).width();
        r = Math.floor(r * Math.pow(10, 3)) / Math.pow(10, 3);
        
        if((WebUI.settings["webui.hsplit"] != r) && (r > 0 && r < 1))
        {
            WebUI.settings["webui.hsplit"] = r;
            WebUI.save();
        }
    },
    
    setVSplitter: function()
    {
        var r = 1 - ($("#details").height() / $(window).height());
        r = Math.floor(r * Math.pow(10, 3)) / Math.pow(10, 3);

        if((WebUI.settings["webui.vsplit"] != r) && (r > 0) && (r < 1)) 
        {
            WebUI.settings["webui.vsplit"] = r;
            WebUI.save();
        }
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
        if(!WebUI.configured)
        {
            return;
        }
        
        $.each(WebUI.tables, function(ndx, table)
        {
            var width = [];
            var enabled = [];
            
            for(i = 0; i < table.obj.cols; i++)
            {
                width.push(table.obj.getColWidth(i));
                enabled.push(table.obj.isColumnEnabled(i));
            }
            
            console.log(width);
            
            WebUI.settings["webui." + ndx + ".colwidth"] = width;
            WebUI.settings["webui." + ndx + ".colenabled"] = enabled;
            WebUI.settings["webui." + ndx +".colorder"] = table.obj.colOrder;
            WebUI.settings["webui." + ndx +".sindex"] = table.obj.sIndex;
            WebUI.settings["webui." + ndx +".rev"] = table.obj.reverse;
            WebUI.settings["webui." + ndx +".sindex2"] = table.obj.secIndex;
            WebUI.settings["webui." + ndx +".rev2"] = table.obj.secRev;
        });
        
        var cfg = {};
        
        $.each(WebUI.settings, function(key, value)
        {
            if((/^webui\./).test(key))
            {
                cfg[key] = JSON.stringify(value);
            }
        });
        
        Network.postJson("/api/config", cfg, null, true);
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