(function(jQuery)
{

var WebUI = 
{
    "torrents": {},
    "peerlist": {},
    "filelist": {},
    "settings": {},
    "props": {},
    
    "categories":
    {
        "cat_all": 0,
        "cat_dls": 0,
        "cat_com": 0,
        "cat_act": 0,
        "cat_iac": 0,
        "cat_nlb": 0,
    },
    
    "labels": {},
    "torGroups": {},
    "defTorGroup":
    {
        "cat": {},
        "lbl": {}
    },
    "limits":
    {
        "reqRetryDelayBase": 2, // seconds
        "reqRetryMaxAttempts": 5,
        "minTableRows": 5,
        "maxVirtTableRows": Math.ceil(screen.height / 16) || 100,
        "minUpdateInterval": 500,
        "minDirListCache": 30, // seconds
        "minFileListCache": 60, // seconds
        "minPeerListCache": 5, // seconds
        "minXferHistCache": 60, // seconds
        "defHSplit": 140,
        "defVSplit": 225,
        "minHSplit": 25,
        "minVSplit": 150,
        "minTrtH": 100,
        "minTrtW": 150
    },
    "defConfig":
    {
        	"showDetails": true,
            "showDetailsIcons": true,
            "showCategories": true,
            "showToolbar": true,
            "showStatusBar": true,
            "showSpeedGraph": true,
            "useSysFont": true,
            "updateInterval": 3000,
            "maxRows": 0,
            "lang": "en",
            "hSplit": -1,
            "vSplit": -1,
        "torrentTable":
        {
            "colMask": 0x0000, // automatically calculated based on this.trtColDefs
            "colOrder": [], // automatically calculated based on this.trtColDefs
            "colWidth": [], // automatically calculated based on this.trtColDefs
            "reverse": false,
            "sIndex": -1
        },
        "activeSettingsPane": "",
        "activeTorGroups":
        {
            "cat": {"cat_all": 1},
            "lbl": {}
        }
    },
    "torrentID": "",
    "propID": "",
    
    //"spdGraph": new SpeedGraph(),
    
    "trtTable": new STable(),
    "trtColDefs":
    [
        //[ colID, colWidth, colType, colDisabled = false, colIcon = false, colAlign = ALIGN_AUTO, colText = "" ]
          ["name", 220, TYPE_STRING]
        , ["order", 35, TYPE_NUM_ORDER]
        , ["size", 75, TYPE_NUMBER]
        , ["remaining", 90, TYPE_NUMBER, true]
        , ["done", 60, TYPE_NUM_PROGRESS]
        , ["status", 100, TYPE_CUSTOM]
        , ["seeds", 60, TYPE_NUMBER]
        , ["peers", 60, TYPE_NUMBER]
        , ["seeds_peers", 80, TYPE_NUMBER, true]
        , ["downspeed", 80, TYPE_NUMBER]
        , ["upspeed", 80, TYPE_NUMBER]
        , ["eta", 60, TYPE_NUM_ORDER]
        , ["uploaded", 75, TYPE_NUMBER, true]
        , ["downloaded", 75, TYPE_NUMBER, true]
        , ["ratio", 50, TYPE_NUMBER]
        , ["availability", 50, TYPE_NUMBER]
        , ["label", 80, TYPE_STRING, true]
        , ["added", 150, TYPE_NUMBER, true, false, ALIGN_LEFT]
        , ["completed", 150, TYPE_NUMBER, true, false, ALIGN_LEFT]
        , ["url", 250, TYPE_STRING, true]
    ],
    
    "trtColDoneIdx": -1, // automatically calculated based on this.trtColDefs
    "trtColStatusIdx": -1, // automatically calculated based on this.trtColDefs
    "flsColPrioIdx": -1, // automatically calculated based on this.flsColDefs
    "updateTimeout": null,
    "totalDL": 0,
    "totalUL": 0,
    
    "init": function()
    {
        this.config = Object.merge({}, this.defConfig);
        this.config.lang = "";
        
        this.trtColDoneIdx = this.trtColDefs.map(function(item) { return item[0] == "done"; }).indexOf(true);
        this.trtColStatusIdx = this.trtColDefs.map(function(item) { return item[0] == "status"; }).indexOf(true);
        
        // file prio
        //this.flsColPrioIdx = this.flsColDefs.map(function(item) { return item[0] == "prio"; }).indexOf(true);
        
        // default col mask
        this.trtColDefs.each(function(item, index) { this.trtColToggle(index, item[3], true); }, this);
        
        if(window.hdknweb) return;
        
        this.getSettings((function()
        {
            this.update.delay(0, this, (function()
            {
                this.refreshSelectedTorGroups();
                this.hideMsg();
            }).bind(this));
        }).bind(this));
    },
    
    "trtDataToRow": function(data)
    {
        return this.trtColDefs.map(function(item)
        {
            switch(item[0])
            {
                case "name":
                    return data[CONST.TORRENT_NAME];
                    
                case "order":
                    return 0;
                    
                case "size":
                    return data[CONST.TORRENT_SIZE];
                    
                case "remaining":
                    return -1;
                    
                case "done":
                    return data[CONST.TORRENT_PROGRESS];
                    
                case "status":
                    return [data[CONST.TORRENT_STATUS], ""];
                    
                case "seeds":
                	return data[CONST.TORRENT_SEEDS_CONNECTED] + " (" + data[CONST.TORRENT_SEEDS_SWARM] + ")";
                    
                case "peers":
                    return data[CONST.TORRENT_PEERS_CONNECTED] + " (" + data[CONST.TORRENT_PEERS_SWARM] + ")";
                    
                case "eta":
                    return data[CONST.TORRENT_ETA];
                    
                case "downloaded":
                    return data[CONST.TORRENT_DOWNLOADED];
                    
                case "downspeed":
                    return data[CONST.TORRENT_DOWNSPEED];
                    
                case "uploaded":
                    return data[CONST.TORRENT_UPLOADED];
                    
                case "upspeed":
                    return data[CONST.TORRENT_UPSPEED];
                    
                case "ratio":
                    var dl = data[CONST.TORRENT_DOWNLOADED];
                    var ul = data[CONST.TORRENT_UPLOADED];
                    
                    return (dl == 0 ? 0 : (ul / dl));
            }
        }, this);
    },
    
    "trtFormatRow": function(values, index)
    {
        var useidx = $chk(index);
        var len = (useidx ? (index + 1) : values.length);
        
        var doneIdx = this.trtColDoneIdx, statIdx = this.trtColStatusIdx;
        
        if(!useidx || index == statIdx)
        {
            var statInfo = this.getStatusInfo(values[statIdx][0], values[doneIdx], false);
            values[statIdx] = (statInfo[0] === "Status_Error" ? values[statIdx][1] || statInfo[1] : statInfo[1]);
        }
        
        for(var i = (index || 0); i < len; i++)
        {
            switch(this.trtColDefs[i][0])
            {
                	case "label":
                    case "name":
                    case "peers":
                    case "seeds":
                    case "status":
                    case "url":
                        break;
                        
                    case "added":
                    case "completed":
                        values[i] = (values[i] > 0) ? new Date(values[i]).toISOString() : "";
                        break;
                        
                    case "availability":
                        values[i] = (values[i] / 65536).toFixedNR(3);
                        break;
                    
                    case "done":
                        values[i] = (values[i]).toFixedNR(1) + "%";
                        break;
                        
                    case "downloaded":
                    case "uploaded":
                        values[i] = values[i].toFileSize();
                        break;
                        
                    case "downspeed":
                    case "upspeed":
                        values[i] = (values[i] >= 103) ? (values[i].toFileSize() + g_perSec) : "";
                        break;
                        
                    case "eta":
                        values[i] = (values[i] == 0) ? "" : (values[i] == -1) ? "\u221E" : values[i].toTimeDelta();
                        break;
                        
                    case "ratio":
                        values[i] = (values[i] == -1) ? "\u221E" : (values[i] / 1000).toFixedNR(3);
                        break;
                        
                    case "order":
                        values[i] = (values[i] <= -1) ? "*" : values[i];
                        break;
                        
                    case "remaining":
                        values[i] = (values[i] >= 103) ? values[i].toFileSize(2) : "";
                        break;
                        
                    case "seeds_peers":
                        values[i] = ($chk(values[i]) && (values[i] != Number.MAX_VALUE)) ? values[i].toFixedNR(3) : "\u221E";
                        break;
                        
                    case "size":
                        values[i] = values[i].toFileSize(2);
                        break;
            }
        }
        
        if(useidx)
        {
            return values[index];
        }
        else
        {
            return values;
        }
    },
    
    "trtSortCustom": function(col, dataX, dataY)
    {
        var ret = 0;
        
        return ret;
    },
    
    "trtSelect": function(ev, id)
    {
        this.updateToolbar();
        
        var selHash = this.trtTable.selectedRows;
        
        if(selHash.length === 0)
        {
            this.torrentID = "";
            this.clearDetails();
            return;
        }
        
        this.torrentID = id;
        
        if(this.config.showDetails)
        {
            this.showDetails(id);
        }
        
        if(ev.isRightClick())
        {
            this.showTrtMenu.delay(0, this, [ev, id]);
        }
    },
    
    "trtDblClick": function(id)
    {
        console.log("trtDblClick");
    },
    
    "showTrtMenu": function(ev, id)
    {
        if(!ev.isRightClick()) return;
        
        var menuItems = [];
        
        // label selection
        var labelIndex = CONST.TORRENT_LABEL;
        var labelSubMenu = [[L_("OV_NEW_LABEL"), this.newLabel.bind(this)]];
        
        if(!this.trtTable.selectedRows.every(function(item) { return (this.torrents[item][labelIndex] == ""); }, this))
        {
            labelSubMenu.push([L_("OV_REMOVE_LABEL"), this.setLabel.bind(this, "")]);
        }
        
        if(Object.getLength(this.labels) > 0)
        {
            labelSubMenu.push([CMENU_SEP]);
            
            $each(this.labels, function(_, label)
            {
                // check if every selected row has the given label, if so mark it as selected
                
                if (this.trtTable.selectedRows.every(function(item) { return (this.torrents[item][labelIndex] == label); }, this))
                {
                    labelSubMenu.push([CMENU_SEL, label]);
                }
                else
                {
                    labelSubMenu.push([label, this.setLabel.bind(this, label)]);
                }
            }, this);
        }
        
        // build menu
        var menuItemsMap =
        {
              "start" : [L_("ML_START"), this.start.bind(this)]
            , "pause" : [L_("ML_PAUSE"), this.pause.bind(this)]
            , "stop" : [L_("ML_STOP"), this.stop.bind(this)]
            , "queueup" : [L_("ML_QUEUEUP"), (function(ev) { this.queueup(ev.shift); }).bind(this)]
            , "queuedown" : [L_("ML_QUEUEDOWN"), (function(ev) { this.queuedown(ev.shift); }).bind(this)]
            , "label" : [CMENU_CHILD, L_("ML_LABEL"), labelSubMenu]
            , "remove" : [L_("ML_REMOVE"), this.remove.bind(this, CONST.TOR_REMOVE)]
            , "removeand" : [CMENU_CHILD, L_("ML_REMOVE_AND"),
              [
                  [L_("ML_DELETE_TORRENT"), this.remove.bind(this, CONST.TOR_REMOVE_TORRENT)]
                , [L_("ML_DELETE_DATATORRENT"), this.remove.bind(this, CONST.TOR_REMOVE_DATATORRENT)]
                , [L_("ML_DELETE_DATA"), this.remove.bind(this, CONST.TOR_REMOVE_DATA)]
              ]]
            , "recheck" : [L_("ML_FORCE_RECHECK"), this.recheck.bind(this)]
            , "copymagnet" : [L_("ML_COPY_MAGNETURI"), this.torShowMagnetCopy.bind(this)]
            , "copy" : [L_("MENU_COPY"), this.torShowCopy.bind(this)]
            , "properties" : [L_("ML_PROPERTIES"), this.showProperties.bind(this)]
        };
        
        // disable actions
        
        var disabled = this.getDisabledActions();

        Object.each(disabled, function(disabled, name)
        {
            var item = menuItemsMap[name];
            if (!item) return;

            if (disabled)
            {
                delete item[1];
            }
        });
        
        // create menu array
        menuItems = menuItems.concat([
              menuItemsMap["start"]
            , menuItemsMap["pause"]
            , menuItemsMap["stop"]
            , [CMENU_SEP]
            , menuItemsMap["queueup"]
            , menuItemsMap["queuedown"]
            , menuItemsMap["label"]
            , [CMENU_SEP]
            , menuItemsMap["remove"]
            , menuItemsMap["removeand"]
            , [CMENU_SEP]
            , menuItemsMap["recheck"]
            , [CMENU_SEP]
            , menuItemsMap["copymagnet"]
            , menuItemsMap["copy"]
            , [CMENU_SEP]
            , menuItemsMap["properties"]
        ]);

        // draw menu

        ContextMenu.clear();
        ContextMenu.add.apply(ContextMenu, menuItems);
        ContextMenu.show(ev.page);
    },
    
    "trtColReset": function()
    {
        console.log("trtColReset");
    },
    
    "trtSort": function(index, reverse)
    {
        console.log("trtSort");
    },
    
    "trtColMove": function()
    {
        console.log("trtColMove");
    },
    
    "trtColResize": function()
    {
        console.log("trtColResize");
    },
    
    "trtColToggle": function(index, enable, nosave)
    {
        var num = 1 << index;
        
        if(enable)
        {
            this.config.torrentTable.colMask |= num;
        }
        else
        {
            this.config.torrentTable.colMask &= ~num;
        }
        
        if(!nosave && Browser.opera)
        {
            this.saveConfig(true);
        }
    },
    
    "hideMsg": function()
    {
        $("cover").hide();
    },
    
    "beginPeriodicUpdate": function(delay)
    {
        this.endPeriodicUpdate();
        
        delay = parseInt(delay, 10);
        if(isNaN(delay)) delay = this.config.updateInterval;
        
        this.config.updateInterval = delay = delay.max(this.limits.minUpdateInterval);
        this.updateTimeout = this.update.delay(delay, this);
    },
    
    "endPeriodicUpdate": function()
    {
        clearTimeout(this.updateTimeout);
        clearInterval(this.updateTimeout);
    },
    
    "request": function(method, url, fn, async, fails)
    {
        if(typeOf(fails) != "array") fails = [0];
        
        var self = this;
        var really_async = true;
        
        if(really_async !== undefined)
        {
            really_async = async;
        }
        
        var req = function()
        {
            try
            {
                new Request.JSON({
                    "url": url,
                    "method": method,
                    "async": typeof(async) === "undefined" || !!async,
                    "onFailure": function()
                    {
                        self.endPeriodicUpdate();
                        
                        fails[0]++;
                        
                        var delay = Math.pow(self.limits.reqRetryDelayBase, fails[0]);
                        
                        if(fails[0] <= self.limits.reqRetryMaxAttempts)
                        {
                            log("Request failure #" + fails[0] + " (will retry in " + delay + " seconds): " + url);
                        }
                        else
                        {
                            window.removeEvents("unload");
                            
                            self.showMsg(
                                "<p>Cannot connect to Hadouken</p>" +
                                '<p>Try <a href="javascript:void(0);" onclick="window.location.reload(true);">reloading</a> the page</p>'
                            );
                            
                            return;
                        }
                        
                        self.request.delay(delay * 1000, self, [ url, function(json)
                        {
                            if(fails[0])
                            {
                                fails[0] = 0;
                                
                                log("Request retry succeeded: " + url);
                                if(fn) fn.delay(0, self, json);
                                self.beginPeriodicUpdate();
                            }
                        }, async, fails ]);
                    },
                    "onSuccess": (fn) ? fn.bind(self) : Function.from()
                }).send();
            }
            catch(e)
            {
                console.log(e);
            }
        };
        
        req();
    },
    
    "update": function(listcb)
    {
        if(window.hdknweb !== undefined) return;
        
        this.totalDL = 0;
        this.totalUL = 0;
        
        this.getList(null, (function()
        {
            // TODO: uncomment when implementing speed graph
            // this.spdGraph.addData(this.totalUL, this.totalDL);
            
            this.showDetails();
            
            this.updateTitle();
            this.updateToolbar();
            this.updateStatusBar();
            
            if(typeof(listcb) === "function") listcb();
        }).bind(this));
        
        if(typeof(DialogManager) !== "undefined")
        {
            if(DialogManager.showing.contains("Settings") && ("dlgSettings-TransferCap" == this.stpanes.active))
            {
                this.getTransferHistory();
            }
        }
    },
    
    "getList": function(url, fn)
    {
        this.endPeriodicUpdate();
        
        this.request("get", "/api/torrents", (function(json)
        {
            this.loadList(json);
            if(fn) fn(json);
        }).bind(this));
    },
    
    "getStatusInfo": function(state, progress, complete)
    {
        var res = ["", ""];
        
        if(state == CONST.STATE_STOPPED && complete)
        {
            res = ["Status_Complete", L_("OV_FL_FINISHED")];
        }
        else if((state == CONST.STATE_STOPPED || state == CONST.STATE_STOPPING) && !complete)
        {
            res = ["Status_Incomplete", L_("OV_FL_STOPPED")];
        }
        else if(state == CONST.STATE_PAUSED)
        {
            res = ["Status_Paused", L_("OV_FL_PAUSED")];
        }
        else if(state == CONST.STATE_DOWNLOADING)
        {
            res = complete ? ["Status_Up", L_("OV_FL_SEEDING")] : ["Status_Down", L_("OV_FL_DOWNLOADING")];
        }
        else if(state == CONST.STATE_SEEDING)
        {
            res = ["Status_Up", L_("OV_FL_SEEDING")];
        }
        else if(state == CONST.STATE_HASHING)
        {
            res = ["Status_Checking", L_("OV_FL_CHECKED").replace(/%:\.1d%/, (progress).toFixedNR(1))];
        }
        else if(state == CONST.STATE_ERROR)
        {
            res = ["Status_Error", L_("OV_FL_ERROR").replace(/%s/, "??")];
        }
        
        return res;
    },
    
    "loadList": function(json)
    {
        function extractLists(fullListName, changedListName, removedListName, key, exList)
        {
            var extracted = { hasChanged: false };
            
            if(!has(json, fullListName))
            {
                if(!has(json, changedListName))
                {
                    extracted[fullListName] = extracted[removedListName] = [];
                    extracted.hasChanged = false;
                }
                else
                {
                    extracted[fullListName] = json[changedListName];
                    delete json[changedListName];
                    
                    extracted[removedListName] = json[removedListName];
                    delete json[removedListName];
                    
                    extracted.hasChanged = ((extracted[fullListName].length + extracted[removedListName].length) > 0);
                }
            }
            else
            {
                extracted.hasChanged = true;
                
                var list = extracted[fullListName] = json[fullListName];
                delete json[fullListName];
                
                var removed = extracted[removedListName] = [];
                
                var exKeys = {};
                for(var k in exList)
                {
                    exKeys[k] = 1;
                }
                
                for(var i = 0, len = list.length; i < len; i++)
                {
                    if(has(exKeys, list[i][key])) delete exKeys[list[i][key]];
                }
                
                for(var k in exKeys)
                {
                    removed.push(k);
                }
            }
            
            return extracted;
        }
        
        if(!json.labels)
        {
            this.loadLabels(Array.clone([]));
        }
        else
        {
            this.loadLabels(Array.clone(json.labels));
        }
        
        (function(deltaLists)
        {
            var sortedColChanged = false;
            
            this.trtTable.keepScroll((function()
            {
                deltaLists.torrents.each(function(item)
                {
                    this.totalDL += item[CONST.TORRENT_DOWNSPEED];
                    this.totalUL += item[CONST.TORRENT_UPSPEED];
                    
                    var hash = item[CONST.TORRENT_HASH];
                    var statinfo = this.getStatusInfo(item.State, item.Progress, item.Complete);
                    
                    this.torGroups[hash] = this.getTorGroups(item);
                    
                    var row = this.trtDataToRow(item);
                    var ret = false;
                    var activeChanged = false;
                    
                    if(has(this.torrents, hash))
                    {
                        // old torrent found, update list
                        var rdata = this.trtTable.rowData[hash];
                        activeChanged = (rdata.hidden == this.torrentIsVisible(hash));
                        
                        if(activeChanged) rdata.hidden = !rdata.hidden;
                        
                        this.trtTable.setIcon(hash, statinfo[0]);
                        
                        row.each(function(v, k)
                        {
                            if(v != rdata.data[k])
                            {
                                ret = this.trtTable.updateCell(hash, k, row) || ret;
                                
                                if(this.trtColDefs[k][0] == "done")
                                {
                                    ret = this.trtTable.updateCell(hash, this.trtColStatusIdx, row) || ret;
                                }
                            }
                        }, this);
                        
                        if(!ret && activeChanged)
                        {
                            this.trtTable._insertRow(hash);
                        }
                    }
                    else
                    {
                        // new torrent found
                        this.trtTable.addRow(row, hash, statinfo[0], !this.torrentIsVisible(hash));
                        ret = true;
                    }
                    
                    this.torrents[hash] = item;
                    sortedColChanged = sortedColChanged || ret;
                }, this);
                
                this.trtTable.requiresRefresh = sortedColChanged || this.trtTable.requiresRefresh;
                
                // handle removed items
                
                var clear = false;
                
                if(window.hdknweb === undefined)
                {
                    deltaLists.torrentm.each(function(key)
                    {
                        Object.each(this.torGroups[key].cat, function(_, cat)
                        {
                            --this.categories[cat];
                        }, this);
                        
                        delete this.torGroups[key];
                        delete this.torrents[key];
                        
                        this.trtTable.removeRow(key);
                        
                        if(this.torrentID == key)
                        {
                            clear = true;
                        }
                    }, this);
                }
                
                if(clear)
                {
                    this.torrentID = "";
                    this.clearDetails();
                }
                
                // calc max torrent job queue number
                var queueMax = -1, q = CONST.TORRENT_QUEUE_POSITION;
                
                Object.each(this.torrents, function(trtData)
                {
                    if(queueMax < trtData[q])
                    {
                        queueMax = trtData[q];
                    }
                });
                
                this.torQueueMax = queueMax;
            }).bind(this));
            
            // finish up
            
            this.trtTable.resizePads();
            
            this.updateLabels();
            this.trtTable.refresh();
        }).bind(this)(extractLists("torrents", "torrentp", "torrentm", CONST.TORRENT_HASH, this.torrents));
        
        json = null;
        
        this.beginPeriodicUpdate();
    },
    
    "loadLabels": function(labels)
    {
        // TODO: remove return
        return;
        
        var labelList = $("mainCatList-labels"), temp = {};
        
        labels.each(function(lbl, idx)
        {
            var label = lbl[0], labelId = "lbl_" + encodeID(label), count = lbl[1], li = null;
            
            if((li = $(labelId)))
            {
                li.getElement(".count").set("text", count);
            }
            else
            {
                labelList.grab(new Element("li", { "id": labelId })
                    .appendText(label + " (")
                    .grab(new Element("span", { "class": "count", "text": count }))
                    .appendText(")")
                );
            }
            
            if(has(this.labels, label))
            {
                delete this.labels[label];
            }
            
            temp[label] = count;
        }, this);
        
        var activeChanged = false;
        
        for(var k in this.labels)
        {
            var id = "lbl_" + encodeID(k);
            
            if(this.config.activeTorGroups.lbl[id])
            {
                activeChanged = true;
            }
            
            delete this.config.activeTorGroups.lbl[id];
            $(id).destroy();
        }
        
        this.labels = temp;
        
        if(activeChanged)
        {
            var activeGroupCount =
            (
                Object.getLength(this.config.activeTorGroups.cat)
              + Object.getLength(this.config.activeTorGroups.lbl)
            );
            
            if(activeGroupCount <= 0)
            {
                this.config.activeTorGroups.cat["cat_all"] = 1;
            }
            
            this.refreshSelectedTorGroups();
        }
    },
    
    "updateLabels": function()
    {
        ["cat_all", "cat_dls", "cat_com", "cat_act", "cat_iac", "cat_nlb"].each(function(cat)
        {
            $(cat + "_c").set("text", this.categories[cat]);
        }, this);
    },
    
    "getSettings": function(fn)
    {
        var act = (function(json)
        {
            this.addSettings(json, fn);
        }).bind(this);
        
        this.request("get", "/api/settings", act);
    },
    
    "addSettings": function(json, fn)
    {
        var loadCookie = (function(newCookie)
        {
            function safeCopy(objOrig, objNew)
            {
                $each(objOrig, function(v, k)
                {
                    var tOrig = typeOf(objOrig[k])
                        tNew  = typeOf(objNew[k]);
                        
                    if(tOrig === tNew)
                    {
                        if(tOrig === "object")
                        {
                            safeCopy(objOrig[k], objNew[k]);
                        }
                        else
                        {
                            objOrig[k] = objNew[k];
                        }
                    }
                });
            }
            
            var cookie = this.config;
            
            newCookie = newCookie || {};
            safeCopy(cookie, newCookie);
            
            cookie.activeTorGroups = newCookie.activeTorGroups || this.defConfig.activeTorGroups || {};
            
            if(cookie.activeSettingsPane)
            {
                this.stpanes.show(cookie.activeSettingsPane.replace(/^tab_/, ""));
            }
            
            // set up listviews
            
            this.trtTable.setConfig({
                "colSort": [ cookie.torrentTable.sIndex, cookie.torrentTable.reverse ],
                "colMask": cookie.torrentTable.colMask,
                "colOrder": cookie.torrentTable.colOrder,
                "colWidth": cookie.torrentTable.colWidth
            });
            
            this.tableSetMaxRows(cookie.maxRows);
            
            resizeUI();
        }).bind(this);
        var tcmode = 0;
        
        for(var i = 0, j = json.settings.length; i < j; i++)
        {
            var key = json.settings[i][CONST.SETTING_NAME],
                typ = json.settings[i][CONST.SETTING_TYPE],
                val = json.settings[i][CONST.SETTING_VALUE],
                par = json.settings[i][CONST.SETTING_PARAM] || {};
                
            // handle cookie
            if(key === "webui.cookie")
            {
                loadCookie(JSON.decode(val, true));
            }
                
            // convert types
            switch(typ)
            {
                case CONST.SETTINGTYPE_INTEGER: val = val.toInt(); break;
                case CONST.SETTINGTYPE_BOOLEAN: val = (val === "true"); break;
            }
            
            // handle special settings
            // none yet
            
            if(par.access === CONST.SETTINGPARAM_ACCESS_RO)
            {
                var ele = $(key);
                if(ele) ele.addClass("disabled");
            }
            
            this.settings[key] = val;
            _unhideSetting(key);
        }
        
        delete json.settings;
        
        // load language
        if(!(this.config.lang in LANG_LIST))
        {
            var langList = "";
            
            for(var lang in LANG_LIST)
            {
                langList += "|" + lang;
            }
            
            var useLang = (navigator.language ? navigator.language : navigator.userLanguage || "").replace("-", "");
            
            if((useLang = useLang.match(new RegExp(langList.substr(1), "i"))))
            {
                useLang = useLang[0];
            }
            
            if(useLang && (useLang in LANG_LIST))
            {
                this.config.lang = useLang;
            }
            else
            {
                this.config.lang = (this.defConfig.lang || "en");
            }
        }
        
        loadLangStrings({
            "lang": this.config.lang,
            "onload": (function()
            {
                this.loadSettings();
                
                if(fn) fn();
            }).bind(this)
        });
    },
    
    "torrentIsVisible": function(hash)
    {
        var group = this.torGroups[hash];
        var actCat = this.config.activeTorGroups.cat;
        var actLbl = this.config.activeTorGroups.lbl;
        
        var visible = true;
        
        // Category: Downloading/Completed
        if (visible && (actCat["cat_dls"] || actCat["cat_com"]))
        {
            visible = visible && ((actCat["cat_dls"] && group.cat["cat_dls"]) || (actCat["cat_com"] && group.cat["cat_com"]));
        }

        // Category: Active/Inactive
        if (visible && (actCat["cat_act"] || actCat["cat_iac"]))
        {
            visible = visible && ((actCat["cat_act"] && group.cat["cat_act"]) || (actCat["cat_iac"] && group.cat["cat_iac"]));
        }

        // Labels
        if (visible && (actCat["cat_nlb"] || Object.some(actLbl, Function.from(true))))
        {
            visible = visible && ((actCat["cat_nlb"] && group.cat["cat_nlb"]) || Object.some(actLbl, function(_, lbl) { return group.lbl[lbl]; }));
        }

        return !!visible;
    },
    
    "getTorGroups": function(tor)
    {
        var groups = Object.merge({}, this.defTorGroup);
        
        // all
        groups.cat["cat_all"] = 1;
        
        // labels
        var lbls = Array.from(tor[CONST.TORRENT_LABEL] || []);
        
        if(lbls.length <= 0)
        {
            groups.cat["cat_nlb"] = 1;
        }
        else
        {
            lbls.each(function(lbl)
            {
                groups.lbl["lbl_" + encodeID(lbl)] = 1;
            });
        }
        
        // dl / complete
        
        if(tor[CONST.TORRENT_COMPLETE])
        {
            groups.cat["cat_com"] = 1;
        }
        else
        {
            groups.cat["cat_dls"] = 1;
        }
        
        // active / inactive
        
        if((tor[CONST.TORRENT_DOWNSPEED] > (this.settings["queue.slow_dl_threshold"] || 103)) ||
           (tor[CONST.TORRENT_UPSPEED] > (this.settings["queue.slow_ul_threshold"] || 103)))
        {
            groups.cat["cat_act"] = 1;
        }
        else
        {
            groups.cat["cat_iac"] = 1;
        }
        
        // update group counts
        // TODO: Move this elsewhere!
        (function(groups, oldGroups) {
            if (!oldGroups)
            {
                Object.each(groups.cat, function(_, cat)
                {
                    ++this.categories[cat];
                }, this);
            }
            else
            {
                // Labels
                if (groups.cat["cat_nlb"])
                {
                    if (!oldGroups.cat["cat_nlb"])
                    {
                        ++this.categories["cat_nlb"];
                    }
                }
                else
                {
                    if (oldGroups.cat["cat_nlb"])
                    {
                        --this.categories["cat_nlb"];
                    }
                }

                // Categories: Downloading/Completed
                if (groups.cat["cat_dls"])
                {
                    if (oldGroups.cat["cat_com"])
                    {
                        --this.categories["cat_com"];
                        ++this.categories["cat_dls"];
                    }
                }
                else
                {
                    if (oldGroups.cat["cat_dls"]) 
                    {
                        --this.categories["cat_dls"];
                        ++this.categories["cat_com"];
                    }
                }

                // Categories: Active/Inactive
                if (groups.cat["cat_act"])
                {
                    if (oldGroups.cat["cat_iac"])
                    {
                        --this.categories["cat_iac"];
                        ++this.categories["cat_act"];
                    }
                }
                else
                {
                    if (oldGroups.cat["cat_act"])
                    {
                        --this.categories["cat_act"];
                        ++this.categories["cat_iac"];
                    }
                }
            }
        }).bind(this)(groups, this.torGroups[tor[CONST.TORRENT_HASH]]);
        
        return groups;
    },
    
    "refreshSelectedTorGroups": function()
    {
        var deltaGroups;
        
        var oldGroups = this.__refreshSelectedTorGroups_activeTorGroups__;
        
        if(oldGroups)
        {
            var curGroups = this.config.activeTorGroups;
            var changeCount = 0;
            
            deltaGroups = {};
            
            // copy group type
            for(var type in oldGroups) { deltaGroups[type] = {}; }
            for(var type in curGroups) { deltaGroups[type] = {}; }
            
            // removed groups
            for(var type in oldGroups)
            {
                for(var group in oldGroups[type])
                {
                    if(!(group in curGroups[type]))
                    {
                        deltaGroups[type][group] = -1;
                        ++changeCount;
                    }
                }
            }
            
            // added groups
            for(var type in curGroups)
            {
                for(var group in curGroups[type])
                {
                    if(!(group in oldGroups[type]))
                    {
                        deltaGroups[type][group] = 1;
                        ++changeCount;
                    }
                }
            }
            
            if(!changeCount) return;
        }
        
        this.__refreshSelectedTorGroups_activeTorGroups__ = Object.merge({}, this.config.activeTorGroups);
        
        if(!oldGroups)
        {
            deltaGroups = this.config.activeTorGroups;
        }
        
        var val, ele;
        for(var type in deltaGroups)
        {
            for(var group in deltaGroups[type])
            {
                ele = $(group);
                if(!ele) continue;
                
                val = deltaGroups[type][group];
                
                if(val > 0)
                {
                    ele.addClass("sel");
                }
                else if(val < 0)
                {
                    ele.removeClass("sel");
                }
            }
        }
        
        // update detail info pane
        if(this.torrentID != "")
        {
            this.torrentID = "";
            this.clearDetails();
        }
        
        // update torrent jb list
        var activeChanged = false;
        
        for(var hash in this.torrents)
        {
            var changed = (!!this.trtTable.rowData[hash].hidden === !!this.torrentIsVisible(hash));
            
            if(changed)
            {
                activeChanged = true;
                
                if(this.trtTable.rowData[hash].hidden)
                {
                    this.trtTable.unhideRow(hash);
                }
                else
                {
                    this.trtTable.hideRow(hash);
                }
            }
        }
        
        this.trtTable.clearSelection(activeChanged);
        this.trtTable.curPage = 0;
        
        if(activeChanged)
        {
            this.trtTable.requiresRefresh = true;
            
            this.trtTable.calcSize();
            this.trtTable.restoreScroll();
            this.trtTable.resizePads();
        }
    },
    
    "loadSettings": function()
    {
        this.props.multi =
        {
            "trackers": 0,
            "ulrate": 0,
            "dlrate": 0,
            "superseed": 0,
            "dht": 0,
            "pex": 0,
            "seed_override": 0,
            "seed_ratio": 0,
            "seed_time": 0,
            "ulslots": 0
        };
        
        // TODO: implement advanced settings
        
        // other settings
        
        for(var k in this.settings)
        {
            var ele = $(k);
            if(!ele) continue;
            
            var v = this.settings[k];
            
            if(ele.type == "checkbox")
            {
                ele.checked = !!v;
            }
            else
            {
                switch(k)
                {
                    case "seed_ratio": v /= 10; break;
                    case "seed_time": v /= 60; break;
                }
                
                ele.set("value", v);
            }
            
            ele.fireEvent("change");
            
            if(Browser.ie)
            {
                ele.fireEvent("click");
            }
        }
        
        // webui config
        [ "useSysFont",
          "showDetails",
          "showCategories",
          "showToolbar",
          "showStatusBar",
          "updateInterval",
          "lang" ].each(function(key)
        {
            var ele;
            if(!(ele = $("webui." + key))) return;
            
            var v = this.config[key];
            
            if(ele.type == "checkbox")
            {
                ele.checked = ((v == 1) || (v == true));
            }
            else
            {
                ele.set("value", v);
            }
        }, this);
        
        if(this.config.maxRows < this.limits.minTableRows)
        {
            value = (this.config.maxRows <= 0 ? 0 : this.limits.minTableRows);
        }
        
        var elemaxrows = $("webui.maxRows");
        if(elemaxrows) elemaxrows.set("value", this.config.maxRows);
        
        this.toggleSystemFont(this.config.useSysFont);
        
        // hide toolbar
        if(!this.config.showToolbar) $("mainToolbar").hide();
        
        // hide category lists
        if(!this.config.showCategories) $("mainCatList").hide();
        
        // hide details
        if(!this.config.showDetails) $("mainInfoPane").hide();
        
        // hide tab icons
        if(!this.config.showDetailsIcons) $("mainInfoPane-tabs").removeClass("icon");
        
        // hide statusbar
        if(!this.config.showStatusBar) $("mainStatusBar").hide();
        
        this.toggleSearchBar();
    },
    
    "saveConfig": function(f)
    {
        console.log("saveconfig");
    },
    
    "showDetails": function(id)
    {
        var force = (id !== undefined);
        
        if(force)
        {
            this.torrentID = id;
        }
        else
        {
            id = this.torrentID;
        }
        
        switch(this.mainTabs.active)
        {
            case "mainInfoPane-generalTab":
                this.updateDetails(id);
                break;
                
            case "mainInfoPane-peersTab":
                this.getPeers(id, force);
                break;
                
            case "mainInfoPane-filesTab":
                this.getFiles(id, force);
                break;
        }
    },
    
    "updateTitle": function()
    {
        // TODO
    },
    
    "updateDetails": function(id)
    {
        if(!id) return;
        
        var d = this.torrents[id];
        
        $("dl").set("html", d[CONST.TORRENT_DOWNLOADED].toFileSize());
        $("ul").set("html", d[CONST.TORRENT_UPLOADED].toFileSize());
        
        var dl = d[CONST.TORRENT_DOWNLOADED];
        var ul = d[CONST.TORRENT_UPLOADED];
        $("ra").set("html", (dl == 0) ? (0).toFixedNR(3) : (ul / dl).toFixedNR(3));
        
        $("us").set("html", d[CONST.TORRENT_UPSPEED].toFileSize() + g_perSec);
        $("ds").set("html", d[CONST.TORRENT_DOWNSPEED].toFileSize() + g_perSec);
        $("rm").set("html", (d[CONST.TORRENT_ETA] == 0) ? "" : (d[CONST.TORRENT_ETA] <= -1) ? "\u221E" : d[CONST.TORRENT_ETA].toTimeDelta());
        $("se").set("html", L_("GN_XCONN").replace(/%d/, d[CONST.TORRENT_SEEDS_CONNECTED]).replace(/%d/, d[CONST.TORRENT_SEEDS_SWARM]).replace(/%d/, "\u00BF?"));
        $("pe").set("html", L_("GN_XCONN").replace(/%d/, d[CONST.TORRENT_PEERS_CONNECTED]).replace(/%d/, d[CONST.TORRENT_PEERS_SWARM]).replace(/%d/, "\u00BF?"));
        $("sa").set("html", d[CONST.TORRENT_SAVE_PATH] || "");
        $("hs").set("html", id);
    },
    
    "clearDetails": function()
    {
        ["rm", "dl", "ul", "ra", "us", "ds", "se", "pe", "sa", "hs"].each(function(id) {
            $(id).set("html", "");
        });
    },
    
    "showAddTorrent": function()
    {
        DialogManager.show("Add");
    },
    
    "showSettings": function()
    {
    },
    
    "showAddUrl": function()
    {
        DialogManager.show("AddURL");
    },
    
    "showAbout": function()
    {
        DialogManager.show("About");
    },
    
    "catListClick": function(ev, listId)
    {
        var element = ev.target;
        
        while(element && element.id !== listId && element.tagName.toLowerCase() !== "li")
        {
            element = element.getParent();
        }
        
        if(!element || !element.id || element.tagName.toLowerCase() !== "li") return;
        
        var eleId = element.id;
        
        if(eleId === "cat_nlb")
        {
            listId = "mainCatList-categories";
        }
        
        var activeGroupCount = (Object.getLength(this.config.activeTorGroups.cat) + Object.getLength(this.config.activeTorGroups.lbl));
        
        var prevSelected = activeGroupCount > 1 && Object.some(this.config.activeTorGroups, function(type)
        {
            return (eleId in type);
        });
        
        if((Browser.Platform.mac && ev.meta) || (!Browser.Platform.mac && ev.control))
        {
            if(ev.isRightClick())
            {
                prevSelected = false;
            }
        }
        else
        {
            if(!(ev.isRightClick() && prevSelected))
            {
                this.config.activeTorGroups = {};
                
                Object.each(this.defConfig.activeTorGroups, function(_, type)
                {
                    this.config.activeTorGroups[type] = {};
                }, this);
            }
            
            prevSelected = false;
        }
        
        var trtTableUpdate = (function()
        {
            this.refreshSelectedTorGroups();
            
            if(ev.isRightClick())
            {
                this.trtTable.fillSelection();
                this.trtTable.fireEvent("onSelect", ev);
            }
        }).bind(this);
        
        switch(listId)
        {
            case "mainCatList-categories":
                if(prevSelected)
                {
                    delete this.config.activeTorGroups.cat[eleId];
                }
                else
                {
                    this.config.activeTorGroups.cat[eleId] = 1;
                }
                
                trtTableUpdate();
                
                break;
                
            case "mainCatList-labels":
                if(prevSelected)
                {
                    delete this.config.activeTorGroups.lbl[eleId];
                }
                else
                {
                    this.config.activeTorGroups.lbl[eleId] = 1;
                }
                break;
        }
    },
    
    "detPanelTabChange": function(id)
    {
    },
    
    "updateToolbar": function()
    {
        var disabled = this.getDisabledActions();
        
        Object.each(disabled, function(disabled, name)
        {
            var item = $(name);
            if(!item) return;
            
            if(disabled)
            {
                item.addClass("disabled");
            }
            else
            {
                item.removeClass("disabled");
            }
        });
    },
    
    "toggleSearchBar": function(show)
    {
        show = (show === undefined ? !!(this.settings["search_list"] || "").trim() : !!show);
        $("mainToolbar-searchbar")[show ? "show" : "hide"]();
    },
    
    "toggleSystemFont": function(use)
    {
        use = (use === undefined ? !this.config.useSysFont : !!use);
        
        document.body[use ? "removeClass" : "addClass"]("nosysfont");
        this.config.useSysFont = use;
        
        resizeUI();
        
        if(Browser.opera) this.saveConfig(true);
    },
    
    "tableSetMaxRows": function(max)
    {
        var virtRows = this.limits.maxVirtTableRows;
        
        var mode = MODE_PAGE;
        max = max || 0;
        
        if(max <= 0)
        {
            mode = MODE_VIRTUAL;
            max = 0;
        }
        else if(max < this.limits.minTableRows)
        {
            max = this.limits.minTableRows;
        }
        
        this.config.maxRows = max;
        
        this.trtTable.setConfig({ "rowMaxCount": max || virtRows, "rowMode": mode });
        
        // do same for file and peers table
        // and advanced settings
    },
    
    "updateStatusBar": function()
    {
    },
    
    "settingsPaneChange": function(id)
    {
    },
    
    "getDisabledActions": function()
    {
        var disabled =
        {
            "start": 1,
            "stop": 1,
            "pause": 1,
            "remove": 1
        };
        
        return disabled;
    }
}

window.WebUI = WebUI;

})(window.jQuery);